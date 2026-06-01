using System;
using System.Globalization;
using System.IO;

namespace BlueStacks.Common
{
	public static class Strings
	{
		public static string OEMTag
		{
			get
			{
				return Strings.GetOemTag();
			}
		}

		public static string GetOemTag()
		{
			bool flag = Strings.sOemTag == null;
			if (flag)
			{
				Strings.sOemTag = ("bgp64".Equals("bgp", StringComparison.InvariantCultureIgnoreCase) ? "" : "_bgp64");
			}
			return Strings.sOemTag;
		}

		public static string OEMTagWithDefaultOemHandling
		{
			get
			{
				return Strings.GetOemTagWithoutDefaultOemCheck();
			}
		}

		public static string GetOemTagWithoutDefaultOemCheck()
		{
			return "_bgp64";
		}

		public static string BlueStacksDriverDisplayName
		{
			get
			{
				return string.Format(CultureInfo.InvariantCulture, "BlueStacks Hypervisor{0}", new object[] { Strings.GetOemTag() });
			}
		}

		public static string BlueStacksDriverFileName
		{
			get
			{
				return string.Format(CultureInfo.InvariantCulture, "BstkDrv{0}.sys", new object[] { Strings.GetOemTagWithoutDefaultOemCheck() });
			}
		}

		public static string CurrentDefaultVmName
		{
			get
			{
				return Strings.mCurrentDefaultVmName;
			}
			set
			{
				Strings.mCurrentDefaultVmName = value;
				bool flag = string.IsNullOrEmpty(Strings.mCurrentDefaultVmName);
				if (flag)
				{
					Strings.mCurrentDefaultVmName = "Android";
				}
			}
		}

		public static string DefaultWindowTitle
		{
			get
			{
				return "App Player";
			}
		}

		public static string AppTitle
		{
			get
			{
				bool flag = Strings.s_AppTitle == null;
				string defaultWindowTitle;
				if (flag)
				{
					defaultWindowTitle = Strings.DefaultWindowTitle;
				}
				else
				{
					defaultWindowTitle = Strings.s_AppTitle;
				}
				return defaultWindowTitle;
			}
			set
			{
				Strings.s_AppTitle = value;
			}
		}

		public static string BlueStacksSetupFolder
		{
			get
			{
				bool flag = string.IsNullOrEmpty(Strings.sBlueStacksSetupFolder);
				if (flag)
				{
					Strings.sBlueStacksSetupFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "BlueStacksSetup");
				}
				return Strings.sBlueStacksSetupFolder;
			}
		}

		public static string AddOrdinal(int num)
		{
			bool flag = num < 0;
			string text;
			if (flag)
			{
				text = num.ToString(CultureInfo.InvariantCulture);
			}
			else
			{
				int num2 = num % 100;
				bool flag2 = num2 - 11 <= 2;
				if (flag2)
				{
					text = num.ToString() + "th";
				}
				else
				{
					string text2;
					switch (num % 10)
					{
					case 1:
						text2 = num.ToString() + "st";
						break;
					case 2:
						text2 = num.ToString() + "nd";
						break;
					case 3:
						text2 = num.ToString() + "rd";
						break;
					default:
						text2 = num.ToString() + "th";
						break;
					}
					text = text2;
				}
			}
			return text;
		}

		public static string GetPlayerLockName(string vmName, string oem = "bgp64")
		{
			return string.Format(CultureInfo.InvariantCulture, "Global\\BlueStacks_{0}{1}_Player_Lock", new object[] { vmName, oem });
		}

		public static string GetHDApkInstallerLockName(string vmName)
		{
			return string.Format(CultureInfo.InvariantCulture, "Global\\BlueStacks_HDApkInstaller_{0}{1}_Lock", new object[] { vmName, "bgp64" });
		}

		public static string GetHDXapkInstallerLockName(string vmName)
		{
			return string.Format(CultureInfo.InvariantCulture, "Global\\BlueStacks_HDXapkInstaller_{0}{1}_Lock", new object[] { vmName, "bgp64" });
		}

		public static string GetClientInstanceLockName(string vmName, string oem = "bgp64")
		{
			return string.Format(CultureInfo.InvariantCulture, "Global\\BlueStacks_Client_Instance_{0}{1}_Lock", new object[] { vmName, oem });
		}

		public static string GetBlueStacksUILockNameOem(string oem = "bgp64")
		{
			return "Global\\BlueStacks_BlueStacksUI_Lock" + oem;
		}

