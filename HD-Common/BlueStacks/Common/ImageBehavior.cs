using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using BlueStacks.Common.Decoding;

namespace BlueStacks.Common
{
	public static class ImageBehavior
	{
		[AttachedPropertyBrowsableForType(typeof(Image))]
		public static ImageSource GetAnimatedSource(Image obj)
		{
			return (ImageSource)((obj != null) ? obj.GetValue(ImageBehavior.AnimatedSourceProperty) : null);
		}

		public static void SetAnimatedSource(Image obj, ImageSource value)
		{
			if (obj != null)
			{
				obj.SetValue(ImageBehavior.AnimatedSourceProperty, value);
			}
		}

		[AttachedPropertyBrowsableForType(typeof(Image))]
		public static RepeatBehavior GetRepeatBehavior(Image obj)
		{
			return (RepeatBehavior)((obj != null) ? obj.GetValue(ImageBehavior.RepeatBehaviorProperty) : null);
		}

		public static void SetRepeatBehavior(Image obj, RepeatBehavior value)
		{
			if (obj != null)
			{
				obj.SetValue(ImageBehavior.RepeatBehaviorProperty, value);
			}
		}

		public static bool GetAnimateInDesignMode(DependencyObject obj)
		{
			return (bool)((obj != null) ? obj.GetValue(ImageBehavior.AnimateInDesignModeProperty) : null);
		}

		public static void SetAnimateInDesignMode(DependencyObject obj, bool value)
		{
			if (obj != null)
			{
				obj.SetValue(ImageBehavior.AnimateInDesignModeProperty, value);
			}
		}

		[AttachedPropertyBrowsableForType(typeof(Image))]
		public static bool GetAutoStart(Image obj)
		{
			return (bool)((obj != null) ? obj.GetValue(ImageBehavior.AutoStartProperty) : null);
		}

		public static void SetAutoStart(Image obj, bool value)
		{
			if (obj != null)
			{
				obj.SetValue(ImageBehavior.AutoStartProperty, value);
			}
		}

		public static ImageAnimationController GetAnimationController(Image imageControl)
		{
			return (ImageAnimationController)((imageControl != null) ? imageControl.GetValue(ImageBehavior.AnimationControllerPropertyKey.DependencyProperty) : null);
		}

		private static void SetAnimationController(DependencyObject obj, ImageAnimationController value)
		{
			if (obj != null)
			{
				obj.SetValue(ImageBehavior.AnimationControllerPropertyKey, value);
			}
		}

		public static bool GetIsAnimationLoaded(Image image)
		{
			return (bool)((image != null) ? image.GetValue(ImageBehavior.IsAnimationLoadedProperty) : null);
		}

		private static void SetIsAnimationLoaded(Image image, bool value)
		{
			image.SetValue(ImageBehavior.IsAnimationLoadedPropertyKey, value);
		}

		public static void AddAnimationLoadedHandler(Image image, RoutedEventHandler handler)
		{
			if (image == null)
			{
				throw new ArgumentNullException("image");
			}
			if (handler == null)
			{
				throw new ArgumentNullException("handler");
			}
			image.AddHandler(ImageBehavior.AnimationLoadedEvent, handler);
		}

		public static void RemoveAnimationLoadedHandler(Image image, RoutedEventHandler handler)
		{
			if (image == null)
			{
				throw new ArgumentNullException("image");
			}
			if (handler == null)
			{
				throw new ArgumentNullException("handler");
			}
			image.RemoveHandler(ImageBehavior.AnimationLoadedEvent, handler);
		}

		public static void AddAnimationCompletedHandler(Image d, RoutedEventHandler handler)
		{
			if (d == null)
			{
				return;
			}
			d.AddHandler(ImageBehavior.AnimationCompletedEvent, handler);
		}

		public static void RemoveAnimationCompletedHandler(Image d, RoutedEventHandler handler)
		{
			if (d == null)
			{
				return;
			}
			d.RemoveHandler(ImageBehavior.AnimationCompletedEvent, handler);
		}

