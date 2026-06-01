using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Win32;

namespace BlueStacks.Common
{
	public sealed class RegistryManager
	{
		public static string UPGRADE_TAG
		{
			get
			{
				return RegistryManager.mUPGRADE_TAG;
			}
			set
			{
				RegistryManager.ClearRegistryMangerInstance();
				RegistryManager.mUPGRADE_TAG = value;
			}
		}

		public static RegistryManager Instance
		{
			get
			{
				if (RegistryManager.sInstance == null)
				{
					object obj = RegistryManager.sLock;
					lock (obj)
					{
						if (RegistryManager.sInstance == null)
						{
							RegistryManager registryManager = new RegistryManager();
							registryManager.mIsAdmin = SystemUtils.IsAdministrator();
							registryManager.Init("bgp64");
							RegistryManager.sInstance = registryManager;
							if (RegistryManager.RegistryManagers.ContainsKey("bgp64"))
							{
								RegistryManager.RegistryManagers["bgp64"] = RegistryManager.sInstance;
							}
						}
					}
				}
				return RegistryManager.sInstance;
			}
			set
			{
				RegistryManager.sInstance = value;
			}
		}

		public static void SetRegistryManagers(List<string> oems)
		{
			if (oems == null || oems.Count == 0)
			{
				oems = new List<string> { "bgp64" };
			}
			object obj = RegistryManager.sLock;
			lock (obj)
			{
				Dictionary<string, RegistryManager> dictionary = new Dictionary<string, RegistryManager>();
				foreach (string text in oems)
				{
					RegistryManager registryManager = new RegistryManager
					{
						mIsAdmin = SystemUtils.IsAdministrator()
					};
					registryManager.Init(text);
					dictionary.Add(text, registryManager);
				}
				RegistryManager.RegistryManagers = dictionary;
			}
		}

		public static bool CheckOemInRegistry(string oemToCheck, string vmId)
		{
			bool flag = false;
			if (!string.IsNullOrEmpty(oemToCheck))
			{
				string text = (oemToCheck.Equals("bgp", StringComparison.InvariantCultureIgnoreCase) ? "" : ("_" + oemToCheck));
				string text2 = "Software\\BlueStacks" + text + RegistryManager.UPGRADE_TAG;
				RegistryKey registryKey;
				if (!string.IsNullOrEmpty(vmId))
				{
					string text3 = text2 + "\\Guests\\" + vmId;
					registryKey = Registry.LocalMachine.OpenSubKey(text3);
				}
				else
				{
					registryKey = Registry.LocalMachine.OpenSubKey(text2);
				}
				flag = registryKey != null;
				if (flag)
				{
					registryKey.Close();
				}
			}
			return flag;
		}

		public static Dictionary<string, RegistryManager> RegistryManagers
		{
			get
			{
				if (RegistryManager._RegistryManagers == null)
				{
					RegistryManager._RegistryManagers = new Dictionary<string, RegistryManager> { 
					{
						"bgp64",
						RegistryManager.Instance
					} };
				}
				return RegistryManager._RegistryManagers;
			}
			set
			{
				RegistryManager._RegistryManagers = value;
			}
		}

		public Dictionary<string, InstanceRegistry> Guest { get; } = new Dictionary<string, InstanceRegistry>();

		public InstanceRegistry DefaultGuest
		{
			get
			{
				return this.Guest[Strings.CurrentDefaultVmName];
			}
		}

		public string BaseKeyPath { get; private set; } = "";

		public string ClientBaseKeyPath { get; private set; } = "";

		public string BTVKeyPath { get; private set; } = "";

		public string HostConfigKeyPath { get; private set; } = "";

		private RegistryManager()
		{
		}

		private RegistryKey InitKeyWithSecurityCheck(string keyName)
		{
			if (!this.mIsAdmin)
			{
				return Registry.LocalMachine.OpenSubKey(keyName);
			}
			return Registry.LocalMachine.CreateSubKey(keyName);
		}

		public static void ClearRegistryMangerInstance()
		{
			RegistryManager.sInstance = null;
		}

		private void Init(string oem = "bgp64")
		{
			string text = (oem.Equals("bgp", StringComparison.InvariantCultureIgnoreCase) ? "" : ("_" + oem));
			this.BaseKeyPath = "Software\\BlueStacks" + text + RegistryManager.UPGRADE_TAG;
			this.HostConfigKeyPath = this.BaseKeyPath + "\\Config";
			this.ClientBaseKeyPath = this.BaseKeyPath + "\\Client";
			this.BTVKeyPath = this.BaseKeyPath + "\\BTV";
			this.mBaseKey = this.InitKeyWithSecurityCheck(this.BaseKeyPath);
			this.mBTVKey = RegistryUtils.InitKey(this.BaseKeyPath + "\\BTV");
			this.mBTVFilterKey = RegistryUtils.InitKey(this.BaseKeyPath + "\\BTV\\Filters");
			this.mClientKey = RegistryUtils.InitKey(this.BaseKeyPath + "\\Client");
			this.mUserKey = RegistryUtils.InitKey(this.BaseKeyPath + "\\User");
			this.mHostConfigKey = RegistryUtils.InitKey(this.BaseKeyPath + "\\Config");
			this.mGuestsKey = RegistryUtils.InitKey(this.BaseKeyPath + "\\Guests");
			this.mMonitorsKey = RegistryUtils.InitKey(this.BaseKeyPath + "\\Monitors");
			if (this.mClientKey == null)
			{
				if (SystemUtils.IsOs64Bit())
				{
					this.mClientKey = RegistryUtils.InitKey("Software\\Wow6432Node\\BlueStacksGP");
				}
				else
				{
					this.mClientKey = RegistryUtils.InitKey("Software\\BlueStacksGP");
				}
			}
			foreach (string text2 in this.VmList)
			{
				this.Guest[text2] = new InstanceRegistry(text2, oem);
			}
		}

		public static void InitVmKeysForInstaller(List<string> vmList)
		{
			if (RegistryManager.sInstance == null)
			{
				RegistryManager.sInstance = new RegistryManager
				{
					mIsAdmin = SystemUtils.IsAdministrator()
				};
				RegistryManager.sInstance.Init("bgp64");
				if (RegistryManager.RegistryManagers.ContainsKey("bgp64"))
				{
					RegistryManager.RegistryManagers["bgp64"] = RegistryManager.sInstance;
				}
			}
			if (vmList != null)
			{
				foreach (string text in vmList)
				{
					if (!RegistryManager.sInstance.Guest.ContainsKey(text))
					{
						RegistryManager.sInstance.Guest[text] = new InstanceRegistry(text, "bgp64");
					}
				}
			}
		}

		public void SetAccessPermissions()
		{
			RegistryUtils.GrantAllAccessPermission(this.mHostConfigKey);
			RegistryUtils.GrantAllAccessPermission(this.mUserKey);
			RegistryUtils.GrantAllAccessPermission(this.mBTVKey);
			RegistryUtils.GrantAllAccessPermission(this.mClientKey);
			RegistryUtils.GrantAllAccessPermission(this.mGuestsKey);
			RegistryUtils.GrantAllAccessPermission(this.mMonitorsKey);
		}

		public bool DeleteAndroidSubKey(string vmName)
		{
			try
			{
				string text = string.Format(CultureInfo.InvariantCulture, "{0}\\Guests\\{1}", new object[] { this.BaseKeyPath, vmName });
				Registry.LocalMachine.DeleteSubKeyTree(text);
				this.Guest.Remove(vmName);
			}
			catch
			{
				return false;
			}
			return true;
		}