		public static string DisableHyperVSupportArticle
		{
			get
			{
				return "http://127.0.0.1";
			}
		}

		public static double? TitleBarProductIconWidth { get; set; } = null;

		public static double? TitleBarTextMaxWidth { get; set; } = null;

		public static string ProductDisplayName { get; set; } = "BlueStacks";

		public static string TitleBarIconImageName { get; set; } = "ProductLogo";

		public static string ProductTopBarDisplayName { get; set; } = Strings.ProductDisplayName;

		public static string UninstallerTitleName { get; set; } = "Bluestacks Uninstaller";

		public static string UninstallCancelBtnColor { get; set; } = "White";

		public static string MaterialDesignPrimaryBtnStyle { get; set; } = "MaterialDesignButton";

		public static int ForceDedicatedGPUDefaultValue { get; set; } = 1;

		public const string DefaultOEM = "bgp";

		public const string BGPOEM64 = "bgp64";

		private static string sOemTag = null;

		public const string DefaultVmName = "Android";

		private static string mCurrentDefaultVmName = "Android";

		private static string s_AppTitle = null;

		private static string sBlueStacksSetupFolder = "";

		public static readonly string BlueStacksOldDriverName = "BstkDrv" + Strings.OEMTag;

		public const string BlueStacksDriverNameWithoutOEM = "BlueStacksDrv";

		public static readonly string BlueStacksDriverName = "BlueStacksDrv" + Strings.OEMTag;

		public const string BlueStacksIdRegistryPath = "Software\\BlueStacksInstaller";

		public static readonly string ClientRegistry32BitPath = "Software\\BlueStacksGP" + Strings.OEMTag;

		public static readonly string ClientRegistry64BitPath = "Software\\WOW6432Node\\BlueStacksGP" + Strings.OEMTag;

		public const string RegistryBaseKeyPathWithoutOEM = "Software\\BlueStacks";

		public static readonly string RegistryBaseKeyPath = "Software\\BlueStacks" + Strings.OEMTag;

		public static readonly string UninstallRegistryExportedFilePath = Path.Combine(Path.GetTempPath(), "BSTUninstall.reg");

		public static readonly string BGPKeyName = "BlueStacksGP" + Strings.OEMTag;

		public const string BlueStacksUIClosingLockName = "Global\\BlueStacks_BlueStacksUI_Closing_Lockbgp64";

		public const string MultiInsLockName = "Global\\BlueStacks_MULTI_INS_Frontend_Lockbgp64";

		public const string LogCollectorLockName = "Global\\BlueStacks_Log_Collector_Lockbgp64";

		public const string HDAgentLockName = "Global\\BlueStacks_HDAgent_Lockbgp64";

		public const string HDQuitMultiInstallLockName = "Global\\BlueStacks_HDQuitMultiInstall_Lockbgp64";

		public const string ComRegistrarLockName = "Global\\BlueStacks_UnRegRegCom_Lockbgp64";

		public const string GetBlueStacksUILockName = "Global\\BlueStacks_BlueStacksUI_Lockbgp64";

		public const string DataManagerLock = "Global\\BlueStacks_Downloader_Lockbgp64";

		public const string UninstallerLock = "Global\\BlueStacks_Uninstaller_Lockbgp64";

		public const string MultiInstanceManagerLock = "Global\\BlueStacks_MultiInstanceManager_Lockbgp64";

		public const string InstallerLockName = "Global\\BlueStacks_Installer_Lockbgp64";

		public const string CloudWatcherLock = "Global\\BlueStacks_CloudWatcher_Lockbgp64";

		public const string DiskCompactorLock = "Global\\BlueStacks_DiskCompactor_Lockbgp64";

		public const string MicroInstallerLock = "Global\\BlueStacks_MicroInstaller_Lockbgp64";

		public const string GetPlayerClosingLockName = "Global\\BlueStacks_PlayerClosing_Lockbgp64";

		public const string InputMapperDLL = "HD-Imap-Native.dll";

		public const string MSIVibrationDLL = "MsiKBVibration.dll";

		public const string MSIVibration64DLL = "MsiKBVibration64.dll";

		public const string InstancePrefix = "Android_";

		public const string AppInstallFinished = "appinstallfinished";

		public const string AppInstallProgress = "appinstallprogress";

		public const string BluestacksMultiInstanceManager = "BlueStacks Multi-Instance Manager";

		public const string DiskCleanup = "Disk cleanup";

