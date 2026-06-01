using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Win32;

namespace BlueStacks.Common
{
	public class InstanceRegistry
	{
		public InstanceRegistry(string vmId, string oem = "bgp64")
		{
			this.mVmId = vmId;
			if (oem == null)
			{
				oem = "bgp64";
			}
			this.Init(oem);
		}

		private void Init(string oem = "bgp64")
		{
			string text = (oem.Equals("bgp", StringComparison.InvariantCultureIgnoreCase) ? "" : ("_" + oem));
			this.mBaseKeyPath = "Software\\BlueStacks" + text + RegistryManager.UPGRADE_TAG;
			this.AndroidKeyPath = this.mBaseKeyPath + "\\Guests\\" + this.mVmId;
			this.mBlockDeviceKeyPath = this.AndroidKeyPath + "\\BlockDevice";
			this.mBlockDevice0KeyPath = this.AndroidKeyPath + "\\BlockDevice\\0";
			this.mBlockDevice1KeyPath = this.AndroidKeyPath + "\\BlockDevice\\1";
			this.mBlockDevice2KeyPath = this.AndroidKeyPath + "\\BlockDevice\\2";
			this.mBlockDevice3KeyPath = this.AndroidKeyPath + "\\BlockDevice\\3";
			this.mBlockDevice4KeyPath = this.AndroidKeyPath + "\\BlockDevice\\4";
			this.mVmConfigKeyPath = this.AndroidKeyPath + "\\Config";
			this.mFrameBufferKeyPath = this.AndroidKeyPath + "\\FrameBuffer";
			this.mFrameBuffer0KeyPath = this.AndroidKeyPath + "\\FrameBuffer\\0";
			this.mNetworkKeyPath = this.AndroidKeyPath + "\\Network";
			this.mNetwork0KeyPath = this.AndroidKeyPath + "\\Network\\0";
			this.mNetworkRedirectKeyPath = this.AndroidKeyPath + "\\Network\\Redirect";
			this.mSharedFolderKeyPath = this.AndroidKeyPath + "\\SharedFolder";
			this.mSharedFolder0KeyPath = this.AndroidKeyPath + "\\SharedFolder\\0";
			this.mSharedFolder1KeyPath = this.AndroidKeyPath + "\\SharedFolder\\1";
			this.mSharedFolder2KeyPath = this.AndroidKeyPath + "\\SharedFolder\\2";
			this.mSharedFolder3KeyPath = this.AndroidKeyPath + "\\SharedFolder\\3";
			this.mSharedFolder4KeyPath = this.AndroidKeyPath + "\\SharedFolder\\4";
			this.mSharedFolder5KeyPath = this.AndroidKeyPath + "\\SharedFolder\\5";
			RegistryUtils.InitKey(this.mBlockDevice0KeyPath);
			RegistryUtils.InitKey(this.mBlockDevice1KeyPath);
			RegistryUtils.InitKey(this.mBlockDevice2KeyPath);
			RegistryUtils.InitKey(this.mBlockDevice3KeyPath);
			RegistryUtils.InitKey(this.mBlockDevice4KeyPath);
			RegistryUtils.InitKey(this.mVmConfigKeyPath);
			RegistryUtils.InitKey(this.mFrameBuffer0KeyPath);
			RegistryUtils.InitKey(this.mNetwork0KeyPath);
			RegistryUtils.InitKey(this.mNetworkRedirectKeyPath);
			RegistryUtils.InitKey(this.mSharedFolder0KeyPath);
			RegistryUtils.InitKey(this.mSharedFolder1KeyPath);
			RegistryUtils.InitKey(this.mSharedFolder2KeyPath);
			RegistryUtils.InitKey(this.mSharedFolder3KeyPath);
			RegistryUtils.InitKey(this.mSharedFolder4KeyPath);
			RegistryUtils.InitKey(this.mSharedFolder5KeyPath);
		}

		public string AndroidKeyPath { get; private set; } = "";

