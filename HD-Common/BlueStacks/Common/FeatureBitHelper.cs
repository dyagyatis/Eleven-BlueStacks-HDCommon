using System;

namespace BlueStacks.Common
{
	public static class FeatureBitHelper
	{
		public static bool IsFeatureEnabled(ulong featureMask, ulong feature)
		{
			bool flag = (featureMask & 8858387942UL) > 0UL;
			return !flag && (feature & featureMask) > 0UL;
		}

		public static ulong EnableFeature(ulong featureMask, ulong feature)
		{
			bool flag = (featureMask & 8858387942UL) > 0UL;
			ulong num;
			if (flag)
			{
				num = feature & ~featureMask;
			}
			else
			{
				bool flag2 = (feature & featureMask) > 0UL;
				if (flag2)
				{
					num = feature;
				}
				else
				{
					feature = (num = feature | featureMask);
				}
			}
			return num;
		}

		public static ulong DisableFeature(ulong featureMask, ulong feature)
		{
			bool flag = (feature & featureMask) == 0UL;
			ulong num;
			if (flag)
			{
				num = feature;
			}
			else
			{
				num = feature & ~featureMask;
			}
			return num;
		}

		public static ulong ToggleFeature(ulong featureMask, ulong feature)
		{
			bool flag = (featureMask & 8858387942UL) > 0UL;
			ulong num;
			if (flag)
			{
				num = feature & ~featureMask;
			}
			else
			{
				bool flag2 = FeatureBitHelper.IsFeatureEnabled(featureMask, feature);
				ulong num2;
				if (flag2)
				{
					num2 = FeatureBitHelper.DisableFeature(featureMask, feature);
				}
				else
				{
					num2 = FeatureBitHelper.EnableFeature(featureMask, feature);
				}
				num = num2;
			}
			return num;
		}

		public static bool WasFeatureChanged(ulong featureMask, ulong newFeature, ulong originalFeature, out bool isEnabled)
		{
			bool flag = (featureMask & 8858387942UL) > 0UL;
			bool flag2;
			if (flag)
			{
				isEnabled = false;
				flag2 = false;
			}
			else
			{
				bool flag3 = FeatureBitHelper.IsFeatureEnabled(featureMask, originalFeature);
				isEnabled = FeatureBitHelper.IsFeatureEnabled(featureMask, newFeature);
				flag2 = flag3 != isEnabled;
			}
			return flag2;
		}

		private const ulong BLOATWARE_MASK = 8858387942UL;
	}
}