		public const string MultiInstanceManagerBinName = "HD-MultiInstanceManager.exe";

		public const string ProductIconName = "ProductLogo.ico";

		public const string ProductImageName = "ProductLogo.png";

		public const string GameIconFileName = "app_icon.ico";

		public const string ApkHandlerBaseKeyName = "BlueStacks.Apk";

		public const string ScriptFileWhichRemovesGamingEdition = "RemoveGamingFiles.bat";

		public const string UninstallCurrentVersionRegPath = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall";

		public const string UninstallCurrentVersionRegPath32Bit = "SOFTWARE\\WOW6432Node\\Microsoft\\Windows\\CurrentVersion\\Uninstall";

		public const string UninstallCurrentVersion32RegPath = "SOFTWARE\\Wow6432Node\\Microsoft\\Windows\\CurrentVersion\\Uninstall";

		public const string UninstallKey = "Software\\Microsoft\\Windows\\CurrentVersion\\Uninstall";

		public const string ShowEnableVtPopupUrl = "showenablevtpopup";

		public const string SharePicUrl = "sharepic";

		public const string ShowGuidanceUrl = "controller_guidance_pressed";

		public const string AgentCrashReportUrl = "http://127.0.0.1/stats/agentcrashreport";

		public const string AppClickStatsUrl = "http://127.0.0.1/stats/appclickstats";

		public const string WebAppChannelClickStatsUrl = "http://127.0.0.1/stats/webappchannelclickstats";

		public const string FrontendStatusUpdateUrl = "FrontendStatusUpdate";

		public const string SearchAppStatsUrl = "http://127.0.0.1/stats/searchappstats";

		public const string AppInstallStatsUrl = "http://127.0.0.1/stats/appinstallstats";

		public const string SystemInfoStatsUrl = "http://127.0.0.1/stats/systeminfostats";

		public const string BootStatsUrl = "http://127.0.0.1/stats/bootstats";

		public const string TimelineStatsUrl = "http://127.0.0.1/stats/timelinestats4";

		public const string HomeScreenStatsUrl = "http://127.0.0.1/stats/homescreenstats";

		public const string BsInstallStatsUrl = "http://127.0.0.1/stats/bsinstallstats";

		public const string MiscellaneousStatsUrl = "http://127.0.0.1/stats/miscellaneousstats";

		public const string KeyMappingUIStatsStatsUrl = "http://127.0.0.1/stats/keymappinguistats";

		public const string ClientStatsUrl = "http://127.0.0.1/bs4/stats/clientstats";

		public const string UploadtobigqueryUrl = "http://127.0.0.1/bigquery/uploadtobigquery";

		public const string BtvFunnelStatsUrl = "http://127.0.0.1/stats/btvfunnelstats";

		public const string TroubleshooterStatsUrl = "http://127.0.0.1/stats/troubleshooterlogs";

		public const string GetCACodeUrl = "http://127.0.0.1/api/getcacode";

		public const string GetCountryUrl = "http://127.0.0.1/api/getcountryforip";

		public const string UploadDebugLogsUrl = "http://127.0.0.1/uploaddebuglogs";

		public const string UploadDebugLogsApkInstallFailureUrl = "http://127.0.0.1/logs/appinstallfailurelog";

		public const string UploadDebugLogsBootFailureUrl = "http://127.0.0.1/logs/bootfailurelog";

		public const string UploadDebugLogsCrashUrl = "http://127.0.0.1/logs/crashlog";

		public const string UserDataDir = "UserData";

		public const string UsersPath = "UsersPath";

		public const string UserProfile = "userprofile";

		public const string IdSeparator = "##";

		public const string UserDefinedDir = "UserDefinedDir";

		public const string StoreAppsDir = "App Stores";

		public const string IconsDir = "Icons";

		public const string CloudHost2 = "http://127.0.0.1";

		public const string CloudHost = "http://127.0.0.1";

		public const string LibraryName = "Apps";

		public const string BstPrefix = "Bst-";

		public const string GameManagerBannerImageDir = "sendappdisplayed";

		public const string SharedFolderName = "BstSharedFolder";

		public const string SharedFolder = "SharedFolder";

		public const string Library = "Library";

		public const string InputMapperFolderName = "InputMapper";

		public const string CaCodeBackUpFileName = "Bst_CaCode_Backup";

		public const string PCodeBackUpFileName = "Bst_PCode_Backup";

		public const string CaSelectorBackUpFileName = "Bst_CaSelector_Backup";

