using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace BlueStacks.Common
{
	public class CustomPictureBox : Image
	{
		public CustomPictureBox.State ButtonState { get; set; }

		public bool IsFullImagePath { get; set; }

		public string ImageName
		{
			get
			{
				return (string)base.GetValue(CustomPictureBox.ImageNameProperty);
			}
			set
			{
				base.SetValue(CustomPictureBox.ImageNameProperty, value);
			}
		}

		public bool IsImageHover
		{
			get
			{
				return (bool)base.GetValue(CustomPictureBox.IsImageHoverProperty);
			}
			set
			{
				base.SetValue(CustomPictureBox.IsImageHoverProperty, value);
			}
		}

		public bool IsAlwaysHalfSize
		{
			get
			{
				return (bool)base.GetValue(CustomPictureBox.IsAlwaysHalfSizeProperty);
			}
			set
			{
				base.SetValue(CustomPictureBox.IsAlwaysHalfSizeProperty, value);
			}
		}
		public static event EventHandler SourceUpdatedEvent;

		public bool IsImageToBeRotated
		{
			get
			{
				return this.mIsImageToBeRotated;
			}
			set
			{
				this.mIsImageToBeRotated = value;
				if (value)
				{
					base.SizeChanged -= this.CustomPictureBox_SizeChanged;
					base.IsVisibleChanged -= this.CustomPictureBox_IsVisibleChanged;
					base.SizeChanged += this.CustomPictureBox_SizeChanged;
					base.IsVisibleChanged += this.CustomPictureBox_IsVisibleChanged;
					return;
				}
				if (this.mStoryBoard != null)
				{
					this.mStoryBoard.Stop();
				}
				base.SizeChanged -= this.CustomPictureBox_SizeChanged;
				base.IsVisibleChanged -= this.CustomPictureBox_IsVisibleChanged;
			}
		}

		public void SetDisabledState()
		{
			this.ButtonState = CustomPictureBox.State.disabled;
			base.Opacity = 0.4;
			this.SetDefaultImage();
		}

		public void SetNormalState()
		{
			this.ButtonState = CustomPictureBox.State.normal;
			base.Opacity = 1.0;
		}

		private string AppendStringToImageName(string appendText)
		{
			if (this.ImageName.EndsWith(".png", StringComparison.InvariantCultureIgnoreCase) || this.ImageName.EndsWith(".jpg", StringComparison.InvariantCultureIgnoreCase) || this.ImageName.EndsWith(".jpeg", StringComparison.InvariantCultureIgnoreCase) || this.ImageName.EndsWith(".ico", StringComparison.InvariantCultureIgnoreCase))
			{
				string extension = Path.GetExtension(this.ImageName);
				string directoryName = Path.GetDirectoryName(this.ImageName);
				return string.Concat(new string[]
				{
					directoryName,
					Path.DirectorySeparatorChar.ToString(),
					Path.GetFileNameWithoutExtension(this.ImageName),
					appendText,
					extension
				});
			}
			return this.ImageName + appendText;
		}

		private string HoverImage
		{
			get
			{
				return this.AppendStringToImageName("_hover");
			}
		}

		private string ClickImage
		{
			get
			{
				return this.AppendStringToImageName("_click");
			}
		}

		private string DisabledImage
		{
			get
			{
				return this.AppendStringToImageName("_dis");
			}
		}

		public string SelectedImage
		{
			get
			{
				return this.AppendStringToImageName("_selected");
			}
		}

		public static string AssetsDir
		{
			get
			{
				return Path.Combine(RegistryManager.Instance.ClientInstallDir, RegistryManager.ClientThemeName);
			}
		}

		public CustomPictureBox()
		{
			base.MouseEnter += this.PictureBox_MouseEnter;
			base.MouseLeave += this.PictureBox_MouseLeave;
			base.MouseDown += this.PictureBox_MouseDown;
			base.IsEnabledChanged += this.PictureBox_IsEnabledChanged;
			base.MouseUp += this.PictureBox_MouseUp;
			RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.HighQuality);
		}

		public static void UpdateImagesFromNewDirectory(string path = "")
		{
			foreach (Tuple<string, bool> tuple in CustomPictureBox.sImageAssetsDict.Select((KeyValuePair<string, Tuple<BitmapImage, bool>> _) => new Tuple<string, bool>(_.Key, _.Value.Item2)).ToList<Tuple<string, bool>>())
			{
				if (tuple.Item1.IndexOfAny(new char[]
				{
					Path.AltDirectorySeparatorChar,
					Path.DirectorySeparatorChar
				}) == -1)
				{
					CustomPictureBox.sImageAssetsDict.Remove(tuple.Item1);
					CustomPictureBox.GetBitmapImage(tuple.Item1, path, tuple.Item2);
				}
			}
			CustomPictureBox.NotifyUIElements();
		}

		internal static void NotifyUIElements()
		{
			if (CustomPictureBox.SourceUpdatedEvent != null)
			{
				CustomPictureBox.SourceUpdatedEvent(null, null);
			}
		}

		private static void ImageNameChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
		{
			CustomPictureBox customPictureBox = source as CustomPictureBox;
			if (!DesignerProperties.GetIsInDesignMode(customPictureBox))
			{
				customPictureBox.SetDefaultImage();
			}
		}

		private static void IsImageHoverChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
		{
			CustomPictureBox customPictureBox = source as CustomPictureBox;
			if (!DesignerProperties.GetIsInDesignMode(customPictureBox))
			{
				if (customPictureBox.IsImageHover)
				{
					customPictureBox.SetHoverImage();
					return;
				}
				customPictureBox.SetDefaultImage();
			}
		}

		private static void IsAlwaysHalfSizeChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
		{
			CustomPictureBox customPictureBox = source as CustomPictureBox;
			if (!DesignerProperties.GetIsInDesignMode(customPictureBox))
			{
				customPictureBox.SetDefaultImage();
			}
		}

		private void PictureBox_MouseEnter(object sender, MouseEventArgs e)
		{
			if (this.ButtonState == CustomPictureBox.State.normal && !this.IsFullImagePath)
			{
				this.SetHoverImage();
			}
		}

		private void PictureBox_MouseLeave(object sender, MouseEventArgs e)
		{
			if (!this.IsFullImagePath)
			{
				this.SetDefaultImage();
			}
		}

		private void PictureBox_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (this.ButtonState == CustomPictureBox.State.normal && !this.IsFullImagePath)
			{
				this.SetClickedImage();
			}
		}

		private void PictureBox_MouseUp(object sender, MouseButtonEventArgs e)
		{
			if (!this.IsFullImagePath)
			{
				if (base.IsMouseOver && this.ButtonState == CustomPictureBox.State.normal)
				{
					this.SetHoverImage();
					return;
				}
				this.SetDefaultImage();
			}
		}

		public void SetHoverImage()
		{
			try
			{
				if (!this.IsFullImagePath)
				{
					CustomPictureBox.SetBitmapImage(this, this.HoverImage, false);
				}
			}
			catch (Exception)
			{
			}
		}

		public void SetClickedImage()
		{
			try
			{
				if (!this.IsFullImagePath)
				{
					CustomPictureBox.SetBitmapImage(this, this.ClickImage, false);
				}
			}
			catch (Exception)
			{
			}
		}

		public void SetSelectedImage()
		{
			try
			{
				if (!this.IsFullImagePath)
				{
					CustomPictureBox.SetBitmapImage(this, this.SelectedImage, false);
				}
			}
			catch (Exception)
			{
			}
		}

		public void SetDisabledImage()
		{
			try
			{
				if (!this.IsFullImagePath)
				{
					CustomPictureBox.SetBitmapImage(this, this.DisabledImage, false);
				}
			}
			catch (Exception)
			{
			}
		}

		public void SetDefaultImage()
		{
			try
			{
				CustomPictureBox.SetBitmapImage(this, this.ImageName, this.IsFullImagePath);
			}
			catch
			{
			}
		}

		public static BitmapImage GetBitmapImage(string fileName, string assetDirectory = "", bool isFullImagePath = false)
		{
			if (string.IsNullOrEmpty(fileName))
			{
				return null;
			}
			if (CustomPictureBox.sImageAssetsDict.ContainsKey(fileName))
			{
				return CustomPictureBox.sImageAssetsDict[fileName].Item1;
			}
			BitmapImage bitmapImage = null;
			if (fileName.IndexOfAny(new char[]
			{
				Path.AltDirectorySeparatorChar,
				Path.DirectorySeparatorChar
			}) != -1)
			{
				if (!isFullImagePath)
				{
					Logger.Warning("Full image path not marked false for image: " + fileName);
				}
				bitmapImage = CustomPictureBox.BitmapFromPath(fileName);
			}
			else if (isFullImagePath)
			{
				Logger.Warning("Full image path marked true for image: " + fileName);
			}
			if (bitmapImage == null)
			{
				if (string.IsNullOrEmpty(assetDirectory))
				{
					assetDirectory = CustomPictureBox.AssetsDir;
				}
				bitmapImage = CustomPictureBox.BitmapFromPath(Path.Combine(assetDirectory, Path.GetFileNameWithoutExtension(fileName) + ".png"));
				if (bitmapImage == null)
				{
					bitmapImage = CustomPictureBox.BitmapFromPath(Path.Combine(assetDirectory, fileName));
					if (bitmapImage == null)
					{
						bitmapImage = CustomPictureBox.BitmapFromPath(Path.Combine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets"), Path.GetFileNameWithoutExtension(fileName) + ".png"));
					}
				}
			}
			CustomPictureBox.sImageAssetsDict.Add(fileName, new Tuple<BitmapImage, bool>(bitmapImage, isFullImagePath));
			if (bitmapImage == null)
			{
				Logger.Warning("Returning a null image for {0}", new object[] { fileName });
			}
			return bitmapImage;
		}

		private static BitmapImage BitmapFromPath(string path)
		{
			BitmapImage bitmapImage = null;
			if (File.Exists(path))
			{
				bitmapImage = new BitmapImage();
				FileStream fileStream = File.OpenRead(path);
				bitmapImage.BeginInit();
				bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
				bitmapImage.StreamSource = fileStream;
				bitmapImage.EndInit();
				fileStream.Close();
				fileStream.Dispose();
			}
			return bitmapImage;
		}

		public static void SetBitmapImage(Image image, string fileName, bool isFullImagePath = false)
		{
			BitmapImage bitmapImage = CustomPictureBox.GetBitmapImage(fileName, "", isFullImagePath);
			if (bitmapImage != null)
			{
				bitmapImage.Freeze();
				BlueStacksUIBinding.Bind(image, Image.SourceProperty, fileName);
				if (image is CustomPictureBox)
				{
					CustomPictureBox customPictureBox = image as CustomPictureBox;
					customPictureBox.BitmapImage = bitmapImage;
					if (customPictureBox.IsAlwaysHalfSize)
					{
						customPictureBox.maxSize = new Point(customPictureBox.MaxWidth, customPictureBox.MaxHeight);
						customPictureBox.MaxWidth = bitmapImage.Width / 2.0;
						customPictureBox.MaxHeight = bitmapImage.Height / 2.0;
					}
					else if (customPictureBox.maxSize != default(Point))
					{
						customPictureBox.MaxWidth = customPictureBox.maxSize.X;
						customPictureBox.MaxHeight = customPictureBox.maxSize.Y;
					}
				}
			}
		}

		private void CustomPictureBox_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			if (base.RenderTransform != null)
			{
				RotateTransform rotateTransform = base.RenderTransform as RotateTransform;
				if (rotateTransform != null)
				{
					rotateTransform.CenterX = base.ActualWidth / 2.0;
					rotateTransform.CenterY = base.ActualHeight / 2.0;
				}
			}
		}

		private void CustomPictureBox_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (base.IsVisible && this.mIsImageToBeRotated)
			{
				if (this.mStoryBoard == null)
				{
					this.mStoryBoard = new Storyboard();
					this.animation = new DoubleAnimation
					{
						From = new double?(0.0),
						To = new double?((double)360),
						RepeatBehavior = RepeatBehavior.Forever,
						Duration = new Duration(new TimeSpan(0, 0, 1))
					};
					RotateTransform rotateTransform = new RotateTransform
					{
						CenterX = base.ActualWidth / 2.0,
						CenterY = base.ActualHeight / 2.0
					};
					base.RenderTransform = rotateTransform;
					Storyboard.SetTarget(this.animation, this);
					Storyboard.SetTargetProperty(this.animation, new PropertyPath("(UIElement.RenderTransform).(RotateTransform.Angle)", new object[0]));
					this.mStoryBoard.Children.Add(this.animation);
				}
				this.mStoryBoard.Begin();
				return;
			}
			Storyboard storyboard = this.mStoryBoard;
			if (storyboard == null)
			{
				return;
			}
			storyboard.Pause();
		}

		public bool IsDisabled
		{
			set
			{
				if (value)
				{
					base.MouseEnter -= this.PictureBox_MouseEnter;
					base.MouseLeave -= this.PictureBox_MouseLeave;
					base.MouseDown -= this.PictureBox_MouseDown;
					base.MouseUp -= this.PictureBox_MouseUp;
					base.IsEnabledChanged -= this.PictureBox_IsEnabledChanged;
					base.Opacity = 0.5;
				}
			}
		}

		private void PictureBox_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			CustomPictureBox customPictureBox = sender as CustomPictureBox;
			if (customPictureBox != null)
			{
				object newValue = e.NewValue;
				if (newValue is bool)
				{
					bool flag = (bool)newValue;
					if (flag)
					{
						customPictureBox.SetDefaultImage();
						return;
					}
					customPictureBox.SetDisabledImage();
				}
			}
		}

		public void ReloadImages()
		{
			CustomPictureBox.sImageAssetsDict.Remove(this.ClickImage);
			CustomPictureBox.sImageAssetsDict.Remove(this.ImageName);
			CustomPictureBox.sImageAssetsDict.Remove(this.HoverImage);
			CustomPictureBox.sImageAssetsDict.Remove(this.SelectedImage);
			CustomPictureBox.sImageAssetsDict.Remove(this.DisabledImage);
			this.SetDefaultImage();
			CustomPictureBox.GCCollectAsync();
		}

		private static void GCCollectAsync()
		{
			new Thread(new ThreadStart(delegate
			{
				GC.Collect();
			}))
			{
				IsBackground = true
			}.Start();
		}

		public bool AllowClickThrough
		{
			get
			{
				return (bool)base.GetValue(CustomPictureBox.AllowClickThroughProperty);
			}
			set
			{
				base.SetValue(CustomPictureBox.AllowClickThroughProperty, value);
			}
		}

		protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
		{
			try
			{
				if (hitTestParameters != null && this.AllowClickThrough)
				{
					Point position = Mouse.GetPosition(this);
					int pixelWidth = ((BitmapSource)base.Source).PixelWidth;
					int pixelHeight = ((BitmapSource)base.Source).PixelHeight;
					double num = position.X * (double)pixelWidth / base.ActualWidth;
					double num2 = position.Y * (double)pixelHeight / base.ActualHeight;
					byte[] array = new byte[4];
					try
					{
						new CroppedBitmap((BitmapSource)base.Source, new Int32Rect((int)num, (int)num2, 1, 1)).CopyPixels(array, 4, 0);
						if ((int)array[3] < RegistryManager.Instance.AdvancedControlTransparencyLevel)
						{
							Logger.Info(string.Format("HitTestCore pixel density at Image location- (X:{0} Y:{1}) is (R:{2} B:{3} G{4} A{5})", new object[]
							{
								num,
								num2,
								array[0],
								array[1],
								array[2],
								array[3]
							}));
							return null;
						}
					}
					catch (Exception)
					{
						Logger.Info(string.Format("Unable to get HitTestCore pixel density at Image location- X:{0} Y:{1}", num, num2));
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Error("HitTestCore: " + ex.Message);
			}
			return base.HitTestCore(hitTestParameters);
		}

		private Point maxSize;

		internal BitmapImage BitmapImage;

		internal DoubleAnimation animation;

		public static readonly DependencyProperty ImageNameProperty = DependencyProperty.Register("ImageName", typeof(string), typeof(CustomPictureBox), new FrameworkPropertyMetadata("", new PropertyChangedCallback(CustomPictureBox.ImageNameChanged)));

		public static readonly DependencyProperty IsImageHoverProperty = DependencyProperty.Register("IsImageHover", typeof(bool), typeof(CustomPictureBox), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(CustomPictureBox.IsImageHoverChanged)));

		public static readonly DependencyProperty IsAlwaysHalfSizeProperty = DependencyProperty.Register("IsAlwaysHalfSize", typeof(bool), typeof(CustomPictureBox), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(CustomPictureBox.IsAlwaysHalfSizeChanged)));

		public static readonly Dictionary<string, Tuple<BitmapImage, bool>> sImageAssetsDict = new Dictionary<string, Tuple<BitmapImage, bool>>();

		private Storyboard mStoryBoard;

		private bool mIsImageToBeRotated;

		public static readonly DependencyProperty AllowClickThroughProperty = DependencyProperty.Register("AllowClickThrough", typeof(bool), typeof(CustomPictureBox), new FrameworkPropertyMetadata(false));

		public enum State
		{
			normal,
			disabled
		}
	}
}


