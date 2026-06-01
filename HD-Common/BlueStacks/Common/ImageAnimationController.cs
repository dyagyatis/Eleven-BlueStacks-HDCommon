using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace BlueStacks.Common
{
	public class ImageAnimationController : IDisposable
	{
		internal ImageAnimationController(Image image, ObjectAnimationUsingKeyFrames animation, bool autoStart)
		{
			this._image = image;
			this._animation = animation;
			this._animation.Completed += this.AnimationCompleted;
			this._clock = this._animation.CreateClock();
			this._clockController = this._clock.Controller;
			ImageAnimationController._sourceDescriptor.AddValueChanged(image, new EventHandler(this.ImageSourceChanged));
			this._clockController.Pause();
			this._image.ApplyAnimationClock(Image.SourceProperty, this._clock);
			if (autoStart)
			{
				this._clockController.Resume();
			}
		}

		private void AnimationCompleted(object sender, EventArgs e)
		{
			this._image.RaiseEvent(new RoutedEventArgs(ImageBehavior.AnimationCompletedEvent, this._image));
		}

		private void ImageSourceChanged(object sender, EventArgs e)
		{
			this.OnCurrentFrameChanged();
		}

		public int FrameCount
		{
			get
			{
				return this._animation.KeyFrames.Count;
			}
		}

		public bool IsPaused
		{
			get
			{
				return this._clock.IsPaused;
			}
		}

		public bool IsComplete
		{
			get
			{
				return this._clock.CurrentState == ClockState.Filling;
			}
		}

		public void GotoFrame(int index)
		{
			ObjectKeyFrame objectKeyFrame = this._animation.KeyFrames[index];
			this._clockController.Seek(objectKeyFrame.KeyTime.TimeSpan, TimeSeekOrigin.BeginTime);
		}

		public int CurrentFrame
		{
			get
			{
				TimeSpan? time = this._clock.CurrentTime;
				var keyFrameData = this._animation.KeyFrames.Cast<ObjectKeyFrame>()
					.Select((f, i) => new KeyFrameInfo { Time = f.KeyTime.TimeSpan, Index = i })
					.FirstOrDefault(fi => fi.Time >= time);
				if (keyFrameData != null)
				{
					return keyFrameData.Index;
				}
				return -1;
			}
		}

		public void Pause()
		{
			this._clockController.Pause();
		}

		public void Play()
		{
			this._clockController.Resume();
		}
		public event EventHandler CurrentFrameChanged;

		private void OnCurrentFrameChanged()
		{
			EventHandler currentFrameChanged = this.CurrentFrameChanged;
			if (currentFrameChanged != null)
			{
				currentFrameChanged(this, EventArgs.Empty);
			}
		}

		~ImageAnimationController()
		{
			this.Dispose(false);
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				this._image.BeginAnimation(Image.SourceProperty, null);
				this._animation.Completed -= this.AnimationCompleted;
				ImageAnimationController._sourceDescriptor.RemoveValueChanged(this._image, new EventHandler(this.ImageSourceChanged));
				this._image.Source = null;
			}
		}

		private static readonly DependencyPropertyDescriptor _sourceDescriptor = DependencyPropertyDescriptor.FromProperty(Image.SourceProperty, typeof(Image));

		private readonly Image _image;

		private readonly ObjectAnimationUsingKeyFrames _animation;

		private readonly AnimationClock _clock;

		private readonly ClockController _clockController;
	}

	public class KeyFrameInfo
	{
		public TimeSpan Time { get; set; }
		public int Index { get; set; }
	}
}