		public int EmulatePortraitMode
		{
			get
			{
				return (int)RegistryUtils.GetRegistryValue(this.mFrameBuffer0KeyPath, "EmulatePortraitMode", -1, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mFrameBuffer0KeyPath, "EmulatePortraitMode", value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public int Depth
		{
			get
			{
				return (int)RegistryUtils.GetRegistryValue(this.mFrameBuffer0KeyPath, "Depth", 0, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mFrameBuffer0KeyPath, "Depth", value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public int HideBootProgress
		{
			get
			{
				return (int)RegistryUtils.GetRegistryValue(this.mFrameBuffer0KeyPath, "HideBootProgress", 0, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mFrameBuffer0KeyPath, "HideBootProgress", value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public int WindowWidth
		{
			get
			{
				return (int)RegistryUtils.GetRegistryValue(this.mFrameBuffer0KeyPath, "WindowWidth", 0, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mFrameBuffer0KeyPath, "WindowWidth", value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public int WindowHeight
		{
			get
			{
				return (int)RegistryUtils.GetRegistryValue(this.mFrameBuffer0KeyPath, "WindowHeight", 0, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mFrameBuffer0KeyPath, "WindowHeight", value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public int GuestWidth
		{
			get
			{
				return (int)RegistryUtils.GetRegistryValue(this.mFrameBuffer0KeyPath, "GuestWidth", 0, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mFrameBuffer0KeyPath, "GuestWidth", value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public int GuestHeight
		{
			get
			{
				return (int)RegistryUtils.GetRegistryValue(this.mFrameBuffer0KeyPath, "GuestHeight", 0, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mFrameBuffer0KeyPath, "GuestHeight", value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public int Memory
		{
			get
			{
				return (int)RegistryUtils.GetRegistryValue(this.AndroidKeyPath, "Memory", 0, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.AndroidKeyPath, "Memory", value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public bool IsSidebarVisible
		{
			get
			{
				return (int)RegistryUtils.GetRegistryValue(this.AndroidKeyPath, "IsSidebarVisible", 1, RegistryKeyKind.HKEY_LOCAL_MACHINE) != 0;
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.AndroidKeyPath, "IsSidebarVisible", value ? 1 : 0, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public bool IsTopbarVisible
		{
			get
			{
				return (int)RegistryUtils.GetRegistryValue(this.AndroidKeyPath, "IsTopbarVisible", 1, RegistryKeyKind.HKEY_LOCAL_MACHINE) != 0;
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.AndroidKeyPath, "IsTopbarVisible", value ? 1 : 0, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public bool IsSidebarInDefaultState
		{
			get
			{
				return (int)RegistryUtils.GetRegistryValue(this.AndroidKeyPath, "IsSidebarInDefaultState", 1, RegistryKeyKind.HKEY_LOCAL_MACHINE) != 0;
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.AndroidKeyPath, "IsSidebarInDefaultState", value ? 1 : 0, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public string Kernel
		{
			get
			{
				return (string)RegistryUtils.GetRegistryValue(this.AndroidKeyPath, "Kernel", null, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.AndroidKeyPath, "Kernel", value, RegistryValueKind.String, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public string Initrd
		{
			get
			{
				return (string)RegistryUtils.GetRegistryValue(this.AndroidKeyPath, "Initrd", null, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.AndroidKeyPath, "Initrd", value, RegistryValueKind.String, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public int DisableRobustness
		{
			get
			{
				return (int)RegistryUtils.GetRegistryValue(this.AndroidKeyPath, "DisableRobustness", 0, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.AndroidKeyPath, "DisableRobustness", value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public string VirtType
		{
			get
			{
				return (string)RegistryUtils.GetRegistryValue(this.AndroidKeyPath, "VirtType", "", RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.AndroidKeyPath, "VirtType", value, RegistryValueKind.String, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public string BootParameters
		{
			get
			{
				return (string)RegistryUtils.GetRegistryValue(this.AndroidKeyPath, "BootParameters", "", RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.AndroidKeyPath, "BootParameters", value, RegistryValueKind.String, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public bool ShowSidebarInFullScreen
		{
			get
			{
				return (int)RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, "ShowSidebarInFullScreen", 1, RegistryKeyKind.HKEY_LOCAL_MACHINE) != 0;
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, "ShowSidebarInFullScreen", (!value) ? 0 : 1, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public string BlockDevice0Name
		{
			get
			{
				return (string)RegistryUtils.GetRegistryValue(this.mBlockDevice0KeyPath, "Name", "", RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mBlockDevice0KeyPath, "Name", value, RegistryValueKind.String, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public string BlockDevice0Path
		{
			get
			{
				return (string)RegistryUtils.GetRegistryValue(this.mBlockDevice0KeyPath, "Path", "", RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mBlockDevice0KeyPath, "Path", value, RegistryValueKind.String, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public string BlockDevice1Name
		{
			get
			{
				return (string)RegistryUtils.GetRegistryValue(this.mBlockDevice1KeyPath, "Name", "", RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mBlockDevice1KeyPath, "Name", value, RegistryValueKind.String, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public string BlockDevice1Path
		{
			get
			{
				return (string)RegistryUtils.GetRegistryValue(this.mBlockDevice1KeyPath, "Path", "", RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mBlockDevice1KeyPath, "Path", value, RegistryValueKind.String, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public string BlockDevice2Name
		{
			get
			{
				return (string)RegistryUtils.GetRegistryValue(this.mBlockDevice2KeyPath, "Name", "", RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mBlockDevice2KeyPath, "Name", value, RegistryValueKind.String, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public string BlockDevice2Path
		{
			get
			{
				return (string)RegistryUtils.GetRegistryValue(this.mBlockDevice2KeyPath, "Path", "", RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mBlockDevice2KeyPath, "Path", value, RegistryValueKind.String, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public string BlockDevice4Name
		{
			get
			{
				return (string)RegistryUtils.GetRegistryValue(this.mBlockDevice4KeyPath, "Name", "", RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mBlockDevice4KeyPath, "Name", value, RegistryValueKind.String, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public string BlockDevice4Path
		{
			get
			{
				return (string)RegistryUtils.GetRegistryValue(this.mBlockDevice4KeyPath, "Path", "", RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mBlockDevice4KeyPath, "Path", value, RegistryValueKind.String, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public string Locale
		{
			get
			{
				return (string)RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, "Locale", "", RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, "Locale", value, RegistryValueKind.String, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public int VCPUs
		{
			get
			{
				return (int)RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, "VCPUs", Utils.GetRecommendedVCPUCount(this.mVmId == "Android"), RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, "VCPUs", value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public string EnableConsoleAccess
		{
			get
			{
				return (string)RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, "EnableConsoleAccess", string.Empty, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, "EnableConsoleAccess", value, RegistryValueKind.String, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public int GlRenderMode
		{
			get
			{
				return (int)RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, "GlRenderMode", -1, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, "GlRenderMode", value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public int FPS
		{
			get
			{
				return (int)RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, "FPS", 60, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, "FPS", value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public int ShowFPS
		{
			get
			{
				return (int)RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, "ShowFPS", 0, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, "ShowFPS", value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public bool AutoLockFps
		{
			get
			{
				return (int)RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, "AutoLockFps", 0, RegistryKeyKind.HKEY_LOCAL_MACHINE) != 0;
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, "AutoLockFps", value ? 1 : 0, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public int EnableAdb
		{
			get
			{
				return (int)RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, "EnableAdb", 0, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, "EnableAdb", value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public int AutoLockFpsValue
		{
			get
			{
				return (int)RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, "AutoLockFpsValue", 8, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, "AutoLockFpsValue", value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public int UnlockFpsValue
		{
			get
			{
				return (int)RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, "UnlockFpsValue", 1, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, "UnlockFpsValue", value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public string AutoUnlockKey
		{
			get
			{
				return (string)RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, "AutoUnlockKey", "", RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, "AutoUnlockKey", value, RegistryValueKind.String, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public int AutoUnlockEnabled
		{
			get
			{
				return (int)RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, "AutoUnlockEnabled", 0, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, "AutoUnlockEnabled", value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public int EnableHighFPS
		{
			get
			{
				return (int)RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, "EnableHighFPS", 0, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, "EnableHighFPS", value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public int GlMode
		{
			get
			{
				return (int)RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, "GlMode", 0, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, "GlMode", value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public int Camera
		{
			get
			{
				return (int)RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, "Camera", 0, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, "Camera", value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public int ConfigSynced
		{
			get
			{
				return (int)RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, "ConfigSynced", 0, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, "ConfigSynced", value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public int HScroll
		{
			get
			{
				return (int)RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, "HScroll", 0, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, "HScroll", value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public int GpsMode
		{
			get
			{
				return (int)RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, "GpsMode", 0, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, "GpsMode", value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public int FileSystem
		{
			get
			{
				return (int)RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, "FileSystem", 0, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, "FileSystem", value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public int StopZygoteOnClose
		{
			get
			{
				return (int)RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, "StopZygoteOnClose", 0, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, "StopZygoteOnClose", value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public int FenceSyncType
		{
			get
			{
				return (int)RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, "FenceSyncType", 0, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, "FenceSyncType", value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public int FrontendNoClose
		{
			get
			{
				return (int)RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, "FrontendNoClose", 0, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, "FrontendNoClose", value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public int GpsSource
		{
			get
			{
				return (int)RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, "GpsSource", 0, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, "GpsSource", value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public string GpsLatitude
		{
			get
			{
				return (string)RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, "GpsLatitude", "", RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, "GpsLatitude", value, RegistryValueKind.String, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public string GpsLongitude
		{
			get
			{
				return (string)RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, "GpsLongitude", "", RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, "GpsLongitude", value, RegistryValueKind.String, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public int GlPort
		{
			get
			{
				return (int)RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, "GlPort", 0, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, "GlPort", value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public string GamingResolutionPubg
		{
			get
			{
				return (string)RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, "GamingResolutionPubg", "1", RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, "GamingResolutionPubg", value, RegistryValueKind.String, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public string DisplayQualityPubg
		{
			get
			{
				return (string)RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, "DisplayQualityPubg", "-1", RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, "DisplayQualityPubg", value, RegistryValueKind.String, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public string GamingResolutionCOD
		{
			get
			{
				return (string)RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, "GamingResolutionCOD", "720", RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, "GamingResolutionCOD", value, RegistryValueKind.String, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public string DisplayQualityCOD
		{
			get
			{
				return (string)RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, "DisplayQualityCOD", "-1", RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, "DisplayQualityCOD", value, RegistryValueKind.String, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public int HostSensorPort
		{
			get
			{
				return (int)RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, "HostSensorPort", 0, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, "HostSensorPort", value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public int SoftControlBarHeightLandscape
		{
			get
			{
				return (int)RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, "SoftControlBarHeightLandscape", 0, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, "SoftControlBarHeightLandscape", value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public int SoftControlBarHeightPortrait
		{
			get
			{
				return (int)RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, "SoftControlBarHeightPortrait", 0, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, "SoftControlBarHeightPortrait", value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public int GrabKeyboard
		{
			get
			{
				return (int)RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, "GrabKeyboard", 1, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, "GrabKeyboard", value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public int DisableDWM
		{
			get
			{
				return (int)RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, "DisableDWM", 0, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, "DisableDWM", value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public int DisablePcIme
		{
			get
			{
				return (int)RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, "DisablePcIme", 0, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, "DisablePcIme", value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public int EnableBSTVC
		{
			get
			{
				return (int)RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, "EnableBSTVC", 0, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, "EnableBSTVC", value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public int ForceVMLegacyMode
		{
			get
			{
				return (int)RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, "ForceVMLegacyMode", 0, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, "ForceVMLegacyMode", value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public int FrontendServerPort
		{
			get
			{
				return (int)RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, "FrontendServerPort", 2881, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, "FrontendServerPort", value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public int BstAndroidPort
		{
			get
			{
				return (int)RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, "BstAndroidPort", 9999, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, "BstAndroidPort", value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public int BstAdbPort
		{
			get
			{
				return (int)RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, "BstAdbPort", 5555, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, "BstAdbPort", value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public int TriggerMemoryTrimThreshold
		{
			get
			{
				return (int)RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, "TriggerMemoryTrimThreshold", 700, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, "TriggerMemoryTrimThreshold", value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public int TriggerMemoryTrimTimerInterval
		{
			get
			{
				return (int)RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, "TriggerMemoryTrimTimerInterval", 60000, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, "TriggerMemoryTrimTimerInterval", value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public int UpdatedVersion
		{
			get
			{
				return (int)RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, "UpdatedVersion", 0, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, "UpdatedVersion", value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public int GPSAvailable
		{
			get
			{
				return (int)RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, "GPSAvailable", 0, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, "GPSAvailable", value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public string OpenSensorDeviceId
		{
			get
			{
				return (string)RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, "OpenSensorDeviceId", "", RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, "OpenSensorDeviceId", value, RegistryValueKind.String, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public int HostForwardSensorPort
		{
			get
			{
				return (int)RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, "HostForwardSensorPort", 0, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, "HostForwardSensorPort", value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public string ImeSelected
		{
			get
			{
				return (string)RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, "ImeSelected", "", RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, "ImeSelected", value, RegistryValueKind.String, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public int RunAppProcessId
		{
			get
			{
				return (int)RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, "RunAppProcessId", 0, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, "RunAppProcessId", value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public string DisplayName
		{
			get
			{
				return (string)RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, "DisplayName", string.Empty, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, "DisplayName", value, RegistryValueKind.String, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public string LastBootDate
		{
			get
			{
				return (string)RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, "LastBootDate", DateTime.Now.Date.ToShortDateString(), RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, "LastBootDate", value, RegistryValueKind.String, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public bool IsOneTimeSetupDone
		{
			get
			{
				return (int)RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, "IsOneTimeSetupDone", 0, RegistryKeyKind.HKEY_LOCAL_MACHINE) != 0;
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, "IsOneTimeSetupDone", value ? 1 : 0, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public bool IsMuted
		{
			get
			{
				return (int)RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, "IsMuted", 0, RegistryKeyKind.HKEY_LOCAL_MACHINE) != 0;
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, "IsMuted", value ? 1 : 0, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public int Volume
		{
			get
			{
				return (int)RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, "Volume", 5, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, "Volume", value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public bool FixVboxConfig
		{
			get
			{
				return (int)RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, "FixVboxConfig", 0, RegistryKeyKind.HKEY_LOCAL_MACHINE) != 0;
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, "FixVboxConfig", value ? 1 : 0, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public string WindowPlacement
		{
			get
			{
				return (string)RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, "WindowPlacement", "", RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, "WindowPlacement", value, RegistryValueKind.String, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public bool IsGoogleSigninDone
		{
			get
			{
				return (int)RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, "IsGoogleSigninDone", 0, RegistryKeyKind.HKEY_LOCAL_MACHINE) != 0;
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, "IsGoogleSigninDone", value ? 1 : 0, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public bool IsGoogleSigninPopupShown
		{
			get
			{
				return (int)RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, "IsGoogleSigninPopupShown", 0, RegistryKeyKind.HKEY_LOCAL_MACHINE) != 0;
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, "IsGoogleSigninPopupShown", value ? 1 : 0, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		[SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "<Pending>")]
		public string[] GrmDonotShowRuleList
		{
			get
			{
				return (string[])RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, "GrmDonotShowRuleList", new string[0], RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, "GrmDonotShowRuleList", value, RegistryValueKind.MultiString, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public string GoogleAId
		{
			get
			{
				return (string)RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, "BstVmAId", string.Empty, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, "BstVmAId", value, RegistryValueKind.String, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public string AndroidId
		{
			get
			{
				return (string)RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, "BstVmId", string.Empty, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, "BstVmId", value, RegistryValueKind.String, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public bool ShowMacroDeletePopup
		{
			get
			{
				return (int)RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, "ShowMacroDeletePopup", 1, RegistryKeyKind.HKEY_LOCAL_MACHINE) != 0;
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, "ShowMacroDeletePopup", value ? 1 : 0, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public bool ShowSchemeDeletePopup
		{
			get
			{
				return (int)RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, "ShowSchemeDeletePopup", 1, RegistryKeyKind.HKEY_LOCAL_MACHINE) != 0;
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, "ShowSchemeDeletePopup", value ? 1 : 0, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public bool IsFreeFireInGameSettingsCustomized
		{
			get
			{
				return (int)RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, "IsFreeFireInGameSettingsCustomized", 1, RegistryKeyKind.HKEY_LOCAL_MACHINE) != 0;
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, "IsFreeFireInGameSettingsCustomized", value ? 1 : 0, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public bool IsClientOnTop
		{
			get
			{
				return (int)RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, "IsClientOnTop", 0, RegistryKeyKind.HKEY_LOCAL_MACHINE) != 0;
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, "IsClientOnTop", value ? 1 : 0, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public ASTCOption ASTCOption
		{
			get
			{
				return (ASTCOption)RegistryUtils.GetRegistryValue(this.AndroidKeyPath, "ASTCOption", FeatureManager.Instance.IsCustomUIForNCSoft ? ASTCOption.SoftwareDecodingCache : ASTCOption.Disabled, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.AndroidKeyPath, "ASTCOption", value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public bool IsHardwareAstcSupported
		{
			get
			{
				return (int)RegistryUtils.GetRegistryValue(this.AndroidKeyPath, "IsHardwareAstcSupported", 0, RegistryKeyKind.HKEY_LOCAL_MACHINE) != 0;
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.AndroidKeyPath, "IsHardwareAstcSupported", value ? 1 : 0, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public NativeGamepadState NativeGamepadState
		{
			get
			{
				return (NativeGamepadState)RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, "NativeGamepadState", NativeGamepadState.Auto, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, "NativeGamepadState", value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public bool IsShowMinimizeBlueStacksPopupOnClose
		{
			get
			{
				return (int)RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, "IsShowMinimizeBlueStacksPopupOnClose", 1, RegistryKeyKind.HKEY_LOCAL_MACHINE) != 0;
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, "IsShowMinimizeBlueStacksPopupOnClose", value ? 1 : 0, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public bool IsMinimizeSelectedOnReceiveGameNotificationPopup
		{
			get
			{
				return (int)RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, "IsMinimizeSelectedOnReceiveGameNotificationPopup", 0, RegistryKeyKind.HKEY_LOCAL_MACHINE) != 0;
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, "IsMinimizeSelectedOnReceiveGameNotificationPopup", value ? 1 : 0, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public int NotificationModePopupShownCount
		{
			get
			{
				return (int)RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, "NotificationModePopupShownCount", 0, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, "NotificationModePopupShownCount", value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public string LastNotificationEnabledAppLaunched
		{
			get
			{
				return (string)RegistryUtils.GetRegistryValue(this.mVmConfigKeyPath, "LastNotificationEnabledAppLaunched", string.Empty, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mVmConfigKeyPath, "LastNotificationEnabledAppLaunched", value, RegistryValueKind.String, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		[SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "<Pending>")]
		public string[] NetworkInboundRules
		{
			get
			{
				return (string[])RegistryUtils.GetRegistryValue(this.mNetwork0KeyPath, "InboundRules", null, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mNetwork0KeyPath, "InboundRules", value, RegistryValueKind.MultiString, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public string AllowRemoteAccess
		{
			get
			{
				return (string)RegistryUtils.GetRegistryValue(this.mNetwork0KeyPath, "AllowRemoteAccess", null, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mNetwork0KeyPath, "AllowRemoteAccess", value, RegistryValueKind.String, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public int NetworkRedirectTcp5555
		{
			get
			{
				return (int)RegistryUtils.GetRegistryValue(this.mNetworkRedirectKeyPath, "tcp/5555", 5555, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mNetworkRedirectKeyPath, "tcp/5555", value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public int NetworkRedirectTcp6666
		{
			get
			{
				return (int)RegistryUtils.GetRegistryValue(this.mNetworkRedirectKeyPath, "tcp/6666", 6666, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mNetworkRedirectKeyPath, "tcp/6666", value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public int NetworkRedirectTcp7777
		{
			get
			{
				return (int)RegistryUtils.GetRegistryValue(this.mNetworkRedirectKeyPath, "tcp/7777", 7777, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mNetworkRedirectKeyPath, "tcp/7777", value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public int NetworkRedirectTcp9999
		{
			get
			{
				return (int)RegistryUtils.GetRegistryValue(this.mNetworkRedirectKeyPath, "tcp/9999", 8888, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mNetworkRedirectKeyPath, "tcp/9999", value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public int NetworkRedirectUdp12000
		{
			get
			{
				return (int)RegistryUtils.GetRegistryValue(this.mNetworkRedirectKeyPath, "udp/12000", 12000, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mNetworkRedirectKeyPath, "udp/12000", value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public string SharedFolder0Name
		{
			get
			{
				return (string)RegistryUtils.GetRegistryValue(this.mSharedFolder0KeyPath, "Name", "", RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mSharedFolder0KeyPath, "Name", value, RegistryValueKind.String, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public string SharedFolder0Path
		{
			get
			{
				return (string)RegistryUtils.GetRegistryValue(this.mSharedFolder0KeyPath, "Path", "", RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mSharedFolder0KeyPath, "Path", value, RegistryValueKind.String, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public int SharedFolder0Writable
		{
			get
			{
				return (int)RegistryUtils.GetRegistryValue(this.mSharedFolder0KeyPath, "Writable", 0, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mSharedFolder0KeyPath, "Writable", value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public string SharedFolder1Name
		{
			get
			{
				return (string)RegistryUtils.GetRegistryValue(this.mSharedFolder1KeyPath, "Name", "", RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mSharedFolder1KeyPath, "Name", value, RegistryValueKind.String, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public string SharedFolder1Path
		{
			get
			{
				return (string)RegistryUtils.GetRegistryValue(this.mSharedFolder1KeyPath, "Path", "", RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mSharedFolder1KeyPath, "Path", value, RegistryValueKind.String, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public int SharedFolder1Writable
		{
			get
			{
				return (int)RegistryUtils.GetRegistryValue(this.mSharedFolder1KeyPath, "Writable", 0, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mSharedFolder1KeyPath, "Writable", value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public string SharedFolder2Name
		{
			get
			{
				return (string)RegistryUtils.GetRegistryValue(this.mSharedFolder2KeyPath, "Name", "", RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mSharedFolder2KeyPath, "Name", value, RegistryValueKind.String, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public string SharedFolder2Path
		{
			get
			{
				return (string)RegistryUtils.GetRegistryValue(this.mSharedFolder2KeyPath, "Path", "", RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mSharedFolder2KeyPath, "Path", value, RegistryValueKind.String, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public int SharedFolder2Writable
		{
			get
			{
				return (int)RegistryUtils.GetRegistryValue(this.mSharedFolder2KeyPath, "Writable", 0, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mSharedFolder2KeyPath, "Writable", value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public string SharedFolder3Name
		{
			get
			{
				return (string)RegistryUtils.GetRegistryValue(this.mSharedFolder3KeyPath, "Name", "", RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mSharedFolder3KeyPath, "Name", value, RegistryValueKind.String, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public string SharedFolder3Path
		{
			get
			{
				return (string)RegistryUtils.GetRegistryValue(this.mSharedFolder3KeyPath, "Path", "", RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mSharedFolder3KeyPath, "Path", value, RegistryValueKind.String, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public int SharedFolder3Writable
		{
			get
			{
				return (int)RegistryUtils.GetRegistryValue(this.mSharedFolder3KeyPath, "Writable", 0, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mSharedFolder3KeyPath, "Writable", value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public string SharedFolder4Name
		{
			get
			{
				return (string)RegistryUtils.GetRegistryValue(this.mSharedFolder4KeyPath, "Name", "", RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mSharedFolder4KeyPath, "Name", value, RegistryValueKind.String, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public string SharedFolder4Path
		{
			get
			{
				return (string)RegistryUtils.GetRegistryValue(this.mSharedFolder4KeyPath, "Path", "", RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mSharedFolder4KeyPath, "Path", value, RegistryValueKind.String, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public int SharedFolder4Writable
		{
			get
			{
				return (int)RegistryUtils.GetRegistryValue(this.mSharedFolder4KeyPath, "Writable", 0, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mSharedFolder4KeyPath, "Writable", value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public string SharedFolder5Name
		{
			get
			{
				return (string)RegistryUtils.GetRegistryValue(this.mSharedFolder5KeyPath, "Name", "", RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mSharedFolder5KeyPath, "Name", value, RegistryValueKind.String, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public string SharedFolder5Path
		{
			get
			{
				return (string)RegistryUtils.GetRegistryValue(this.mSharedFolder5KeyPath, "Path", "", RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mSharedFolder5KeyPath, "Path", value, RegistryValueKind.String, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		public int SharedFolder5Writable
		{
			get
			{
				return (int)RegistryUtils.GetRegistryValue(this.mSharedFolder5KeyPath, "Writable", 0, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
			set
			{
				RegistryUtils.SetRegistryValue(this.mSharedFolder5KeyPath, "Writable", value, RegistryValueKind.DWord, RegistryKeyKind.HKEY_LOCAL_MACHINE);
			}
		}

		private string mVmId;

		private string mBaseKeyPath = "";

		private string mBlockDeviceKeyPath = "";

		private string mBlockDevice0KeyPath = "";

		private string mBlockDevice1KeyPath = "";

		private string mBlockDevice2KeyPath = "";

		private string mBlockDevice3KeyPath = "";

		private string mBlockDevice4KeyPath = "";

		private string mVmConfigKeyPath = "";

		private string mFrameBufferKeyPath = "";

		private string mFrameBuffer0KeyPath = "";

		private string mNetworkKeyPath = "";

		private string mNetwork0KeyPath = "";

		private string mNetworkRedirectKeyPath = "";

		private string mSharedFolderKeyPath = "";

		private string mSharedFolder0KeyPath = "";

		private string mSharedFolder1KeyPath = "";

		private string mSharedFolder2KeyPath = "";

		private string mSharedFolder3KeyPath = "";

		private string mSharedFolder4KeyPath = "";

		private string mSharedFolder5KeyPath = "";
	}
}