		public const string UserInfoBackupFileName = "Bst_UserInfo_Backup";

		public const string BlueStacksPackagePrefix = "com.bluestacks";

		public const string LatinImeId = "com.android.inputmethod.latin/.LatinIME";

		public const string FrontendPortBootParam = "WINDOWSFRONTEND";

		public const string AgentPortBootParam = "WINDOWSAGENT";

		public const string VMXBitIsOn = "Cannot run guest while VMX is in use";

		public const string InvalidOpCode = "invalid_op";

		public const string KernelPanic = "Kernel panic";

		public const string Ext4Error = ".*EXT4-fs error \\(device sd[a-b]1\\): (mb_free_blocks|ext4_mb_generate_buddy|ext4_lookup|.*deleted inode referenced):";

		public const string PgaCtlInitFailedString = "BlueStacks.Frontend.Interop.Opengl.GetPgaServerInitStatus()";

		public const string AppCrashInfoFile = "App_C_Info.txt";

		public const string LogDirName = "Logs";

		public const string AppNotInstalledString = "package not installed";

		public const string NetEaseReportProblemUrl = "http://127.0.0.1";

		public const string NetEaseOpenBrowserString = "????";

		public const string OEMNameBluestacks = "bluestacks";

		public const string InstallDirKeyName = "InstallDir";

		public const string StyleThemeStatsTag = "StyleAndThemeData";

		public const string ConfigParam = "32";

		public const string OemPrimaryInfo = "e";

		public const string WorldWideToggleDisabledAppListUrl = "http://127.0.0.1";

		public const string TombStoneFilePrefix = "tombstone";

		public const string MultiInstanceStatsUrl = "http://127.0.0.1/stats/multiinstancestats";

		public const string VmManagerExe = "HD-VmManager.exe";

		public const string BluestacksServiceString = "BlueStacks Service";

		public const string ReportProblemWindowTitle = "BlueStacks Report Problem";

		public const string XapkHandlerBaseKeyName = "BlueStacks.Xapk";

		public const int AgentServerStartingPort = 2861;

		public const int HTTPServerPortRangeAgent = 10;

		public const int ClientServerStartingPort = 2871;

		public const int HTTPServerPortRangeClient = 10;

		public const int PlayerServerStartingPort = 2881;

		public const int VmMonitorServerStartingPort = 2921;

		public const int HTTPServerVmPortRange = 40;

		public const int MultiInstanceServerStartingPort = 2961;

		public const int MultiInstanceServerPortRange = 10;

		public const int DefaultBlueStacksSize = 2047;

		public const string ChannelsProdTwitchServerUrl = "http://127.0.0.1";

		public const string VideoTutorialUrl = "videoTutorial";

		public const string OnboardingUrl = "http://127.0.0.1/bs3/page/onboarding-tutorial";

		public const string ComExceptionErrorString = "com exception";

		public const string BootFailedTimeoutString = "boot timeout exception";

		public const long MinimumRAMRequiredForInstallationInGB = 1L;

		public const long MinimumSpaceRequiredFreshInstallationInGB = 5L;

		public const long MinimumSpaceRequiredFreshInstallationInInstallDirMB = 500L;

		public const long MinimumSpaceRequiredFreshInstallationInMB = 5120L;

		public const string UninstallerFileName = "BlueStacksUninstaller.exe";

		public const string ArchInstallerFile64 = "64bit";

		public const string TestRollbackRegistryKeyName = "TestRollback";

		public const string TestRollbackFailRegistryKeyName = "TestRollbackFail";

		public const string UnifiedInstallStats = "http://127.0.0.1/bs3/stats/unified_install_stats";

		public const string OEMConfigFileName = "Oem.cfg";

		public const string RunAppBinaryName = "HD-RunApp.exe";

		public const string BlueStacksBinaryName = "BlueStacks.exe";

		public const string BlueStacksSetupFolderName = "BlueStacksSetup";

		public const string GlModeString = "GlMode";

		public const string MinimumClientVersionForUpgrade = "3.52.66.1905";

		public const string MinimumEngineVersionForUpgrade = "2.52.66.8704";

		public const string DiskCompactionToolMinSupportVersion = "4.60.00.0000";

		public const string FirstIMapBuildVersion = "4.30.33.1590";

		public const string FirstUnifiedInstallerBuildVersion = "4.20.21";

		public const string CommonInstallUtilsZipFileName = "CommonInstallUtils.zip";

		public const string RollbackCompletedStatString = "ROLLBACK_COMPLETED";