		[SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "<Pending>")]
		public string[] VmList
		{
			get
			{
				return (string[])this.mHostConfigKey.GetValue("VmList", new string[] { "Android" });
			}
			set
			{
				this.mHostConfigKey.SetValue("VmList", value, RegistryValueKind.MultiString);
				this.mHostConfigKey.Flush();
			}
		}

		[SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "<Pending>")]
		public string[] UpgradeVersionList
		{
			get
			{
				return (string[])this.mHostConfigKey.GetValue("UpgradeVersionList", new string[0]);
			}
			set
			{
				this.mHostConfigKey.SetValue("UpgradeVersionList", value, RegistryValueKind.MultiString);
				this.mHostConfigKey.Flush();
			}
		}

		public bool IsShootingModeTooltipVisible
		{
			get
			{
				return (int)this.mHostConfigKey.GetValue("IsShootingModeTooltipVisible", 1) != 0;
			}
			set
			{
				this.mHostConfigKey.SetValue("IsShootingModeTooltipVisible", (!value) ? 0 : 1);
				this.mHostConfigKey.Flush();
			}
		}

		public bool KeyMappingAvailablePromptEnabled
		{
			get
			{
				return (int)this.mClientKey.GetValue("KeyMappingAvailablePromptEnabled", 1) != 0;
			}
			set
			{
				this.mClientKey.SetValue("KeyMappingAvailablePromptEnabled", (!value) ? 0 : 1);
				this.mClientKey.Flush();
			}
		}

		public bool ForceDedicatedGPU
		{
			get
			{
				return (int)this.mHostConfigKey.GetValue("ForceDedicatedGPU", Strings.ForceDedicatedGPUDefaultValue) != 0;
			}
			set
			{
				this.mHostConfigKey.SetValue("ForceDedicatedGPU", (!value) ? 0 : 1);
				this.mHostConfigKey.Flush();
			}
		}

		public string AvailableGPUDetails
		{
			get
			{
				return (string)this.mHostConfigKey.GetValue("AvailableGPUDetails", "");
			}
			set
			{
				this.mHostConfigKey.SetValue("AvailableGPUDetails", value);
				this.mHostConfigKey.Flush();
			}
		}

		public bool OverlayAvailablePromptEnabled
		{
			get
			{
				return (int)this.mClientKey.GetValue("OverlayAvailablePromptEnabled", 0) != 0;
			}
			set
			{
				this.mClientKey.SetValue("OverlayAvailablePromptEnabled", (!value) ? 0 : 1);
				this.mClientKey.Flush();
			}
		}

		public bool DisableImageDetection
		{
			get
			{
				return (int)this.mClientKey.GetValue("DisableImageDetection", 0) != 0;
			}
			set
			{
				this.mClientKey.SetValue("DisableImageDetection", (!value) ? 0 : 1);
				this.mClientKey.Flush();
			}
		}

		public bool ShowKeyControlsOverlay
		{
			get
			{
				return (int)this.mClientKey.GetValue("ShowKeyControlsOverlay", 0) != 0;
			}
			set
			{
				this.mClientKey.SetValue("ShowKeyControlsOverlay", (!value) ? 0 : 1);
				this.mClientKey.Flush();
			}
		}

		public bool TranslucentControlsEnabled
		{
			get
			{
				return (int)this.mClientKey.GetValue("TranslucentControlsEnabled", 0) != 0;
			}
			set
			{
				this.mClientKey.SetValue("TranslucentControlsEnabled", (!value) ? 0 : 1);
				this.mClientKey.Flush();
			}
		}

		public double TranslucentControlsTransparency
		{
			get
			{
				return double.Parse((string)this.mClientKey.GetValue("TranslucentControlsTransparency", 0.8.ToString(CultureInfo.InvariantCulture)), CultureInfo.InvariantCulture);
			}
			set
			{
				this.mClientKey.SetValue("TranslucentControlsTransparency", value.ToString(CultureInfo.InvariantCulture));
				this.mClientKey.Flush();
			}
		}

		public bool ShowGamingSummary
		{
			get
			{
				return (int)this.mClientKey.GetValue("ShowGamingSummary", 1) == 1;
			}
			set
			{
				this.mClientKey.SetValue("ShowGamingSummary", value ? 1 : 0);
				this.mClientKey.Flush();
			}
		}

		public bool DiscordEnabled
		{
			get
			{
				return (int)this.mClientKey.GetValue("DiscordEnabled", 1) == 1;
			}
			set
			{
				this.mClientKey.SetValue("DiscordEnabled", value ? 1 : 0);
				this.mClientKey.Flush();
			}
		}

		public bool CustomCursorEnabled
		{
			get
			{
				return (int)this.mClientKey.GetValue("CustomCursorEnabled", 1) == 1;
			}
			set
			{
				this.mClientKey.SetValue("CustomCursorEnabled", value ? 1 : 0);
				this.mClientKey.Flush();
			}
		}

		public bool GamepadDetectionEnabled
		{
			get
			{
				return (int)this.mClientKey.GetValue("GamepadDetectionEnabled", 1) == 1;
			}
			set
			{
				this.mClientKey.SetValue("GamepadDetectionEnabled", value ? 1 : 0);
				this.mClientKey.Flush();
			}
		}

		public List<string> IgnoreAutoPlayPackageList
		{
			get
			{
				return ((string[])this.mClientKey.GetValue("ShownVideoOnFirstLaunchPackageList", new string[0])).ToList<string>();
			}
			set
			{
				this.mClientKey.SetValue("ShownVideoOnFirstLaunchPackageList", (value != null) ? value.ToArray() : null, RegistryValueKind.MultiString);
				this.mClientKey.Flush();
			}
		}

		public bool UpdateBstConfig
		{
			get { return false; }
			set { }
		}

		public bool IsGameTvEnabled
		{
			get
			{
				return (int)this.mClientKey.GetValue("IsGameTvEnabled", 0) != 0;
			}
			set
			{
				this.mClientKey.SetValue("IsGameTvEnabled", (!value) ? 0 : 1);
				this.mClientKey.Flush();
			}
		}

		public int CommonFPS
		{
			get
			{
				return (int)this.mHostConfigKey.GetValue("CommonFPS", 60);
			}
			set
			{
				this.mHostConfigKey.SetValue("CommonFPS", value);
				this.mHostConfigKey.Flush();
			}
		}

		public int TrimMemoryDuration
		{
			get
			{
				return (int)this.mHostConfigKey.GetValue("TrimMemoryDuration", 15);
			}
			set
			{
				this.mHostConfigKey.SetValue("TrimMemoryDuration", value);
				this.mHostConfigKey.Flush();
			}
		}

		public int DevEnv
		{
			get
			{
				return (int)this.mHostConfigKey.GetValue("DevEnv", 0);
			}
		}

		public int ArrangeWindowMode
		{
			get
			{
				return (int)this.mHostConfigKey.GetValue("ArrangeWindowModeConfig", 0);
			}
			set
			{
				this.mHostConfigKey.SetValue("ArrangeWindowModeConfig", value);
				this.mHostConfigKey.Flush();
			}
		}

		public long TileWindowColumnCount
		{
			get
			{
				return long.Parse(this.mHostConfigKey.GetValue("TileWindowColumnCount", 2).ToString(), CultureInfo.InvariantCulture);
			}
			set
			{
				this.mHostConfigKey.SetValue("TileWindowColumnCount", value);
				this.mHostConfigKey.Flush();
			}
		}

		public bool ManageGooglePlayPromptEnabled
		{
			get
			{
				return (int)this.mClientKey.GetValue("ManageGooglePlayPromptEnabled", 1) != 0;
			}
			set
			{
				this.mClientKey.SetValue("ManageGooglePlayPromptEnabled", (!value) ? 0 : 1);
				this.mClientKey.Flush();
			}
		}

		public bool UseEscapeToExitFullScreen
		{
			get
			{
				return (int)this.mClientKey.GetValue("UseEscapeToExitFullScreen", 0) == 1;
			}
			set
			{
				this.mClientKey.SetValue("UseEscapeToExitFullScreen", value ? 1 : 0);
				this.mClientKey.Flush();
			}
		}

		public bool IsVTXPopupEnable
		{
			get
			{
				return (int)this.mClientKey.GetValue("IsVTXPopupEnable", 1) == 1;
			}
			set
			{
				this.mClientKey.SetValue("IsVTXPopupEnable", value ? 1 : 0);
				this.mClientKey.Flush();
			}
		}

		public int FrontendHeight
		{
			get
			{
				return (int)this.mClientKey.GetValue("FrontendHeight", 0);
			}
			set
			{
				this.mClientKey.SetValue("FrontendHeight", value);
				this.mClientKey.Flush();
			}
		}

		public int FrontendWidth
		{
			get
			{
				return (int)this.mClientKey.GetValue("FrontendWidth", 0);
			}
			set
			{
				this.mClientKey.SetValue("FrontendWidth", value);
				this.mClientKey.Flush();
			}
		}

		public string BossKey
		{
			get
			{
				return (string)this.mClientKey.GetValue("BossKey", "");
			}
			set
			{
				this.mClientKey.SetValue("BossKey", value);
			}
		}

		public string CampaignMD5
		{
			get
			{
				return (string)this.mClientKey.GetValue("CampaignMD5", "");
			}
			set
			{
				this.mClientKey.SetValue("CampaignMD5", value);
				this.mClientKey.SetValue("FLECampaignMD5", value);
				this.mClientKey.Flush();
			}
		}

		public string FLECampaignMD5
		{
			get
			{
				return (string)this.mClientKey.GetValue("FLECampaignMD5", string.Empty);
			}
		}

		public string CampaignJson
		{
			get
			{
				return (string)this.mClientKey.GetValue("CampaignJson", "");
			}
			set
			{
				this.mClientKey.SetValue("CampaignJson", value);
				if (!string.IsNullOrEmpty(value))
				{
					this.DeleteFLECampaignMD5();
				}
				this.mClientKey.Flush();
			}
		}

		public void DeleteFLECampaignMD5()
		{
			if (this.mClientKey.GetValue("FLECampaignMD5", null) != null)
			{
				this.mClientKey.DeleteValue("FLECampaignMD5");
			}
		}

		public string CDNAppsTimeStamp
		{
			get
			{
				return (string)this.mClientKey.GetValue("CDNAppsTimeStamp", "");
			}
			set
			{
				this.mClientKey.SetValue("CDNAppsTimeStamp", value);
				this.mClientKey.Flush();
			}
		}

		public string SetupFolder
		{
			get
			{
				return (string)this.mClientKey.GetValue("SetupFolder", Strings.BlueStacksSetupFolder);
			}
			set
			{
				this.mClientKey.SetValue("SetupFolder", value);
			}
		}

		public string EngineDataDir
		{
			get
			{
				return (string)this.mClientKey.GetValue("EngineDataDir", "");
			}
			set
			{
				this.mClientKey.SetValue("EngineDataDir", value);
			}
		}

		public string ClientInstallDir
		{
			get
			{
				return (string)this.mClientKey.GetValue("ClientInstallDir", "");
			}
			set
			{
				this.mClientKey.SetValue("ClientInstallDir", value);
			}
		}

		public static string ClientThemeName { get; set; } = "Assets";

		public bool OpenThemeEditor
		{
			get
			{
				return (int)this.mClientKey.GetValue("OpenThemeEditor", 0) == 1;
			}
			set
			{
				this.mClientKey.SetValue("OpenThemeEditor", value ? 1 : 0);
				this.mClientKey.Flush();
			}
		}

		public string CefDataPath
		{
			get
			{
				return (string)this.mClientKey.GetValue("CefDataPath", string.Empty);
			}
			set
			{
				this.mClientKey.SetValue("CefDataPath", value);
				this.mClientKey.Flush();
			}
		}

		public string OfflineHtmlHomeUrl
		{
			get
			{
				string text = string.Empty;
				if (File.Exists(Path.Combine(this.ClientInstallDir, "OfflineHtmlHome\\offline.html")))
				{
					text = Path.Combine(this.ClientInstallDir, "OfflineHtmlHome\\offline.html");
				}
				if (string.IsNullOrEmpty((string)this.mClientKey.GetValue("OfflineHtmlHomeUrl", string.Empty)))
				{
					return text;
				}
				return (string)this.mClientKey.GetValue("OfflineHtmlHomeUrl", string.Empty);
			}
			set
			{
				this.mClientKey.SetValue("OfflineHtmlHomeUrl", value);
				this.mClientKey.Flush();
			}
		}

		public int CefDevEnv
		{
			get
			{
				return (int)this.mClientKey.GetValue("CefDevEnv", 0);
			}
		}

		public int CefDebugPort
		{
			get
			{
				return (int)this.mClientKey.GetValue("CefDebugPort", 0);
			}
		}

		public int LastBootTime
		{
			get
			{
				return (int)this.mClientKey.GetValue("LastBootTime", 120000);
			}
			set
			{
				this.mClientKey.SetValue("LastBootTime", value);
			}
		}

		public int NoOfBootCompleted
		{
			get
			{
				return (int)this.mClientKey.GetValue("NoOfBootCompleted", 0);
			}
			set
			{
				this.mClientKey.SetValue("NoOfBootCompleted", value);
				this.mClientKey.Flush();
			}
		}

		public bool IsScreenshotsLocationPopupEnabled
		{
			get
			{
				return (int)this.mClientKey.GetValue("ScreenshotsLocationPopupEnabled", 1) == 1;
			}
			set
			{
				this.mClientKey.SetValue("ScreenshotsLocationPopupEnabled", (!value) ? 0 : 1);
			}
		}

		public bool IsSynchronizerUsedStatSent
		{
			get
			{
				return (int)this.mClientKey.GetValue("IsSynchronizerUsedStatSent", 0) == 1;
			}
			set
			{
				this.mClientKey.SetValue("IsSynchronizerUsedStatSent", (!value) ? 0 : 1);
			}
		}

		public string ScreenShotsPath
		{
			get
			{
				return (string)this.mClientKey.GetValue("ScreenShotsPath", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), Strings.ProductTopBarDisplayName));
			}
			set
			{
				this.mClientKey.SetValue("ScreenShotsPath", value);
				this.mClientKey.Flush();
			}
		}

		public bool RequirementConfigUpdateRequired
		{
			get
			{
				return (int)this.mClientKey.GetValue("RequirementConfigUpdateRequired", 0) == 1;
			}
			set
			{
				this.mClientKey.SetValue("RequirementConfigUpdateRequired", (!value) ? 0 : 1);
				this.mClientKey.Flush();
			}
		}

		public bool IsShowIconBorder
		{
			get
			{
				return (int)this.mClientKey.GetValue("ShowIconBorder", 0) == 1;
			}
		}

		public string UserSelectedLocale
		{
			get
			{
				return (string)this.mClientKey.GetValue("UserSelectedLocale", "");
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					this.mClientKey.SetValue("UserSelectedLocale", value);
					this.mClientKey.Flush();
				}
			}
		}

		public string TargetLocale
		{
			get
			{
				return (string)this.mClientKey.GetValue("TargetLocale", "");
			}
		}

		public string TargetLocaleUrl
		{
			get
			{
				return (string)this.mClientKey.GetValue("TargetLocaleUrl", "");
			}
		}

		public string FailedUpgradeVersion
		{
			get
			{
				return (string)this.mClientKey.GetValue("FailedUpgradeVersion", "");
			}
			set
			{
				this.mClientKey.SetValue("FailedUpgradeVersion", value);
				this.mClientKey.Flush();
			}
		}

		public string LastUpdateSkippedVersion
		{
			get
			{
				return (string)this.mClientKey.GetValue("LastUpdateSkippedVersion", "");
			}
			set
			{
				this.mClientKey.SetValue("LastUpdateSkippedVersion", value);
				this.mClientKey.Flush();
			}
		}

		public string Partner
		{
			get
			{
				return (string)this.mClientKey.GetValue("Partner", "");
			}
			set
			{
				this.mClientKey.SetValue("Partner", value);
				this.mClientKey.Flush();
			}
		}

		public string DownloadedUpdateFile
		{
			get
			{
				return (string)this.mClientKey.GetValue("DownloadedUpdateFile", "");
			}
			set
			{
				this.mClientKey.SetValue("DownloadedUpdateFile", value);
				this.mClientKey.Flush();
			}
		}

		public string ClientVersion
		{
			get
			{
				string text = (string)this.mBaseKey.GetValue("ClientVersion", "");
				if (string.IsNullOrEmpty(text))
				{
					text = (string)this.mClientKey.GetValue("ClientVersion", "");
				}
				return text;
			}
			set
			{
				this.mBaseKey.SetValue("ClientVersion", value);
				this.mBaseKey.Flush();
			}
		}

		public int IsClientFirstLaunch
		{
			get
			{
				return (int)this.mClientKey.GetValue("IsClientFirstLaunch", 1);
			}
			set
			{
				this.mClientKey.SetValue("IsClientFirstLaunch", value);
				this.mClientKey.Flush();
			}
		}

		public int IsEngineUpgraded
		{
			get
			{
				return (int)this.mClientKey.GetValue("IsEngineUpgraded", 0);
			}
			set
			{
				this.mClientKey.SetValue("IsEngineUpgraded", value);
				this.mClientKey.Flush();
			}
		}

		public bool IsShowRibbonNotification
		{
			get
			{
				return (int)this.mClientKey.GetValue("IsShowRibbonNotification", 1) == 1;
			}
			set
			{
				this.mClientKey.SetValue("IsShowRibbonNotification", value ? 1 : 0);
				this.mClientKey.Flush();
			}
		}

		public bool IsShowToastNotification
		{
			get
			{
				return (int)this.mClientKey.GetValue("IsShowToastNotification", 1) == 1;
			}
			set
			{
				this.mClientKey.SetValue("IsShowToastNotification", value ? 1 : 0);
				this.mClientKey.Flush();
			}
		}

		public bool IsShowPromotionalTeaser
		{
			get
			{
				return false;
			}
			set
			{
				this.mClientKey.SetValue("IsShowPromotionalTeaser", value ? 1 : 0);
				this.mClientKey.Flush();
			}
		}

		public bool IsClientUpgraded
		{
			get
			{
				return (int)this.mClientKey.GetValue("IsClientUpgraded", 0) == 1;
			}
			set
			{
				this.mClientKey.SetValue("IsClientUpgraded", value ? 1 : 0);
				this.mClientKey.Flush();
			}
		}

		public string AInfo
		{
			get
			{
				return (string)this.mClientKey.GetValue("AInfo", "");
			}
			set
			{
				this.mClientKey.SetValue("AInfo", value);
				this.mClientKey.Flush();
			}
		}

		public string BGPDevUrl
		{
			get
			{
				return (string)this.mClientKey.GetValue("BGPDevUrl", "");
			}
			set
			{
				this.mClientKey.SetValue("BGPDevUrl", value);
				this.mClientKey.Flush();
			}
		}

		public string FriendsDevServer
		{
			get
			{
				return (string)this.mClientKey.GetValue("FriendsDevServer", "");
			}
		}

		public string PromotionId
		{
			get
			{
				return "";
			}
			set
			{
				this.mClientKey.SetValue("PromotionId", value);
				this.mClientKey.Flush();
			}
		}

		public string DMMRecommendedWindowUrl
		{
			get
			{
				return (string)this.mClientKey.GetValue("RecommendedWindowUrl", "http://site-gameplayer.dmm.com/emulator-recommend");
			}
			set
			{
				this.mClientKey.SetValue("RecommendedWindowUrl", value);
				this.mClientKey.Flush();
			}
		}

		public string DeviceProfileFromCloud
		{
			get
			{
				return (string)this.mClientKey.GetValue("DeviceProfileFromCloud", string.Empty);
			}
			set
			{
				this.mClientKey.SetValue("DeviceProfileFromCloud", value);
				this.mClientKey.Flush();
			}
		}

		public int GlPlusTransportConfig
		{
			get
			{
				return (int)this.mHostConfigKey.GetValue("GlPlusTransportConfig", 3);
			}
			set
			{
				this.mHostConfigKey.SetValue("GlPlusTransportConfig", value);
				this.mHostConfigKey.Flush();
			}
		}

		public int GlLegacyTransportConfig
		{
			get
			{
				return (int)this.mHostConfigKey.GetValue("GlLegacyTransportConfig", 0);
			}
			set
			{
				this.mHostConfigKey.SetValue("GlLegacyTransportConfig", value);
				this.mHostConfigKey.Flush();
			}
		}

		public string CurrentEngine
		{
			get
			{
				if (string.IsNullOrEmpty(this.sCurrentEngine))
				{
					this.sCurrentEngine = (string)this.mHostConfigKey.GetValue("CurrentEngine", "plus");
				}
				return this.sCurrentEngine;
			}
			set
			{
				this.sCurrentEngine = value;
				this.mHostConfigKey.SetValue("CurrentEngine", value);
				this.mHostConfigKey.Flush();
			}
		}

		public string EnginePreference
		{
			get
			{
				return (string)this.mHostConfigKey.GetValue("EnginePreference", "plus");
			}
			set
			{
				this.mHostConfigKey.SetValue("EnginePreference", value);
				this.mHostConfigKey.Flush();
			}
		}

		public string InstallDir
		{
			get
			{
				return (string)this.mBaseKey.GetValue("InstallDir", "");
			}
			set
			{
				this.mBaseKey.SetValue("InstallDir", value);
				this.mBaseKey.Flush();
			}
		}

		public bool IsUpgrade
		{
			get
			{
				return (int)this.mBaseKey.GetValue("IsUpgrade", 0) == 1;
			}
			set
			{
				this.mBaseKey.SetValue("IsUpgrade", value ? 1 : 0);
				this.mBaseKey.Flush();
			}
		}

		public string DataDir
		{
			get
			{
				return (string)this.mBaseKey.GetValue("DataDir", "");
			}
			set
			{
				this.mBaseKey.SetValue("DataDir", value);
				this.mBaseKey.Flush();
			}
		}

		public string UserDefinedDir
		{
			get
			{
				if (this.sUserDefinedDir == null)
				{
					string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
					this.sUserDefinedDir = (string)this.mBaseKey.GetValue("UserDefinedDir", folderPath);
				}
				return this.sUserDefinedDir;
			}
			set
			{
				this.sUserDefinedDir = value;
				this.mBaseKey.SetValue("UserDefinedDir", value);
				this.mBaseKey.Flush();
			}
		}

		public string LogDir
		{
			get
			{
				string text = (string)this.mBaseKey.GetValue("LogDir", null);
				if (text == null)
				{
					text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Bluestacks\\Logs");
				}
				return text;
			}
			set
			{
				this.mBaseKey.SetValue("LogDir", value);
				this.mBaseKey.Flush();
			}
		}

		public int PlusDebug
		{
			get
			{
				return (int)this.mBaseKey.GetValue("PlusDebug", 0);
			}
			set
			{
				this.mBaseKey.SetValue("PlusDebug", value);
				this.mBaseKey.Flush();
			}
		}

		public string Version
		{
			get
			{
				if (this.sVersion != null)
				{
					return this.sVersion;
				}
				this.sVersion = (string)this.mBaseKey.GetValue("Version", "");
				return this.sVersion;
			}
			set
			{
				this.mBaseKey.SetValue("Version", value);
				this.mBaseKey.Flush();
				this.sVersion = value;
			}
		}

		public string UserGuid
		{
			get
			{
				return (string)this.mBaseKey.GetValue("USER_GUID", "");
			}
			set
			{
				this.mBaseKey.SetValue("USER_GUID", value);
				this.mBaseKey.Flush();
			}
		}

		public string WebAppVersion
		{
			get
			{
				return (string)this.mClientKey.GetValue("WebAppVersion", string.Empty);
			}
			set
			{
				this.mClientKey.SetValue("WebAppVersion", value);
				this.mClientKey.Flush();
			}
		}

		public string OpenExternalLink
		{
			get
			{
				return (string)this.mClientKey.GetValue("OpenExternalLink", string.Empty);
			}
			set
			{
				this.mClientKey.SetValue("OpenExternalLink", value);
				this.mClientKey.Flush();
			}
		}

		public string ClientLaunchParams
		{
			get
			{
				return (string)this.mClientKey.GetValue("ClientLaunchParams", "");
			}
			set
			{
				this.mClientKey.SetValue("ClientLaunchParams", value);
				this.mClientKey.Flush();
			}
		}

		public string ApiToken
		{
			get
			{
				return (string)this.mBaseKey.GetValue("ApiToken", "");
			}
			set
			{
				this.mBaseKey.SetValue("ApiToken", value);
				this.mBaseKey.Flush();
			}
		}

		public bool IsBTVCheckedAfterUpdate
		{
			get
			{
				return (int)this.mHostConfigKey.GetValue("IsBTVCheckedAfterUpdate", 0) == 1;
			}
			set
			{
				this.mHostConfigKey.SetValue("IsBTVCheckedAfterUpdate", value ? 1 : 0);
				this.mHostConfigKey.Flush();
			}
		}

		public string CurrentBtvVersionInstalled
		{
			get
			{
				return (string)this.mHostConfigKey.GetValue("CurrentBtvVersionInstalled", "");
			}
			set
			{
				this.mHostConfigKey.SetValue("CurrentBtvVersionInstalled", value);
				this.mHostConfigKey.Flush();
			}
		}

		public bool IsFirstTimeCheck
		{
			get
			{
				bool flag = (int)this.mHostConfigKey.GetValue("IsFirstTimeCheck", 1) == 1;
				this.mHostConfigKey.SetValue("IsFirstTimeCheck", 0);
				this.mHostConfigKey.Flush();
				return flag;
			}
		}

		public int SystemInfoStats2
		{
			get
			{
				return (int)this.mHostConfigKey.GetValue("SystemInfoStats2", 0);
			}
			set
			{
				this.mHostConfigKey.SetValue("SystemInfoStats2", value);
				this.mHostConfigKey.Flush();
			}
		}

		public int Features
		{
			get
			{
				return (int)this.mHostConfigKey.GetValue("Features", 0);
			}
			set
			{
				this.mHostConfigKey.SetValue("Features", value);
				this.mHostConfigKey.Flush();
			}
		}

		public int FeaturesHigh
		{
			get
			{
				return (int)this.mHostConfigKey.GetValue("FeaturesHigh", 0);
			}
			set
			{
				this.mHostConfigKey.SetValue("FeaturesHigh", value);
				this.mHostConfigKey.Flush();
			}
		}

		public int SystemStats
		{
			get
			{
				return (int)this.mHostConfigKey.GetValue("SystemStats", 0);
			}
			set
			{
				this.mHostConfigKey.SetValue("SystemStats", value);
				this.mHostConfigKey.Flush();
			}
		}

		public int SendBotsCheckStats
		{
			get
			{
				return (int)this.mHostConfigKey.GetValue("SendBotsCheckStats", 0);
			}
			set
			{
				this.mHostConfigKey.SetValue("SendBotsCheckStats", 1);
			}
		}

		public string BotsCheckStatsTime
		{
			get
			{
				return (string)this.mHostConfigKey.GetValue("BotsCheckStatsTime", "");
			}
			set
			{
				this.mHostConfigKey.SetValue("BotsCheckStatsTime", value);
			}
		}

		public string Host
		{
			get
			{
				bool flag = string.IsNullOrEmpty(this.mHost);
				if (flag)
				{
					string text = (string)this.mHostConfigKey.GetValue("Host", "http://127.0.0.1");
					bool flag2 = string.IsNullOrEmpty(text);
					if (flag2)
					{
						text = "http://127.0.0.1";
					}
					this.mHost = text;
				}
				return this.mHost;
			}
			set
			{
				this.mHostConfigKey.SetValue("Host", value);
				this.mHostConfigKey.Flush();
				this.mHost = value;
			}
		}

		public string Host2
		{
			get
			{
				return (string)this.mHostConfigKey.GetValue("Host2", "https://23.23.194.123");
			}
			set
			{
				this.mHostConfigKey.SetValue("Host2", value);
				this.mHostConfigKey.Flush();
			}
		}

		public string RedDotShownOnIcon
		{
			get
			{
				return (string)this.mHostConfigKey.GetValue("RedDotShownOnIcon", "");
			}
			set
			{
				this.mHostConfigKey.SetValue("RedDotShownOnIcon", value);
				this.mHostConfigKey.Flush();
			}
		}

		public string TwitchServerPath
		{
			get
			{
				return (string)this.mBTVKey.GetValue("TwitchServerPath", "");
			}
			set
			{
				this.mBTVKey.SetValue("TwitchServerPath", value);
				this.mBTVKey.Flush();
			}
		}

		public int CLRBrowserServerPort
		{
			get
			{
				return (int)this.mBTVKey.GetValue("CLRBrowserServerPort", 2911);
			}
			set
			{
				this.mBTVKey.SetValue("CLRBrowserServerPort", value);
				this.mBTVKey.Flush();
			}
		}

		public string BtvDevServer
		{
			get
			{
				return (string)this.mBTVKey.GetValue("BtvDevServer", "");
			}
			set
			{
				this.mBTVKey.SetValue("BtvDevServer", value);
				this.mBTVKey.Flush();
			}
		}

		public string BtvNetwork
		{
			get
			{
				return (string)this.mBTVKey.GetValue("Network", "");
			}
			set
			{
				this.mBTVKey.SetValue("Network", value);
				this.mBTVKey.Flush();
			}
		}

		public int StreamingResolution
		{
			get
			{
				return (int)this.mBTVKey.GetValue("StreamingResolution", 0);
			}
			set
			{
				this.mBTVKey.SetValue("StreamingResolution", value);
				this.mBTVKey.Flush();
			}
		}

		public string SelectedCam
		{
			get
			{
				return (string)this.mBTVKey.GetValue("SelectedCam", string.Empty);
			}
			set
			{
				this.mBTVKey.SetValue("SelectedCam", value);
				this.mBTVKey.Flush();
			}
		}

		public int ReplayBufferEnabled
		{
			get
			{
				return (int)this.mBTVKey.GetValue("ReplayBufferEnabled", 0);
			}
			set
			{
				this.mBTVKey.SetValue("ReplayBufferEnabled", value);
				this.mBTVKey.Flush();
			}
		}

		public int BTVServerPort
		{
			get
			{
				return (int)this.mBTVKey.GetValue("BTVServerPort", 2885);
			}
			set
			{
				this.mBTVKey.SetValue("BTVServerPort", value);
				this.mBTVKey.Flush();
			}
		}

		public int AppViewLayout
		{
			get
			{
				return (int)this.mBTVKey.GetValue("AppViewLayout", 0);
			}
			set
			{
				this.mBTVKey.SetValue("AppViewLayout", value);
				this.mBTVKey.Flush();
			}
		}

		public string FilterUrl
		{
			get
			{
				return (string)this.mBTVKey.GetValue("FilterUrl", "");
			}
		}

		public string LayoutUrl
		{
			get
			{
				return (string)this.mBTVKey.GetValue("LayoutUrl", "");
			}
		}

		public string LayoutTheme
		{
			get
			{
				return (string)this.mBTVKey.GetValue("LayoutTheme", "");
			}
			set
			{
				this.mBTVKey.SetValue("LayoutTheme", value);
				this.mBTVKey.Flush();
			}
		}

		public string LastCameraLayoutTheme
		{
			get
			{
				return (string)this.mBTVKey.GetValue("LastCameraLayoutTheme", "");
			}
			set
			{
				this.mBTVKey.SetValue("LastCameraLayoutTheme", value);
				this.mBTVKey.Flush();
			}
		}

		public int ScreenWidth
		{
			get
			{
				return (int)this.mBTVKey.GetValue("ScreenWidth", 0);
			}
			set
			{
				this.mBTVKey.SetValue("ScreenWidth", value);
				this.mBTVKey.Flush();
			}
		}

		public int ScreenHeight
		{
			get
			{
				return (int)this.mBTVKey.GetValue("ScreenHeight", 0);
			}
			set
			{
				this.mBTVKey.SetValue("ScreenHeight", value);
				this.mBTVKey.Flush();
			}
		}

		public bool IsImeDebuggingEnabled
		{
			get
			{
				return (int)this.mHostConfigKey.GetValue("IsImeDebuggingEnabled", 0) == 1;
			}
		}

		public int OBSServerPort
		{
			get
			{
				return (int)this.mBTVKey.GetValue("OBSServerPort", 2851);
			}
			set
			{
				this.mBTVKey.SetValue("OBSServerPort", value);
				this.mBTVKey.Flush();
			}
		}

		public bool IsGameCaptureSupportedInMachine
		{
			get
			{
				return (int)this.mBTVKey.GetValue("IsGameCaptureSupportedInMachine", 1) != 0;
			}
			set
			{
				this.mBTVKey.SetValue("IsGameCaptureSupportedInMachine", (!value) ? 0 : 1);
				this.mBTVKey.Flush();
			}
		}

		public string StreamName
		{
			get
			{
				return (string)this.mBTVKey.GetValue("StreamName", "");
			}
			set
			{
				this.mBTVKey.SetValue("StreamName", value);
				this.mBTVKey.Flush();
			}
		}

		public string ServerLocation
		{
			get
			{
				return (string)this.mBTVKey.GetValue("ServerLocation", "");
			}
			set
			{
				this.mBTVKey.SetValue("ServerLocation", value);
				this.mBTVKey.Flush();
			}
		}

		public string ChannelName
		{
			get
			{
				return (string)this.mBTVFilterKey.GetValue("ChannelName", "");
			}
			set
			{
				this.mBTVFilterKey.SetValue("ChannelName", value);
				this.mBTVFilterKey.Flush();
			}
		}

		public string NotificationData
		{
			get
			{
				return (string)this.mHostConfigKey.GetValue("NotificationData", string.Empty);
			}
			set
			{
				this.mHostConfigKey.SetValue("NotificationData", value);
				this.mHostConfigKey.Flush();
			}
		}

		public string DeviceCaps
		{
			get
			{
				return (string)this.mHostConfigKey.GetValue("DeviceCaps", "");
			}
			set
			{
				this.mHostConfigKey.SetValue("DeviceCaps", value);
				this.mHostConfigKey.Flush();
			}
		}

		public int AgentServerPort
		{
			get
			{
				return (int)this.mHostConfigKey.GetValue("AgentServerPort", 2861);
			}
			set
			{
				this.mHostConfigKey.SetValue("AgentServerPort", value);
				this.mHostConfigKey.Flush();
			}
		}

		public int MultiInstanceServerPort
		{
			get
			{
				return (int)this.mHostConfigKey.GetValue("MultiInstanceServerPort", 2961);
			}
			set
			{
				this.mHostConfigKey.SetValue("MultiInstanceServerPort", value);
				this.mHostConfigKey.Flush();
			}
		}

		public string Oem
		{
			get
			{
				return (string)this.mHostConfigKey.GetValue("Oem", "gamemanager");
			}
			set
			{
				this.mHostConfigKey.SetValue("Oem", value);
				this.mHostConfigKey.Flush();
			}
		}

		public int BatchInstanceStartInterval
		{
			get
			{
				return (int)this.mHostConfigKey.GetValue("BatchInstanceStartInterval", 2);
			}
			set
			{
				this.mHostConfigKey.SetValue("BatchInstanceStartInterval", value);
				this.mHostConfigKey.Flush();
			}
		}

		public string CampaignName
		{
			get
			{
				return (string)this.mHostConfigKey.GetValue("CampaignName", "");
			}
			set
			{
				this.mHostConfigKey.SetValue("CampaignName", value);
				this.mHostConfigKey.Flush();
			}
		}

		public string PartnerExePath
		{
			get
			{
				return (string)this.mHostConfigKey.GetValue("PartnerExePath", "");
			}
			set
			{
				this.mHostConfigKey.SetValue("PartnerExePath", value);
				this.mHostConfigKey.Flush();
			}
		}

		public int CamStatus
		{
			get
			{
				return (int)this.mHostConfigKey.GetValue("CamStatus", 0);
			}
			set
			{
				this.mHostConfigKey.SetValue("CamStatus", value);
				this.mHostConfigKey.Flush();
			}
		}

		public int PartnerServerPort
		{
			get
			{
				return (int)this.mHostConfigKey.GetValue("PartnerServerPort", 2871);
			}
			set
			{
				this.mHostConfigKey.SetValue("PartnerServerPort", value);
				this.mHostConfigKey.Flush();
			}
		}

		public string RegisteredEmail
		{
			get
			{
				return (string)this.mUserKey.GetValue("RegisteredEmail", "");
			}
			set
			{
				this.mUserKey.SetValue("RegisteredEmail", value);
				this.mUserKey.Flush();
			}
		}

		public bool IsTimelineStats4Enabled
		{
			get
			{
				return (int)this.mHostConfigKey.GetValue("IsTimelineStats4Enabled", 0) != 0;
			}
			set
			{
				this.mHostConfigKey.SetValue("IsTimelineStats4Enabled", (!value) ? 0 : 1);
				this.mHostConfigKey.Flush();
			}
		}

		public bool EnableAutomation
		{
			get
			{
				return (int)this.mHostConfigKey.GetValue("EnableAutomation", 0) != 0;
			}
			set
			{
				this.mHostConfigKey.SetValue("EnableAutomation", (!value) ? 0 : 1);
				this.mHostConfigKey.Flush();
			}
		}

		public string PikaWorldId
		{
			get
			{
				return (string)this.mUserKey.GetValue("PikaWorldId", "");
			}
			set
			{
				this.mUserKey.SetValue("PikaWorldId", value);
				this.mUserKey.Flush();
			}
		}

		public string Token
		{
			get
			{
				return (string)this.mUserKey.GetValue("Token", "");
			}
			set
			{
				this.mUserKey.SetValue("Token", value);
				this.mUserKey.Flush();
			}
		}

		public bool IsPremium
		{
			get
			{
				return (int)this.mUserKey.GetValue("IsPremium", 0) == 1;
			}
			set
			{
				this.mUserKey.SetValue("IsPremium", value ? 1 : 0);
				this.mUserKey.Flush();
			}
		}

		public bool AddDesktopShortcuts
		{
			get
			{
				return (int)this.mHostConfigKey.GetValue("AddDesktopShortcuts", 1) != 0;
			}
			set
			{
				this.mHostConfigKey.SetValue("AddDesktopShortcuts", (!value) ? 0 : 1);
				this.mHostConfigKey.Flush();
			}
		}

		public bool SwitchToAndroidHome
		{
			get
			{
				return (int)this.mHostConfigKey.GetValue("SwitchToAndroidHome", 0) != 0;
			}
			set
			{
				this.mHostConfigKey.SetValue("SwitchToAndroidHome", (!value) ? 0 : 1);
				this.mHostConfigKey.Flush();
			}
		}

		public bool SwitchKillWebTab
		{
			get
			{
				return (int)this.mClientKey.GetValue("SwitchKillWebTab", 1) != 0;
			}
			set
			{
				this.mClientKey.SetValue("SwitchKillWebTab", (!value) ? 0 : 1);
				this.mClientKey.Flush();
			}
		}

		public bool EnableMemoryTrim
		{
			get
			{
				return (int)this.mHostConfigKey.GetValue("EnableMemoryTrim", 0) != 0;
			}
			set
			{
				this.mHostConfigKey.SetValue("EnableMemoryTrim", (!value) ? 0 : 1);
				this.mHostConfigKey.Flush();
			}
		}

		public bool GLES3
		{
			get
			{
				return (int)this.mHostConfigKey.GetValue("GLES3", 0) != 0;
			}
			set
			{
				this.mHostConfigKey.SetValue("GLES3", (!value) ? 0 : 1);
				this.mHostConfigKey.Flush();
			}
		}

		public bool IsAutoShowGuidance
		{
			get
			{
				return (int)this.mHostConfigKey.GetValue("IsAutoShowGuidance", 1) != 0;
			}
			set
			{
				this.mHostConfigKey.SetValue("IsAutoShowGuidance", (!value) ? 0 : 1);
				this.mHostConfigKey.Flush();
			}
		}

		[SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "<Pending>")]
		public string[] DisabledGuidancePackages
		{
			get
			{
				return (string[])this.mHostConfigKey.GetValue("DisabledGuidancePackages", new string[0]);
			}
			set
			{
				this.mHostConfigKey.SetValue("DisabledGuidancePackages", value);
				this.mHostConfigKey.Flush();
			}
		}

		public bool IsRememberWindowPositionEnabled
		{
			get
			{
				return (int)this.mHostConfigKey.GetValue("IsRememberWindowPositionEnabled", 1) != 0;
			}
			set
			{
				this.mHostConfigKey.SetValue("IsRememberWindowPositionEnabled", (!value) ? 0 : 1);
				this.mHostConfigKey.Flush();
			}
		}

		public string InstallID
		{
			get
			{
				return (string)this.mHostConfigKey.GetValue("InstallID", string.Empty);
			}
			set
			{
				this.mHostConfigKey.SetValue("InstallID", value);
				this.mHostConfigKey.Flush();
			}
		}

		public string OldInstallID
		{
			get
			{
				return (string)this.mHostConfigKey.GetValue("OldInstallID", string.Empty);
			}
			set
			{
				this.mHostConfigKey.SetValue("OldInstallID", value);
				this.mHostConfigKey.Flush();
			}
		}

		public string HelperVersion
		{
			get
			{
				return (string)this.mHostConfigKey.GetValue("HelperVersion", string.Empty);
			}
			set
			{
				this.mHostConfigKey.SetValue("HelperVersion", value);
				this.mHostConfigKey.Flush();
			}
		}

		public string InstallerPkgName
		{
			get
			{
				return (string)this.mHostConfigKey.GetValue("InstallerPkgName", string.Empty);
			}
			set
			{
				this.mHostConfigKey.SetValue("InstallerPkgName", value);
				this.mHostConfigKey.Flush();
			}
		}

		public InstallationTypes InstallationType
		{
			get
			{
				if (this.mInstallationType == InstallationTypes.None)
				{
					this.mInstallationType = (InstallationTypes)Enum.Parse(typeof(InstallationTypes), (string)this.mHostConfigKey.GetValue("InstallationType", InstallationTypes.FullEdition.ToString()), true);
				}
				return this.mInstallationType;
			}
			set
			{
				this.mHostConfigKey.SetValue("InstallationType", value);
				this.mHostConfigKey.Flush();
				this.mInstallationType = value;
			}
		}

		public string CurrentFirebaseHost
		{
			get
			{
				return (string)this.mClientKey.GetValue("CurrentFirebaseHost", string.Empty);
			}
			set
			{
				this.mClientKey.SetValue("CurrentFirebaseHost", value);
				this.mClientKey.Flush();
			}
		}

		public string PendingLaunchAction
		{
			get
			{
				return (string)this.mClientKey.GetValue("PendingLaunchAction", string.Format(CultureInfo.InvariantCulture, "{0},{1}", new object[]
				{
					GenericAction.None,
					string.Empty
				}));
			}
			set
			{
				this.mClientKey.SetValue("PendingLaunchAction", value);
				this.mClientKey.Flush();
			}
		}

		public DateTime AnnouncementTime
		{
			get
			{
				string text = (string)this.mHostConfigKey.GetValue("AnnouncementTime", string.Empty);
				DateTime dateTime = DateTime.Now;
				try
				{
					if (!string.IsNullOrEmpty(text))
					{
						dateTime = DateTime.ParseExact(text, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
					}
				}
				catch
				{
				}
				return dateTime;
			}
			set
			{
				string text = value.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
				this.mHostConfigKey.SetValue("AnnouncementTime", text);
				this.mHostConfigKey.Flush();
			}
		}

		public string RootVdiMd5Hash
		{
			get
			{
				return (string)this.mHostConfigKey.GetValue("RootVdiMd5Hash", string.Empty);
			}
			set
			{
				this.mHostConfigKey.SetValue("RootVdiMd5Hash", value);
				this.mHostConfigKey.Flush();
			}
		}

		public string Geo
		{
			get
			{
				return (string)this.mClientKey.GetValue("Geo", "");
			}
			set
			{
				this.mClientKey.SetValue("Geo", value);
				this.mClientKey.Flush();
			}
		}

		public string QuitDefaultOption
		{
			get
			{
				return (string)this.mClientKey.GetValue("QuitDefaultOption", "STRING_CLOSE_CURRENT_INSTANCE");
			}
			set
			{
				this.mClientKey.SetValue("QuitDefaultOption", value);
				this.mClientKey.Flush();
			}
		}

		public bool IsQuitOptionSaved
		{
			get
			{
				return (int)this.mClientKey.GetValue("QuitOptionSaved", 0) != 0;
			}
			set
			{
				this.mClientKey.SetValue("QuitOptionSaved", (!value) ? 0 : 1);
				this.mClientKey.Flush();
			}
		}

		public int VmId
		{
			get
			{
				return (int)this.mHostConfigKey.GetValue("VmId", 1);
			}
			set
			{
				this.mHostConfigKey.SetValue("VmId", value);
				this.mHostConfigKey.Flush();
			}
		}

		[SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "<Pending>")]
		public string[] BookmarkedScriptList
		{
			get
			{
				return (string[])this.mHostConfigKey.GetValue("BookmarkedScriptList", new string[0]);
			}
			set
			{
				this.mHostConfigKey.SetValue("BookmarkedScriptList", value, RegistryValueKind.MultiString);
			}
		}

		public bool CurrentFarmModeStatus
		{
			get
			{
				return (int)this.mHostConfigKey.GetValue("CurrentFarmModeStatus", 0) != 0;
			}
			set
			{
				this.mHostConfigKey.SetValue("CurrentFarmModeStatus", (!value) ? 0 : 1);
				this.mHostConfigKey.Flush();
			}
		}

		public string DefaultShortcuts
		{
			get
			{
				return (string)this.mHostConfigKey.GetValue("DefaultShortcuts", "");
			}
			set
			{
				this.mHostConfigKey.SetValue("DefaultShortcuts", value);
				this.mHostConfigKey.Flush();
			}
		}

		public string UserDefinedShortcuts
		{
			get
			{
				return (string)this.mHostConfigKey.GetValue("UserDefinedShortcuts", "");
			}
			set
			{
				this.mHostConfigKey.SetValue("UserDefinedShortcuts", Regex.Replace(value, "\\n|\\r", ""));
				this.mHostConfigKey.Flush();
			}
		}

		public bool AreAllInstancesMuted
		{
			get
			{
				return (int)this.mHostConfigKey.GetValue("AreAllInstancesMuted", 0) != 0;
			}
			set
			{
				this.mHostConfigKey.SetValue("AreAllInstancesMuted", (!value) ? 0 : 1);
				this.mHostConfigKey.Flush();
			}
		}

		public bool IsSamsungStorePresent
		{
			get
			{
				return (int)this.mHostConfigKey.GetValue("IsSamsungStorePresent", 0) != 0;
			}
			set
			{
				this.mHostConfigKey.SetValue("IsSamsungStorePresent", (!value) ? 0 : 1);
				this.mHostConfigKey.Flush();
			}
		}

		public bool IsCacodeValid
		{
			get
			{
				return (int)this.mHostConfigKey.GetValue("IsCacodeValid", 0) != 0;
			}
			set
			{
				this.mHostConfigKey.SetValue("IsCacodeValid", value ? 1 : 0);
				this.mHostConfigKey.Flush();
			}
		}

		public void SetClientThemeNameInRegistry(string themeName)
		{
			this.mClientKey.SetValue("ClientThemeName", themeName);
			RegistryManager.ClientThemeName = themeName;
			this.mClientKey.Flush();
		}

		public string GetClientThemeNameFromRegistry()
		{
			return this.mClientKey.GetValue("ClientThemeName", "Assets").ToString();
		}

		public int AdvancedControlTransparencyLevel
		{
			get
			{
				return (int)this.mClientKey.GetValue("AdvancedControlTransparencyLevel", 50);
			}
			set
			{
				this.mClientKey.SetValue("AdvancedControlTransparencyLevel", value);
				this.mClientKey.Flush();
			}
		}

		public AppLaunchState FirstAppLaunchState
		{
			get
			{
				if (this.mFirstAppLaunchState == AppLaunchState.Unknown)
				{
					this.mFirstAppLaunchState = (AppLaunchState)Enum.Parse(typeof(AppLaunchState), (string)this.mClientKey.GetValue("FirstAppLaunchedState", AppLaunchState.Launched.ToString()), true);
				}
				return this.mFirstAppLaunchState;
			}
			set
			{
				this.mClientKey.SetValue("FirstAppLaunchedState", value);
				this.mClientKey.Flush();
				this.mFirstAppLaunchState = value;
			}
		}

		public string AppConfiguration
		{
			get
			{
				return (string)this.mHostConfigKey.GetValue("AppConfiguration", "{ }");
			}
			set
			{
				this.mHostConfigKey.SetValue("AppConfiguration", value);
				this.mHostConfigKey.Flush();
			}
		}

		public string AppPlayerEngineInfo
		{
			get
			{
				return (string)this.mHostConfigKey.GetValue("AppPlayerEngineInfo", Constants.DefaultAppPlayerEngineInfo);
			}
			set
			{
				this.mHostConfigKey.SetValue("AppPlayerEngineInfo", value);
				this.mHostConfigKey.Flush();
			}
		}

		public int CloudABIValue
		{
			get
			{
				return (int)this.mHostConfigKey.GetValue("CloudABIValue", 0);
			}
			set
			{
				this.mHostConfigKey.SetValue("CloudABIValue", value);
				this.mHostConfigKey.Flush();
			}
		}

		public int NotificationModeCounter
		{
			get
			{
				return (int)this.mHostConfigKey.GetValue("NotificationModeCounter", 3);
			}
			set
			{
				this.mHostConfigKey.SetValue("NotificationModeCounter", value);
				this.mHostConfigKey.Flush();
			}
		}

		public bool IsNotificationModeAlwaysOn
		{
			get
			{
				return (int)this.mHostConfigKey.GetValue("IsNotificationModeAlwaysOn", 0) != 0;
			}
			set
			{
				this.mHostConfigKey.SetValue("IsNotificationModeAlwaysOn", (!value) ? 0 : 1);
				this.mHostConfigKey.Flush();
			}
		}

		public bool IsNotificationSoundsActive
		{
			get
			{
				return (int)this.mHostConfigKey.GetValue("IsNotificationSoundsActive", 1) != 0;
			}
			set
			{
				this.mHostConfigKey.SetValue("IsNotificationSoundsActive", (!value) ? 0 : 1);
				this.mHostConfigKey.Flush();
			}
		}

		public string UpdaterFileDeletePath
		{
			get
			{
				return (string)this.mClientKey.GetValue("UpdaterFileDeletePath", "");
			}
			set
			{
				this.mClientKey.SetValue("UpdaterFileDeletePath", value);
				this.mClientKey.Flush();
			}
		}

		private static string mUPGRADE_TAG = string.Empty;

		public const string UPGRADE_TAG_NEW = ".new";

		private RegistryKey mBaseKey;

		private RegistryKey mClientKey;

		private RegistryKey mBTVKey;

		private RegistryKey mBTVFilterKey;

		private RegistryKey mUserKey;

		private RegistryKey mHostConfigKey;

		private RegistryKey mGuestsKey;

		private RegistryKey mMonitorsKey;

		private bool mIsAdmin;

		private static RegistryManager sInstance = null;

		private static object sLock = new object();

		private static Dictionary<string, RegistryManager> _RegistryManagers;

		private string sCurrentEngine = "";

		private string sUserDefinedDir;

		private string sVersion;

		private string mHost = string.Empty;

		private InstallationTypes mInstallationType;

		private AppLaunchState mFirstAppLaunchState;
	}
}


