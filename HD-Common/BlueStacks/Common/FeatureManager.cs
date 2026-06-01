using System;

namespace BlueStacks.Common
{
	public class FeatureManager
	{
		public static FeatureManager Instance
		{
			get
			{
				bool flag = FeatureManager.sInstance == null;
				if (flag)
				{
					object obj = FeatureManager.syncRoot;
					lock (obj)
					{
						bool flag3 = FeatureManager.sInstance == null;
						if (flag3)
						{
							FeatureManager.sInstance = new FeatureManager();
						}
					}
				}
				return FeatureManager.sInstance;
			}
		}

		public bool IsBTVEnabled
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		public bool IsWallpaperChangeDisabled { get; set; }

		public bool IsCreateBrowserOnStart { get; set; }

		public bool IsOpenActivityFromAccountIcon { get; set; }

		public bool IsBrowserKilledOnTabSwitch { get; set; }

		public bool IsPromotionDisabled
		{
			get
			{
				return true;
			}
			set
			{
			}
		}

		public bool IsGuidBackUpEnable
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		public bool IsCustomUIForDMMSandbox { get; set; }

		public bool IsCustomUIForDMM { get; set; }

		public bool IsThemeEnabled
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		public bool IsSearchBarVisible { get; set; }

		public bool IsCustomResolutionInputAllowed
		{
			get
			{
				return true;
			}
			set
			{
			}
		}

		public bool ShowBeginnersGuidePreference { get; set; }

		public bool IsShowNotificationCentre
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		public bool IsUseWpfTextbox { get; set; }

		public bool IsComboKeysDisabled { get; set; }

		public bool IsMacroRecorderEnabled { get; set; }

		public bool IsFarmingModeDisabled
		{
			get
			{
				return true;
			}
			set
			{
			}
		}

		public bool IsOperationsSyncEnabled
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		public bool IsRotateScreenDisabled { get; set; }

		public bool IsUserAccountBtnEnabled
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		public bool IsWarningBtnEnabled { get; set; }

		public bool IsAppCenterTabVisible
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		public bool IsMultiInstanceControlsGridVisible { get; set; }

		public bool IsPromotionFixed
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		public bool IsShowLanguagePreference { get; set; }

		public bool IsShowDesktopShortcutPreference { get; set; }

		public bool IsShowGamingSummaryPreference
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		public bool IsShowSpeedUpTips
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		public bool IsShowPostOTSScreen { get; set; }

		public bool IsShowHelpCenter
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		public bool IsAppSettingsAvailable
		{
			get
			{
				return true;
			}
			set
			{
			}
		}

		public bool IsShowPerformancePreference { get; set; }

		public bool IsShowDiscordPreference
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		public bool IsCustomUIForNCSoft { get; set; }

        public bool AllowADBSettingToggle { get; set; } = false;

        public bool ShowClientOnTopPreference { get; set; }

		public bool IsAllowGameRecording { get; set; }

		public bool IsShowAppRecommendations
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		public bool IsCheckForQuitPopup { get; set; }

		public bool IsCustomCursorEnabled
		{
			get
			{
				return true;
			}
			set
			{
			}
		}

		public bool ForceEnableMacroAndSync { get; set; }

        public bool IsHtmlSideBar { get; set; }

        public bool IsTimelineStatsEnabled
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		public bool IsShowAdvanceExitOption
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		public bool IsShowAndroidInputDebugSetting { get; set; }

		public bool IsTopbarHelpEnabled { get; set; }

        public bool IsHtmlHome { get; set; }

        public FeatureManager()
		{
			this.IsPromotionDisabled = true;
			this.IsWallpaperChangeDisabled = false;
			this.IsCreateBrowserOnStart = false;
			this.IsOpenActivityFromAccountIcon = false;
			this.IsBrowserKilledOnTabSwitch = true;
			this.IsCustomUIForDMMSandbox = false;
			this.IsCustomUIForDMM = false;
			this.IsSearchBarVisible = false;
			this.ShowBeginnersGuidePreference = false;
			this.IsUseWpfTextbox = false;
			this.IsComboKeysDisabled = false;
			this.IsMacroRecorderEnabled = false;
			this.IsRotateScreenDisabled = false;
			this.IsUserAccountBtnEnabled = false;
			this.IsWarningBtnEnabled = false;
			this.IsMultiInstanceControlsGridVisible = false;
			this.IsShowLanguagePreference = true;
			this.IsShowDesktopShortcutPreference = false;
			this.IsShowPostOTSScreen = false;
			this.IsShowPerformancePreference = false;
			this.IsCustomUIForNCSoft = false;
			this.ShowClientOnTopPreference = true;
			this.IsAllowGameRecording = false;
			this.IsCheckForQuitPopup = false;
			this.ForceEnableMacroAndSync = false;
			this.IsShowAndroidInputDebugSetting = false;
			this.IsTopbarHelpEnabled = false;
		}

		private static string sFilePath = string.Empty;

		private static volatile FeatureManager sInstance;

		private static object syncRoot = new object();
	}
}


