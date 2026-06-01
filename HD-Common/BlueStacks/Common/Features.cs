using System;

namespace BlueStacks.Common
{
	public static class Features
	{
		public static ulong GetEnabledFeatures()
		{
			ulong num = (ulong)Convert.ToUInt32(RegistryManager.Instance.Features);
			ulong num2 = ((ulong)Convert.ToUInt32(RegistryManager.Instance.FeaturesHigh) << 32) | num;
			return num2 & 18446744064582745692UL & 18446744064851163673UL;
		}

		public static void SetEnabledFeatures(ulong feature)
		{
			feature &= 18446744064851163673UL;
			feature &= 18446744064582745692UL;
			uint num;
			uint num2;
			Features.GetHighLowFeatures(feature, out num, out num2);
			RegistryManager.Instance.Features = (int)num2;
			RegistryManager.Instance.FeaturesHigh = (int)num;
		}

		public static void GetHighLowFeatures(ulong features, out uint featuresHigh, out uint featuresLow)
		{
			featuresLow = (uint)features;
			featuresHigh = (uint)(features >> 32);
		}

		public static bool IsFeatureEnabled(ulong featureMask)
		{
			bool flag = (featureMask & 8858387942UL) > 0UL;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				bool flag3 = (featureMask & 9126805923UL) > 0UL;
				bool flag4 = flag3;
				if (flag4)
				{
					flag2 = false;
				}
				else
				{
					ulong num = Features.GetEnabledFeatures();
					bool flag5 = num == 0UL;
					if (flag5)
					{
						num = Oem.Instance.WindowsOEMFeatures;
					}
					flag2 = Features.IsFeatureEnabled(featureMask, num);
				}
			}
			return flag2;
		}

		public static bool IsFeatureEnabled(ulong featureMask, ulong features)
		{
			bool flag = (featureMask & 8858387942UL) > 0UL;
			return !flag && (featureMask & 9126805923UL) <= 0UL && (features & featureMask) > 0UL;
		}

		public static void DisableFeature(ulong featureMask)
		{
			ulong enabledFeatures = Features.GetEnabledFeatures();
			bool flag = (enabledFeatures & featureMask) > 0UL;
			if (flag)
			{
				Features.SetEnabledFeatures(enabledFeatures & ~featureMask);
			}
		}

		public static void EnableFeature(ulong featureMask)
		{
			bool flag = (featureMask & 8858387942UL) > 0UL;
			if (!flag)
			{
				bool flag2 = (featureMask & 9126805923UL) == 0UL;
				if (flag2)
				{
					ulong enabledFeatures = Features.GetEnabledFeatures();
					bool flag3 = (enabledFeatures & featureMask) == 0UL;
					if (flag3)
					{
						Features.SetEnabledFeatures(enabledFeatures | featureMask);
					}
				}
			}
		}

		public static void EnableAllFeatures()
		{
			Features.SetEnabledFeatures(9223372025312033304UL);
		}

		public static void EnableFeaturesOfOem()
		{
			Features.SetEnabledFeatures(Oem.Instance.WindowsOEMFeatures & 18446744064582745692UL & 18446744064851163673UL);
		}

		public static bool IsFullScreenToggleEnabled()
		{
			return true;
		}

		public static bool IsHomeButtonEnabled()
		{
			return true;
		}

		public static bool IsShareButtonEnabled()
		{
			return false;
		}

		public static bool IsGraphicsDriverReminderEnabled()
		{
			return false;
		}

		public static bool IsSettingsButtonEnabled()
		{
			return true;
		}

		public static bool IsBackButtonEnabled()
		{
			return true;
		}

		public static bool IsMenuButtonEnabled()
		{
			return true;
		}

		public static bool ExitOnHome()
		{
			return false;
		}

		public static bool UpdateFrontendAppTitle()
		{
			return false;
		}

		public static bool UseDefaultNetworkText()
		{
			return false;
		}

		public const ulong BROADCAST_MESSAGES = 1UL;

		public const ulong INSTALL_NOTIFICATIONS = 2UL;

		public const ulong UNINSTALL_NOTIFICATIONS = 4UL;

		public const ulong CREATE_APP_SHORTCUTS = 8UL;

		public const ulong LAUNCH_SETUP_APP = 16UL;

		public const ulong SHOW_USAGE_STATS = 32UL;

		public const ulong SYS_TRAY_SUPPORT = 64UL;

		public const ulong SUGGESTED_APPS_SUPPORT = 128UL;

		public const ulong OTA_SUPPORT = 256UL;

		public const ulong SHOW_RESTART = 512UL;