		private static void AnimatedSourceChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			Image image = o as Image;
			if (image == null)
			{
				return;
			}
			ImageSource imageSource = e.OldValue as ImageSource;
			ImageSource imageSource2 = e.NewValue as ImageSource;
			if (imageSource == imageSource2)
			{
				return;
			}
			if (imageSource != null)
			{
				image.Loaded -= ImageBehavior.ImageControlLoaded;
				image.Unloaded -= ImageBehavior.ImageControlUnloaded;
				AnimationCache.DecrementReferenceCount(imageSource, ImageBehavior.GetRepeatBehavior(image));
				ImageAnimationController animationController = ImageBehavior.GetAnimationController(image);
				if (animationController != null)
				{
					animationController.Dispose();
				}
				image.Source = null;
			}
			if (imageSource2 != null)
			{
				image.Loaded += ImageBehavior.ImageControlLoaded;
				image.Unloaded += ImageBehavior.ImageControlUnloaded;
				if (image.IsLoaded)
				{
					ImageBehavior.InitAnimationOrImage(image);
				}
			}
		}

		private static void ImageControlLoaded(object sender, RoutedEventArgs e)
		{
			Image image = sender as Image;
			if (image == null)
			{
				return;
			}
			ImageBehavior.InitAnimationOrImage(image);
		}

		private static void ImageControlUnloaded(object sender, RoutedEventArgs e)
		{
			Image image = sender as Image;
			if (image == null)
			{
				return;
			}
			ImageSource animatedSource = ImageBehavior.GetAnimatedSource(image);
			if (animatedSource != null)
			{
				AnimationCache.DecrementReferenceCount(animatedSource, ImageBehavior.GetRepeatBehavior(image));
			}
			ImageAnimationController animationController = ImageBehavior.GetAnimationController(image);
			if (animationController != null)
			{
				animationController.Dispose();
			}
		}