		public const string HandleBinaryName = "HD-Handle.exe";

		public const string HandleRegistryKeyPath = "Software\\Sysinternals\\Handle";

		public const string HandleEULAKeyName = "EulaAccepted";

		public const string BootStrapperFileName = "Bootstrapper.exe";

		public const string TakeBackupURL = "Backup_and_Restore";

		public const string CloudImageTranslateUrl = "http://127.0.0.1/translate/postimage";

		public const string RemoteAccessRequestString = "May I please have remote access?";

		public const string UninstallRegistryFileName = "BSTUninstall.reg";

		public const string BuildVersionForUpgradeFromParserVersion13To14 = "4.140.00.0000";

		public const string MultiInstanceBuildUrl = "http://127.0.0.1/bs4/getmultiinstancebuild?";

		public const string MultiInstanceCheckUpgrade = "http://127.0.0.1/bs4/check_upgrade?";

		public const string RemoveDiskCommand = "removedisk";

		public const string AnnouncementActionPopupSimple = "simple_popup";

		public const string AnnouncementActionPopupRich = "rich_popup";

		public const string AnnouncementActionPopupCenter = "center_popup";

		public const string AnnouncementActionDownloadExecute = "download_execute";

		public const string AnnouncementActionSilentLogCollect = "silent_logcollect";

		public const string AnnouncementActionSilentExecute = "silent_execute";

		public const string AnnouncementActionSelfUpdate = "self_update";

		public const string AnnouncementActionWebURLGM = "web_url_gm";

		public const string AnnouncementActionWebURL = "web_url";

		public const string AnnouncementActionStartAndroidApp = "start_android_app";

		public const string AnnouncementActionSilentInstall = "silent_install";

		public const string AnnouncementActionDisablePermanently = "disable_permanently";

		public const string AnnouncementActionNoPopup = "no_popup";

		public const string CloudStuckAtBoot = "http://127.0.0.1";

		public const string CloudEnhancePerformance = "http://127.0.0.1";

		public const string CloudWhyGoogle = "http://127.0.0.1";

		public const string CloudTroubleSigningIn = "http://127.0.0.1";

		public const string ExitPopupTagBoot = "exit_popup_boot";

		public const string ExitPopupTagOTS = "exit_popup_ots";

		public const string ExitPopupTagNoApp = "exit_popup_no_app";

		public const string ExitPopupEventShown = "popup_shown";

		public const string ExitPopupEventClosedButton = "popup_closed";

		public const string ExitPopupEventClosedCross = "click_action_close";

		public const string ExitPopupEventReturnBlueStacks = "click_action_return_bluestacks";

		public const string ExitPopupEventClosedContinue = "click_action_continue_bluestacks";

		public const string ExitPopupEventAutoHidden = "popup_auto_hidden";

		public const string GL_MODE = "GlMode";

		public const string BootPromotionImageName = "BootPromo";

		public const string BackgroundPromotionImageName = "BackPromo";

		public const string AppSuggestionImageName = "AppSuggestion";

		public const string AppSuggestionRemovedFileName = "app_suggestion_removed";

		public const string ClientPromoDir = "Promo";

		public const string BGPDataDirFolderName = "Engine";

		public const string EngineActivityUri = "engine_activity";

		public const string EmulatorActivityUri = "emulator_activity";

		public const string AppInstallUri = "app_install";

		public const string DeviceProfileListUrl = "get_device_profile_list";

		public const string AppActivityUri = "app_activity";

		public const string HelpArticles = "help_articles";

		public const string EnableVirtualization = "enable_virtualization";

		public const string CloudWatcherFolderName = "Helper";

		public const string CloudWatcherBinaryName = "BlueStacksHelper.exe";

		public const string CloudWatcherTaskName = "BlueStacksHelper";

		public const string CloudWatcherIdleTaskName = "BlueStacksHelperTask";

		public const string DirectX = "DirectX";

		public const string OpenGL = "OpenGL";

		public const string LogCollectorZipFileName = "BlueStacks-Support.7z";

		public const string BstClient = "BstClient";

		public const string OperationSynchronization = "operation_synchronization";

		public const string SetIframeUrl = "setIframeUrl";

		public const string OnBrowserVisibilityChange = "onBrowserVisibilityChange";

		public const string CloudAnnouncementTimeFormat = "dd/MM/yyyy HH:mm:ss";

		public const string GadgetDir = "Gadget";

		public const string MachineID = "MachineID";

