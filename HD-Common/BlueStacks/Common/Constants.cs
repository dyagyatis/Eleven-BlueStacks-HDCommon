using System;
using System.IO;

namespace BlueStacks.Common
{
	public static class Constants
	{
		public static string MOBACursorPath
		{
			get
			{
				return Path.Combine(Path.Combine(RegistryManager.Instance.ClientInstallDir, RegistryManager.Instance.GetClientThemeNameFromRegistry()), "Mouse_cursor_MOBA.cur");
			}
		}

		public static string CustomCursorPath
		{
			get
			{
				return Path.Combine(Path.Combine(RegistryManager.Instance.ClientInstallDir, RegistryManager.Instance.GetClientThemeNameFromRegistry()), "Mouse_cursor.cur");
			}
		}

		public static string BrawlStarsMOBACursorPath
		{
			get
			{
				return Path.Combine(Path.Combine(RegistryManager.Instance.ClientInstallDir, "Assets"), "Mouse_cursor_MOBA_brawl.cur");
			}
		}

		public static string BrawlStarsCustomCursorPath
		{
			get
			{
				return Path.Combine(Path.Combine(RegistryManager.Instance.ClientInstallDir, "Assets"), "Mouse_cursor_brawl.cur");
			}
		}

		public const string MacroPostFix = "_macro";

		public const string dateFormat = "yyyy-MM-dd HH:mm";

		public static readonly Version win8version = new Version(6, 2, 9200, 0);

		public const int IDENTITY_OFFSET = 16;

		public const int GUEST_ABS_MAX_X = 32768;

		public const int GUEST_ABS_MAX_Y = 32768;

		public const int MaxAllowedCPUCores = 8;

		public const int TOUCH_POINTS_MAX = 16;

		public const int SWIPE_TOUCH_POINTS_MAX = 1;

		public const long LWIN_TIMEOUT_TICKS = 1000000L;

		public const int CURSOR_HIDE_CLIP_LEN = 15;

		public static readonly string LocaleStringsConstant = "STRING_";

		public static readonly string ImapLocaleStringsConstant = "IMAP_" + Constants.LocaleStringsConstant;

		public const string ImapDependent = "Dependent";

		public const string ImapIndependent = "Independent";

		public const string ImapSubElement = "SubElement";

		public const string ImapParentElement = "ParentElement";

		public const string ImapNotCommon = "NotCommon";

		public const string ImapLinked = "Linked";

		public const string ImapCanvasElementY = "IMAP_CanvasElementX";

		public const string ImapCanvasElementX = "IMAP_CanvasElementY";

		public const string ImapCanvasElementRadius = "IMAP_CanvasElementRadius";

		public const string IMAPPopupUIElement = "IMAP_PopupUIElement";

		public const string IMAPKeypropertyPrefix = "Key";

		public const string IMAPUserDefined = "User-Defined";

		public const string ImapVideoHeaderConstant = "AAVideo";

		public const string ImapMiscHeaderConstant = "MISC";

		public const string ImapGlobalValid = "GlobalValidTag";

		public const string ImapDeveloperModeUIElemnt = "IMAP_DeveloperModeUIElemnt";

		public const string ImapGamepadStartKey = "GamepadStart";

		public const string ImapGamepadBackKey = "GamepadBack";

		public const string ImapGamepadLeftStickKey = "LeftStick";

		public const string ImapGamepadRightStickKey = "RightStick";

		public static readonly string[] ImapGamepadEvents = new string[]
		{
			"GamepadDpadUp", "GamepadDpadDown", "GamepadDpadLeft", "GamepadDpadRight", "GamepadStart", "GamepadStop", "GamepadLeftThumb", "GamepadRightThumb", "GamepadLeftShoulder", "GamepadRightShoulder",
			"GamepadA", "GamepadB", "GamepadX", "GamepadY", "GamepadLStickUp", "GamepadLStickDown", "GamepadLStickLeft", "GamepadLStickRight", "GamepadRtickUp", "GamepadRStickDown",
			"GamepadRStickLeft", "GamepadRStickRight", "GamepadLTrigger", "GamepadRTrigger"
		};

		public static readonly string[] ReservedFileNamesList = new string[]
		{
			"con", "prn", "aux", "nul", "clock$", "com1", "com2", "com3", "com4", "com5",
			"com6", "com7", "com8", "com9", "lpt1", "lpt3", "lpt3", "lpt4", "lpt5", "lpt6",
			"lpt7", "lpt8", "lpt9"
		};

		public const string CustomCursorImageName = "yellow_cursor";

		public const string BrawlStarsCustomCursorImageName = "yellow_cursor_brawl";

		public static readonly string[] ImapGameControlsHiddenInOverlayList = new string[] { "Zoom", "Tilt", "Swipe", "State", "MouseZoom" };

		public static readonly string DefaultAppPlayerEngineInfo = "[{\"oem\":\"bgp\",\"prod_ver\":\"\",\"display_name\":\"Android N-32\",\"download_url\":\"\",\"abi_value\":15,\"suffix\":\"\"},{\"oem\":\"bgp64\",\"prod_ver\":\"\",\"display_name\":\"Android N-64 (32 bit apps)\",\"download_url\":\"\",\"abi_value\":7,\"suffix\":\"N-64 (32)\"},{\"oem\":\"bgp64\",\"prod_ver\":\"\",\"display_name\":\"Android N-64\",\"download_url\":\"\",\"abi_value\":15,\"suffix\":\"N-64\"}]";
	}
}


