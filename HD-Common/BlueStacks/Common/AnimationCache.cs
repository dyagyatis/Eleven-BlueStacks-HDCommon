using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace BlueStacks.Common
{
	internal static class AnimationCache
	{
		public static void IncrementReferenceCount(ImageSource source, RepeatBehavior repeatBehavior)
		{
			AnimationCache.CacheKey cacheKey = new AnimationCache.CacheKey(source, repeatBehavior);
			int num;
			AnimationCache._referenceCount.TryGetValue(cacheKey, out num);
			num++;
			AnimationCache._referenceCount[cacheKey] = num;
		}

		public static void DecrementReferenceCount(ImageSource source, RepeatBehavior repeatBehavior)
		{
			AnimationCache.CacheKey cacheKey = new AnimationCache.CacheKey(source, repeatBehavior);
			int num;
			AnimationCache._referenceCount.TryGetValue(cacheKey, out num);
			if (num > 0)
			{
				num--;
				AnimationCache._referenceCount[cacheKey] = num;
			}
			if (num == 0)
			{
				AnimationCache._animationCache.Remove(cacheKey);
				AnimationCache._referenceCount.Remove(cacheKey);
			}
		}

		public static void AddAnimation(ImageSource source, RepeatBehavior repeatBehavior, ObjectAnimationUsingKeyFrames animation)
		{
			AnimationCache.CacheKey cacheKey = new AnimationCache.CacheKey(source, repeatBehavior);
			AnimationCache._animationCache[cacheKey] = animation;
		}

		public static void RemoveAnimation(ImageSource source, RepeatBehavior repeatBehavior, ObjectAnimationUsingKeyFrames _)
		{
			AnimationCache.CacheKey cacheKey = new AnimationCache.CacheKey(source, repeatBehavior);
			AnimationCache._animationCache.Remove(cacheKey);
		}

		public static ObjectAnimationUsingKeyFrames GetAnimation(ImageSource source, RepeatBehavior repeatBehavior)
		{
			AnimationCache.CacheKey cacheKey = new AnimationCache.CacheKey(source, repeatBehavior);
			ObjectAnimationUsingKeyFrames objectAnimationUsingKeyFrames;
			AnimationCache._animationCache.TryGetValue(cacheKey, out objectAnimationUsingKeyFrames);
			return objectAnimationUsingKeyFrames;
		}

		private static readonly Dictionary<AnimationCache.CacheKey, ObjectAnimationUsingKeyFrames> _animationCache = new Dictionary<AnimationCache.CacheKey, ObjectAnimationUsingKeyFrames>();

		private static readonly Dictionary<AnimationCache.CacheKey, int> _referenceCount = new Dictionary<AnimationCache.CacheKey, int>();

		private class CacheKey
		{
			public CacheKey(ImageSource source, RepeatBehavior repeatBehavior)
			{
				this._source = source;
				this._repeatBehavior = repeatBehavior;
			}

			private bool Equals(AnimationCache.CacheKey other)
			{
				return AnimationCache.CacheKey.ImageEquals(this._source, other._source) && object.Equals(this._repeatBehavior, other._repeatBehavior);
			}

			public override bool Equals(object obj)
			{
				return obj != null && (this == obj || (obj.GetType() == base.GetType() && this.Equals((AnimationCache.CacheKey)obj)));
			}

			public override int GetHashCode()
			{
				return (AnimationCache.CacheKey.ImageGetHashCode(this._source) * 397) ^ this._repeatBehavior.GetHashCode();
			}

			private static int ImageGetHashCode(ImageSource image)
			{
				if (image != null)
				{
					Uri uri = AnimationCache.CacheKey.GetUri(image);
					if (uri != null)
					{
						return uri.GetHashCode();
					}
				}
				return 0;
			}

			private static bool ImageEquals(ImageSource x, ImageSource y)
			{
				if (object.Equals(x, y))
				{
					return true;
				}
				if (x == null != (y == null))
				{
					return false;
				}
				if (x.GetType() != y.GetType())
				{
					return false;
				}
				Uri uri = AnimationCache.CacheKey.GetUri(x);
				Uri uri2 = AnimationCache.CacheKey.GetUri(y);
				return uri != null && uri == uri2;
			}

			private static Uri GetUri(ImageSource image)
			{
				BitmapImage bitmapImage = image as BitmapImage;
				if (bitmapImage != null && bitmapImage.UriSource != null)
				{
					if (bitmapImage.UriSource.IsAbsoluteUri)
					{
						return bitmapImage.UriSource;
					}
					if (bitmapImage.BaseUri != null)
					{
						return new Uri(bitmapImage.BaseUri, bitmapImage.UriSource);
					}
				}
				BitmapFrame bitmapFrame = image as BitmapFrame;
				if (bitmapFrame != null)
				{
					string text = bitmapFrame.ToString(CultureInfo.InvariantCulture);
					Uri uri;
					if (text != bitmapFrame.GetType().FullName && Uri.TryCreate(text, UriKind.RelativeOrAbsolute, out uri))
					{
						if (uri.IsAbsoluteUri)
						{
							return uri;
						}
						if (bitmapFrame.BaseUri != null)
						{
							return new Uri(bitmapFrame.BaseUri, uri);
						}
					}
				}
				return null;
			}

			private readonly ImageSource _source;

			private readonly RepeatBehavior _repeatBehavior;
		}
	}
}