		private static void RepeatBehaviorChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			Image image = o as Image;
			if (image == null)
			{
				return;
			}
			ImageSource animatedSource = ImageBehavior.GetAnimatedSource(image);
			if (animatedSource != null)
			{
				if (!object.Equals(e.OldValue, e.NewValue))
				{
					AnimationCache.DecrementReferenceCount(animatedSource, (RepeatBehavior)e.OldValue);
				}
				if (image.IsLoaded)
				{
					ImageBehavior.InitAnimationOrImage(image);
				}
			}
		}

		private static void AnimateInDesignModeChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			Image image = o as Image;
			if (image == null)
			{
				return;
			}
			bool flag = (bool)e.NewValue;
			if (ImageBehavior.GetAnimatedSource(image) != null && image.IsLoaded)
			{
				if (flag)
				{
					ImageBehavior.InitAnimationOrImage(image);
					return;
				}
				image.BeginAnimation(Image.SourceProperty, null);
			}
		}

		private static void InitAnimationOrImage(Image imageControl)
		{
			ImageBehavior.SetAnimationController(imageControl, null);
			ImageBehavior.SetIsAnimationLoaded(imageControl, false);
			BitmapSource source = ImageBehavior.GetAnimatedSource(imageControl) as BitmapSource;
			int isInDesignMode = (DesignerProperties.GetIsInDesignMode(imageControl) ? 1 : 0);
			bool animateInDesignMode = ImageBehavior.GetAnimateInDesignMode(imageControl);
			bool flag = isInDesignMode == 0 || animateInDesignMode;
			bool flag2 = ImageBehavior.IsLoadingDeferred(source);
			if (source != null && flag && !flag2)
			{
				if (source.IsDownloading)
				{
					EventHandler handler = null;
					handler = delegate(object sender, EventArgs args)
					{
						source.DownloadCompleted -= handler;
						ImageBehavior.InitAnimationOrImage(imageControl);
					};
					source.DownloadCompleted += handler;
					imageControl.Source = source;
					return;
				}
				ObjectAnimationUsingKeyFrames animation = ImageBehavior.GetAnimation(imageControl, source);
				if (animation != null)
				{
					if (animation.KeyFrames.Count > 0)
					{
						ImageBehavior.TryTwice(delegate
						{
							imageControl.Source = (ImageSource)animation.KeyFrames[0].Value;
						});
					}
					else
					{
						imageControl.Source = source;
					}
					ImageAnimationController imageAnimationController = new ImageAnimationController(imageControl, animation, ImageBehavior.GetAutoStart(imageControl));
					ImageBehavior.SetAnimationController(imageControl, imageAnimationController);
					ImageBehavior.SetIsAnimationLoaded(imageControl, true);
					imageControl.RaiseEvent(new RoutedEventArgs(ImageBehavior.AnimationLoadedEvent, imageControl));
					return;
				}
			}
			imageControl.Source = source;
			if (source != null)
			{
				ImageBehavior.SetIsAnimationLoaded(imageControl, true);
				imageControl.RaiseEvent(new RoutedEventArgs(ImageBehavior.AnimationLoadedEvent, imageControl));
			}
		}

		private static ObjectAnimationUsingKeyFrames GetAnimation(Image imageControl, BitmapSource source)
		{
			ObjectAnimationUsingKeyFrames objectAnimationUsingKeyFrames = AnimationCache.GetAnimation(source, ImageBehavior.GetRepeatBehavior(imageControl));
			if (objectAnimationUsingKeyFrames != null)
			{
				return objectAnimationUsingKeyFrames;
			}
			GifFile gifFile;
			GifBitmapDecoder gifBitmapDecoder = ImageBehavior.GetDecoder(source, out gifFile) as GifBitmapDecoder;
			if (gifBitmapDecoder != null && gifBitmapDecoder.Frames.Count > 1)
			{
				ImageBehavior.Int32Size fullSize = ImageBehavior.GetFullSize(gifBitmapDecoder, gifFile);
				int num = 0;
				objectAnimationUsingKeyFrames = new ObjectAnimationUsingKeyFrames();
				TimeSpan timeSpan = TimeSpan.Zero;
				BitmapSource bitmapSource = null;
				foreach (BitmapFrame bitmapFrame in gifBitmapDecoder.Frames)
				{
					ImageBehavior.FrameMetadata frameMetadata = ImageBehavior.GetFrameMetadata(gifBitmapDecoder, gifFile, num);
					BitmapSource bitmapSource2 = ImageBehavior.MakeFrame(fullSize, bitmapFrame, frameMetadata, bitmapSource);
					DiscreteObjectKeyFrame discreteObjectKeyFrame = new DiscreteObjectKeyFrame(bitmapSource2, timeSpan);
					objectAnimationUsingKeyFrames.KeyFrames.Add(discreteObjectKeyFrame);
					timeSpan += frameMetadata.Delay;
					switch (frameMetadata.DisposalMethod)
					{
					case ImageBehavior.FrameDisposalMethod.None:
					case ImageBehavior.FrameDisposalMethod.DoNotDispose:
						bitmapSource = bitmapSource2;
						break;
					case ImageBehavior.FrameDisposalMethod.RestoreBackground:
						if (ImageBehavior.IsFullFrame(frameMetadata, fullSize))
						{
							bitmapSource = null;
						}
						else
						{
							bitmapSource = ImageBehavior.ClearArea(bitmapSource2, frameMetadata);
						}
						break;
					}
					num++;
				}
				objectAnimationUsingKeyFrames.Duration = timeSpan;
				objectAnimationUsingKeyFrames.RepeatBehavior = ImageBehavior.GetActualRepeatBehavior(imageControl, gifBitmapDecoder, gifFile);
				AnimationCache.AddAnimation(source, ImageBehavior.GetRepeatBehavior(imageControl), objectAnimationUsingKeyFrames);
				AnimationCache.IncrementReferenceCount(source, ImageBehavior.GetRepeatBehavior(imageControl));
				return objectAnimationUsingKeyFrames;
			}
			return null;
		}

		private static BitmapSource ClearArea(BitmapSource frame, ImageBehavior.FrameMetadata metadata)
		{
			DrawingVisual drawingVisual = new DrawingVisual();
			using (DrawingContext drawingContext = drawingVisual.RenderOpen())
			{
				Rect rect = new Rect(0.0, 0.0, (double)frame.PixelWidth, (double)frame.PixelHeight);
				Rect rect2 = new Rect((double)metadata.Left, (double)metadata.Top, (double)metadata.Width, (double)metadata.Height);
				PathGeometry pathGeometry = Geometry.Combine(new RectangleGeometry(rect), new RectangleGeometry(rect2), GeometryCombineMode.Exclude, null);
				drawingContext.PushClip(pathGeometry);
				drawingContext.DrawImage(frame, rect);
			}
			RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap(frame.PixelWidth, frame.PixelHeight, frame.DpiX, frame.DpiY, PixelFormats.Pbgra32);
			renderTargetBitmap.Render(drawingVisual);
			if (renderTargetBitmap.CanFreeze && !renderTargetBitmap.IsFrozen)
			{
				renderTargetBitmap.Freeze();
			}
			return renderTargetBitmap;
		}

		private static void TryTwice(Action action)
		{
			try
			{
				action();
			}
			catch (Exception)
			{
				action();
			}
		}

		private static bool IsLoadingDeferred(BitmapSource source)
		{
			BitmapImage bitmapImage = source as BitmapImage;
			return bitmapImage != null && (bitmapImage.UriSource != null && !bitmapImage.UriSource.IsAbsoluteUri) && bitmapImage.BaseUri == null;
		}

		private static BitmapDecoder GetDecoder(BitmapSource image, out GifFile gifFile)
		{
			gifFile = null;
			BitmapDecoder bitmapDecoder = null;
			Stream stream = null;
			Uri uri = null;
			BitmapCreateOptions bitmapCreateOptions = BitmapCreateOptions.None;
			BitmapImage bitmapImage = image as BitmapImage;
			if (bitmapImage != null)
			{
				bitmapCreateOptions = bitmapImage.CreateOptions;
				if (bitmapImage.StreamSource != null)
				{
					stream = bitmapImage.StreamSource;
				}
				else if (bitmapImage.UriSource != null)
				{
					uri = bitmapImage.UriSource;
					if (bitmapImage.BaseUri != null && !uri.IsAbsoluteUri)
					{
						uri = new Uri(bitmapImage.BaseUri, uri);
					}
				}
			}
			else
			{
				BitmapFrame bitmapFrame = image as BitmapFrame;
				if (bitmapFrame != null)
				{
					bitmapDecoder = bitmapFrame.Decoder;
					Uri.TryCreate(bitmapFrame.BaseUri, bitmapFrame.ToString(CultureInfo.InvariantCulture), out uri);
				}
			}
			if (bitmapDecoder == null)
			{
				if (stream != null)
				{
					stream.Position = 0L;
					bitmapDecoder = BitmapDecoder.Create(stream, bitmapCreateOptions, BitmapCacheOption.OnLoad);
				}
				else if (uri != null && uri.IsAbsoluteUri)
				{
					bitmapDecoder = BitmapDecoder.Create(uri, bitmapCreateOptions, BitmapCacheOption.OnLoad);
				}
			}
			if (bitmapDecoder is GifBitmapDecoder && !ImageBehavior.CanReadNativeMetadata(bitmapDecoder))
			{
				if (stream != null)
				{
					stream.Position = 0L;
					gifFile = GifFile.ReadGifFile(stream, true);
				}
				else
				{
					if (!(uri != null))
					{
						throw new InvalidOperationException("Can't get URI or Stream from the source. AnimatedSource should be either a BitmapImage, or a BitmapFrame constructed from a URI.");
					}
					gifFile = ImageBehavior.DecodeGifFile(uri);
				}
			}
			if (bitmapDecoder == null)
			{
				throw new InvalidOperationException("Can't get a decoder from the source. AnimatedSource should be either a BitmapImage or a BitmapFrame.");
			}
			return bitmapDecoder;
		}

		private static bool CanReadNativeMetadata(BitmapDecoder decoder)
		{
			bool flag;
			try
			{
				flag = decoder.Metadata != null;
			}
			catch
			{
				flag = false;
			}
			return flag;
		}

		private static GifFile DecodeGifFile(Uri uri)
		{
			Stream stream = null;
			if (uri.Scheme == PackUriHelper.UriSchemePack)
			{
				StreamResourceInfo streamResourceInfo;
				if (uri.Authority == "siteoforigin:,,,")
				{
					streamResourceInfo = Application.GetRemoteStream(uri);
				}
				else
				{
					streamResourceInfo = Application.GetResourceStream(uri);
				}
				if (streamResourceInfo != null)
				{
					stream = streamResourceInfo.Stream;
				}
			}
			else
			{
				using (WebClient webClient = new WebClient())
				{
					stream = webClient.OpenRead(uri);
				}
			}
			if (stream != null)
			{
				using (stream)
				{
					return GifFile.ReadGifFile(stream, true);
				}
			}
			return null;
		}

		private static bool IsFullFrame(ImageBehavior.FrameMetadata metadata, ImageBehavior.Int32Size fullSize)
		{
			return metadata.Left == 0 && metadata.Top == 0 && metadata.Width == fullSize.Width && metadata.Height == fullSize.Height;
		}

		private static BitmapSource MakeFrame(ImageBehavior.Int32Size fullSize, BitmapSource rawFrame, ImageBehavior.FrameMetadata metadata, BitmapSource baseFrame)
		{
			if (baseFrame == null && ImageBehavior.IsFullFrame(metadata, fullSize))
			{
				return rawFrame;
			}
			DrawingVisual drawingVisual = new DrawingVisual();
			using (DrawingContext drawingContext = drawingVisual.RenderOpen())
			{
				if (baseFrame != null)
				{
					Rect rect = new Rect(0.0, 0.0, (double)fullSize.Width, (double)fullSize.Height);
					drawingContext.DrawImage(baseFrame, rect);
				}
				Rect rect2 = new Rect((double)metadata.Left, (double)metadata.Top, (double)metadata.Width, (double)metadata.Height);
				drawingContext.DrawImage(rawFrame, rect2);
			}
			RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap(fullSize.Width, fullSize.Height, 96.0, 96.0, PixelFormats.Pbgra32);
			renderTargetBitmap.Render(drawingVisual);
			if (renderTargetBitmap.CanFreeze && !renderTargetBitmap.IsFrozen)
			{
				renderTargetBitmap.Freeze();
			}
			return renderTargetBitmap;
		}

		private static RepeatBehavior GetActualRepeatBehavior(Image imageControl, BitmapDecoder decoder, GifFile gifMetadata)
		{
			RepeatBehavior repeatBehavior = ImageBehavior.GetRepeatBehavior(imageControl);
			if (repeatBehavior != default(RepeatBehavior))
			{
				return repeatBehavior;
			}
			int num;
			if (gifMetadata != null)
			{
				num = (int)gifMetadata.RepeatCount;
			}
			else
			{
				num = ImageBehavior.GetRepeatCount(decoder);
			}
			if (num == 0)
			{
				return RepeatBehavior.Forever;
			}
			return new RepeatBehavior((double)num);
		}

		private static int GetRepeatCount(BitmapDecoder decoder)
		{
			BitmapMetadata applicationExtension = ImageBehavior.GetApplicationExtension(decoder, "NETSCAPE2.0");
			if (applicationExtension != null)
			{
				byte[] queryOrNull = applicationExtension.GetQueryOrNull<byte[]>("/Data");
				if (queryOrNull != null && queryOrNull.Length >= 4)
				{
					return (int)BitConverter.ToUInt16(queryOrNull, 2);
				}
			}
			return 1;
		}

		private static BitmapMetadata GetApplicationExtension(BitmapDecoder decoder, string application)
		{
			int num = 0;
			string text = "/appext";
			for (BitmapMetadata bitmapMetadata = decoder.Metadata.GetQueryOrNull<BitmapMetadata>(text); bitmapMetadata != null; bitmapMetadata = decoder.Metadata.GetQueryOrNull<BitmapMetadata>(text))
			{
				byte[] queryOrNull = bitmapMetadata.GetQueryOrNull<byte[]>("/Application");
				if (queryOrNull != null && Encoding.ASCII.GetString(queryOrNull) == application)
				{
					return bitmapMetadata;
				}
				text = string.Format(CultureInfo.InvariantCulture, "/[{0}]appext", new object[] { ++num });
			}
			return null;
		}

		private static ImageBehavior.FrameMetadata GetFrameMetadata(BitmapDecoder decoder, GifFile gifMetadata, int frameIndex)
		{
			if (gifMetadata != null && gifMetadata.Frames.Count > frameIndex)
			{
				return ImageBehavior.GetFrameMetadata(gifMetadata.Frames[frameIndex]);
			}
			return ImageBehavior.GetFrameMetadata(decoder.Frames[frameIndex]);
		}

		private static ImageBehavior.FrameMetadata GetFrameMetadata(BitmapFrame frame)
		{
			BitmapMetadata bitmapMetadata = (BitmapMetadata)frame.Metadata;
			TimeSpan timeSpan = TimeSpan.FromMilliseconds(100.0);
			int queryOrDefault = bitmapMetadata.GetQueryOrDefault("/grctlext/Delay", 10);
			if (queryOrDefault != 0)
			{
				timeSpan = TimeSpan.FromMilliseconds((double)(queryOrDefault * 10));
			}
			ImageBehavior.FrameDisposalMethod queryOrDefault2 = (ImageBehavior.FrameDisposalMethod)bitmapMetadata.GetQueryOrDefault("/grctlext/Disposal", 0);
			return new ImageBehavior.FrameMetadata
			{
				Left = bitmapMetadata.GetQueryOrDefault("/imgdesc/Left", 0),
				Top = bitmapMetadata.GetQueryOrDefault("/imgdesc/Top", 0),
				Width = bitmapMetadata.GetQueryOrDefault("/imgdesc/Width", frame.PixelWidth),
				Height = bitmapMetadata.GetQueryOrDefault("/imgdesc/Height", frame.PixelHeight),
				Delay = timeSpan,
				DisposalMethod = queryOrDefault2
			};
		}

		private static ImageBehavior.FrameMetadata GetFrameMetadata(GifFrame gifMetadata)
		{
			GifImageDescriptor descriptor = gifMetadata.Descriptor;
			ImageBehavior.FrameMetadata frameMetadata = new ImageBehavior.FrameMetadata
			{
				Left = descriptor.Left,
				Top = descriptor.Top,
				Width = descriptor.Width,
				Height = descriptor.Height,
				Delay = TimeSpan.FromMilliseconds(100.0),
				DisposalMethod = ImageBehavior.FrameDisposalMethod.None
			};
			GifGraphicControlExtension gifGraphicControlExtension = gifMetadata.Extensions.OfType<GifGraphicControlExtension>().FirstOrDefault<GifGraphicControlExtension>();
			if (gifGraphicControlExtension != null)
			{
				if (gifGraphicControlExtension.Delay != 0)
				{
					frameMetadata.Delay = TimeSpan.FromMilliseconds((double)gifGraphicControlExtension.Delay);
				}
				frameMetadata.DisposalMethod = (ImageBehavior.FrameDisposalMethod)gifGraphicControlExtension.DisposalMethod;
			}
			return frameMetadata;
		}

		private static ImageBehavior.Int32Size GetFullSize(BitmapDecoder decoder, GifFile gifMetadata)
		{
			if (gifMetadata != null)
			{
				GifLogicalScreenDescriptor logicalScreenDescriptor = gifMetadata.Header.LogicalScreenDescriptor;
				return new ImageBehavior.Int32Size(logicalScreenDescriptor.Width, logicalScreenDescriptor.Height);
			}
			int queryOrDefault = decoder.Metadata.GetQueryOrDefault("/logscrdesc/Width", 0);
			int queryOrDefault2 = decoder.Metadata.GetQueryOrDefault("/logscrdesc/Height", 0);
			return new ImageBehavior.Int32Size(queryOrDefault, queryOrDefault2);
		}

		private static T GetQueryOrDefault<T>(this BitmapMetadata metadata, string query, T defaultValue)
		{
			if (metadata.ContainsQuery(query))
			{
				return (T)((object)Convert.ChangeType(metadata.GetQuery(query), typeof(T), CultureInfo.InvariantCulture));
			}
			return defaultValue;
		}

		private static T GetQueryOrNull<T>(this BitmapMetadata metadata, string query) where T : class
		{
			if (metadata.ContainsQuery(query))
			{
				return metadata.GetQuery(query) as T;
			}
			return default(T);
		}

		public static readonly DependencyProperty AnimatedSourceProperty = DependencyProperty.RegisterAttached("AnimatedSource", typeof(ImageSource), typeof(ImageBehavior), new UIPropertyMetadata(null, new PropertyChangedCallback(ImageBehavior.AnimatedSourceChanged)));

		public static readonly DependencyProperty RepeatBehaviorProperty = DependencyProperty.RegisterAttached("RepeatBehavior", typeof(RepeatBehavior), typeof(ImageBehavior), new UIPropertyMetadata(default(RepeatBehavior), new PropertyChangedCallback(ImageBehavior.RepeatBehaviorChanged)));

		public static readonly DependencyProperty AnimateInDesignModeProperty = DependencyProperty.RegisterAttached("AnimateInDesignMode", typeof(bool), typeof(ImageBehavior), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits, new PropertyChangedCallback(ImageBehavior.AnimateInDesignModeChanged)));

		public static readonly DependencyProperty AutoStartProperty = DependencyProperty.RegisterAttached("AutoStart", typeof(bool), typeof(ImageBehavior), new PropertyMetadata(true));

		private static readonly DependencyPropertyKey AnimationControllerPropertyKey = DependencyProperty.RegisterAttachedReadOnly("AnimationController", typeof(ImageAnimationController), typeof(ImageBehavior), new PropertyMetadata(null));

		private static readonly DependencyPropertyKey IsAnimationLoadedPropertyKey = DependencyProperty.RegisterAttachedReadOnly("IsAnimationLoaded", typeof(bool), typeof(ImageBehavior), new PropertyMetadata(false));

		public static readonly DependencyProperty IsAnimationLoadedProperty = ImageBehavior.IsAnimationLoadedPropertyKey.DependencyProperty;

		public static readonly RoutedEvent AnimationLoadedEvent = EventManager.RegisterRoutedEvent("AnimationLoaded", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ImageBehavior));

		public static readonly RoutedEvent AnimationCompletedEvent = EventManager.RegisterRoutedEvent("AnimationCompleted", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ImageBehavior));

		private struct Int32Size
		{
			public Int32Size(int width, int height)
			{
				this = default(ImageBehavior.Int32Size);
				this.Width = width;
				this.Height = height;
			}

			public int Width { readonly get; private set; }

			public int Height { readonly get; private set; }
		}

		private class FrameMetadata
		{
			public int Left { get; set; }

			public int Top { get; set; }

			public int Width { get; set; }

			public int Height { get; set; }

			public TimeSpan Delay { get; set; }

			public ImageBehavior.FrameDisposalMethod DisposalMethod { get; set; }
		}

		private enum FrameDisposalMethod
		{
			None,
			DoNotDispose,
			RestoreBackground,
			RestorePrevious
		}
	}
}


