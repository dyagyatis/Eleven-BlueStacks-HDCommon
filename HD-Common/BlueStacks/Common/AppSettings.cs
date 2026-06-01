using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace BlueStacks.Common
{
	[Serializable]
	public class AppSettings
	{
		[JsonProperty("IsKeymappingTooltipShown")]
		public bool IsKeymappingTooltipShown
		{
			get
			{
				return this.mIsKeymappingTooltipShown;
			}
			set
			{
				this.mIsKeymappingTooltipShown = value;
				AppConfigurationManager.Save();
			}
		}

		[JsonProperty("IsDefaultSchemeRecorded")]
		public bool IsDefaultSchemeRecorded
		{
			get
			{
				return this.mIsDefaultSchemeRecorded;
			}
			set
			{
				this.mIsDefaultSchemeRecorded = value;
				AppConfigurationManager.Save();
			}
		}

		[JsonProperty("IsAppOnboardingCompleted")]
		public bool IsAppOnboardingCompleted
		{
			get
			{
				return this.mIsAppOnboardingCompleted;
			}
			set
			{
				this.mIsAppOnboardingCompleted = value;
				AppConfigurationManager.Save();
			}
		}

		[JsonProperty("IsGeneralAppOnBoardingCompleted")]
		public bool IsGeneralAppOnBoardingCompleted
		{
			get
			{
				return this.mIsGeneralAppOnBoardingCompleted;
			}
			set
			{
				this.mIsGeneralAppOnBoardingCompleted = value;
				AppConfigurationManager.Save();
			}
		}

		[JsonProperty("IsCloseGuidanceOnboardingCompleted")]
		public bool IsCloseGuidanceOnboardingCompleted
		{
			get
			{
				return this.mIsCloseGuidanceOnboardingCompleted;
			}
			set
			{
				this.mIsCloseGuidanceOnboardingCompleted = value;
				AppConfigurationManager.Save();
			}
		}

		[JsonProperty("IsForcedLandscapeEnabled")]
		public bool IsForcedLandscapeEnabled
		{
			get
			{
				return this.mIsForcedLandscapeEnabled;
			}
			set
			{
				this.mIsForcedLandscapeEnabled = value;
				AppConfigurationManager.Save();
			}
		}

		[JsonProperty("IsForcedPortraitEnabled")]
		public bool IsForcedPortraitEnabled
		{
			get
			{
				return this.mIsForcedPortraitEnabled;
			}
			set
			{
				this.mIsForcedPortraitEnabled = value;
				AppConfigurationManager.Save();
			}
		}

		[JsonExtensionData]
		public IDictionary<string, object> ExtraData { get; set; }

		private bool mIsKeymappingTooltipShown;

		private bool mIsDefaultSchemeRecorded;

		private bool mIsAppOnboardingCompleted = true;

		private bool mIsGeneralAppOnBoardingCompleted = true;

		private bool mIsCloseGuidanceOnboardingCompleted = true;

		private bool mIsForcedLandscapeEnabled;

		private bool mIsForcedPortraitEnabled;
	}
}