		public const ulong ANDROID_NOTIFICATIONS = 1024UL;

		public const ulong RIGHT_ALIGN_PORTRAIT_MODE = 2048UL;

		public const ulong LAUNCH_FRONTEND_AFTER_INSTALLTION = 4096UL;

		public const ulong CREATE_LIBRARY = 8192UL;

		public const ulong SHOW_AGENT_ICON_IN_SYSTRAY = 16384UL;

		public const ulong IS_HOME_BUTTON_ENABLED = 32768UL;

		public const ulong IS_GRAPHICS_DRIVER_REMINDER_ENABLED = 65536UL;

		public const ulong EXIT_ON_HOME = 131072UL;

		public const ulong MULTI_INSTANCE_SUPPORT = 262144UL;

		public const ulong UPDATE_FRONTEND_APP_TITLE = 524288UL;

		public const ulong USE_DEFAULT_NETWORK_TEXT = 1048576UL;

		public const ulong IS_FULL_SCREEN_TOGGLE_ENABLED = 2097152UL;

		public const ulong SET_CHINA_LOCALE_AND_TIMEZONE = 4194304UL;

		public const ulong SHOW_TOGGLE_BUTTON_IN_LOADING_SCREEN = 8388608UL;

		public const ulong ENABLE_ALT_CTRL_I_SHORTCUTS = 16777216UL;

		public const ulong CREATE_LIBRARY_SHORTCUT_AT_DESKTOP = 33554432UL;

		public const ulong CREATE_START_LAUNCHER_SHORTCUT = 67108864UL;

		public const ulong WRITE_APP_CRASH_LOGS = 268435456UL;

		public const ulong CHINA_CLOUD = 536870912UL;

		public const ulong FORCE_DESKTOP_MODE = 1073741824UL;

		public const ulong NOT_TO_BE_USED = 2147483648UL;

		public const ulong ENABLE_ALT_CTRL_M_SHORTCUTS = 4294967296UL;

		public const ulong COLLECT_APK_HANDLER_LOGS = 8589934592UL;

		public const ulong SHOW_FRONTEND_FULL_SCREEN_TOAST = 17179869184UL;

		public const ulong IS_CHINA_UI = 34359738368UL;

		public const ulong NOT_TO_BE_USED_2 = 9223372036854775808UL;

		public const ulong ALL_FEATURES = 9223372034707292159UL;

		public const uint BST_HIDE_NAVIGATIONBAR = 1U;

		public const uint BST_HIDE_STATUSBAR = 2U;

		public const uint BST_HIDE_BACKBUTTON = 4U;

		public const uint BST_HIDE_HOMEBUTTON = 8U;

		public const uint BST_HIDE_RECENTSBUTTON = 16U;

		public const uint BST_HIDE_SCREENSHOTBUTTON = 32U;

		public const uint BST_HIDE_TOGGLEBUTTON = 64U;

		public const uint BST_HIDE_CLOSEBUTTON = 128U;

		public const uint BST_HIDE_GPS = 512U;

		public const uint BST_SHOW_APKINSTALLBUTTON = 2048U;

		public const uint BST_HIDE_HOMEAPPNEWLOADER = 65536U;

		public const uint BST_SENDLETSGOS2PCLICKREPORT = 131072U;

		public const uint BST_DISABLE_P2DM = 262144U;

		public const uint BST_DISABLE_ARMTIPS = 524288U;

		public const uint BST_DISABLE_S2P = 1048576U;

		public const uint BST_SOGOUIME = 268435456U;

		public const uint BST_BAIDUIME = 1073741824U;

		public const uint BST_QQIME = 2147483648U;

		public const uint BST_QEMU_3BT_COEXISTENCE_BIT = 536870912U;

		public const uint BST_HIDE_S2P_SEARCH_BAIDU_IN_HOMEAPPNEW = 4194304U;

		public const uint BST_NEW_TASK_ON_HOME = 2097152U;

		public const uint BST_NO_REINSTALL = 67108864U;

		public const int BST_HIDE_GUIDANCESCREEN = 1024;

		public const int BST_USE_CHINESE_CDN = 4096;

		public const int BST_ENALBE_ABOUT_PHONE_OPTION = 16777216;

		public const int BST_ENABLE_SECURITY_OPTION = 33554432;

		public const uint BST_SKIP_S2P_WHILE_LAUNCHING_APP = 2048U;

		internal static string ConfigFeature = "net.";

		private const ulong BLOCKED_FEATURES = 9126805923UL;

		private const ulong BLOATWARE_MASK = 8858387942UL;
	}
}