		public const string VersionMachineID = "VersionMachineId_4.220.0.4001";

		public const string UserScripts = "UserScripts";

		public const string PcodeString = "pcode";

		public const string ROOT_VDI_UUID = "fca296ce-8268-4ed7-a57f-d32ec11ab304";

		public const string LocaleFileNameFormat = "i18n.{0}.txt";

		public const string MicroInstallerWindowTitle = "BlueStacks Installer";

		public const string Bit64 = "x64 (64-bit)";

		public const string Bit32 = "x86 (32-bit)";

		public const int MaxRequiredAvailablePhysicalMemory = 2148;

		public const string MediaFilesPathSetStatsTag = "MediaFilesPathSet";

		public const string MediaFileSaveSuccess = "MediaFileSaveSuccess";

		public const string VideoRecording = "VideoRecording";

		public const string RestoreDefaultKeymappingStatsTag = "RestoreDefaultKeymappingClicked";

		public const string ImportKeymappingStatsTag = "ImportKeymappingClicked";

		public const string ExportKeymappingStatsTag = "ExportKeymappingClicked";

		public const string ForceGPUBinaryName = "HD-ForceGPU.exe";

		public const string DMMEnableVtUrl = "http://127.0.0.1";

		public const string ObsBinaryName = "HD-OBS.exe";

		public const string NCSoftSharedMemoryName = "ngpmmf";

		public const string SidebarElementsDefaultFile = "sidebar_config.json";

		public const string SidebarElementsFileName = "SidebarConfig_{0}.json";

		public const string AnotherInstanceRunningPromptText1ForDMM = "????????????????????????";

		public const string AnotherInstanceRunningPromptText2ForDMM = "???????????????????????";

		public const string GlTextureFolder = "UCT";

		public const string ServiceInstallerBinaryName = "HD-ServiceInstaller.exe";

		public const string MacroRecorder = "MacroOperations";

		public const string GLCheckBinaryName = "HD-GLCheck.exe";

		public const string BTVFolderName = "BTV";

		public const string KeyboardShortcuts = "KeyboardShortcuts";

		public const string ComRegistrationBinaryName = "HD-ComRegistrar.exe";

		public const string CurrentParserVersion = "17";

		public const string InternetConnectivityCheckUrl = "http://127.0.0.1";

		public const string AbiValueString = "abivalue";

		public const string MemAllocator = "MEMALLOCATOR";

		public const string MacroCommunityUrlKey = "macro-share";

		public const int DefaultMaxBatchInstanceCreationCount = 5;

		public const string MOBATag = "MOBA";

		public const string DefaultScheme = "DefaultScheme";

		public const string SchemeChanged = "SchemeChanged";

		public const string GameControlBlurbTitle = "GameControlBlurb";

		public const string GuidanceVideoBlurbTitle = "GuidanceVideoBlurb";

		public const string FullScreenBlurbTitle = "FullScreenBlurb";

		public const string ViewGuideControlBlurbTitle = "ViewControlBlurb";

		public const string SelectedGameSchemeBlurbTitle = "SelectedGameSchemeBlurb";

		public const string WinId = "winid";

		public const string SelectedSchemeName = "SelectedSchemeName";

		public const string DecreaseVolumeImageName = "decrease";

		public const string DecreaseVolumeDisableImageName = "decrease_disable";

		public const string IncreaseVolumeImageName = "increase";

		public const string IncreseVolumeDisableImageName = "increase_disable";

		public const string MuteVolumeImageName = "volume_switch_off";

		public const string UnmuteVolumeImageName = "volume_switch_on";

		public const string Custom = "Custom";

		public const string ProductMajorVersion = " 4";

		public static class VersionConstants
		{
			public const string SmartControlsCOD = "4.140.10.1000";

			public const string GameControlNewNotificationVersion = "4.150.0.1000";

			public const string NotificationModeVersion = "4.210.0.1000";
		}

		public static class EngineSettingsConfiguration
		{
			public const double HighendMachineRAMThreshold = 7782.4;

			public const int HighendHighRAM = 4096;

			public const int HighendHighRAMInGB = 4;

			public const int HighCPU = 4;

			public const int HighRAM = 3072;

			public const int HighRAMInGB = 3;

			public const int MediumCPU = 2;

			public const int MediumRAM = 2048;

			public const int MediumRAMInGB = 2;

			public const int LowCPU = 1;

			public const int LowRAM = 1024;

			public const int LowRAMInGB = 1;
		}
	}
}


