using System;
using System.Collections.Generic;

namespace BlueStacks.Common
{
	public static class Stats
	{
		private static string SessionId
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		public static string GetSessionId()
		{
			return null;
		}

		public static string ResetSessionId()
		{
			return null;
		}

		public static void SendAppStats(string appName, string packageName, string appVersion, string homeVersion, Stats.AppType appType, string vmName, string appVersionName = "")
		{
		}

		public static void SendAppStats(string appName, string packageName, string appVersion, string homeVersion, Stats.AppType appType, string source, string vmName, string appVersionName)
		{
		}

		public static void SendWebAppChannelStats(string appName, string packageName, string homeVersion, string source, string vmName)
		{
		}

		public static void SendSearchAppStats(string keyword, string vmName)
		{
		}

		public static void SendSearchAppStats(string keyword, string source, string vmName)
		{
		}

		public static void SendAppInstallStats(string appName, string packageName, string appVersion, string appVersionName, string appInstall, string isUpdate, string source, string vmName, string campaignName, string clientVersion, string apkType = "")
		{
		}

		public static void SendSystemInfoStats(string vmName)
		{
		}

		public static void SendSystemInfoStatsAsync(string host, bool createRegKey, Dictionary<string, string> dataInfo, string guid, string pfDir, string pdDir, string vmName)
		{
		}

		public static string SendSystemInfoStatsSync(string host, bool createRegKey, Dictionary<string, string> dataInfo, string guid, string programFilesDir, string programDataDir, string vmName)
		{
			return null;
		}

		public static void SendFrontendStatusUpdate(string evt, string vmName)
		{
		}

		public static void SendTimelineStats(long agent_timestamp, long sequence, string evt, long duration, string s1, string s2, string s3, string s4, string s5, string s6, string s7, string s8, string timezone, string locale, long from_timestamp, long to_timestamp, long from_ticks, long to_ticks, string vmName)
		{
		}

		public static void SendBootStats(string type, bool booted, bool wait, string vmName)
		{
		}

		public static void SendHomeScreenDisplayedStats(string vmName)
		{
		}

		public static void SendBtvFunnelStatsSync(string network, string statEvent, string statDataKey, string statDataValue, string vmName)
		{
		}

		public static void SendStyleAndThemeInfoStats(string actionName, string styleName, string themeName, string optionalParam, string vmName)
		{
		}

		public static void SendStyleAndThemeInfoStatsAsync(string actionName, string styleName, string themeName, string optionalParam, string vmName)
		{
		}

		public static void SendStyleAndThemeInfoStatsSync(string actionName, string styleName, string themeName, string optionalParam, string vmName)
		{
		}

		public static void SendMiscellaneousStatsSync(string tag, string arg1, string arg2, string arg3, string arg4, string arg5, string arg6 = null, string arg7 = null, string arg8 = null, string vmName = "Android", int timeOut = 0)
		{
		}

		public static void SendMiscellaneousStatsAsync(string tag, string arg1, string arg2, string arg3, string arg4, string arg5, string arg6 = null, string arg7 = null, string arg8 = null, string vmName = "Android", int timeOut = 0)
		{
		}

		public static void SendGamingMouseStats(string jsonData, string vmName)
		{
		}

		public static void SendData(string url, Dictionary<string, string> data, string vmName, int timeOut = 0)
		{
		}

		public static void SendCommonClientStatsAsync(string featureType, string eventType, string vmName, string packageName = "", string extraInfo = "", string arg2 = "")
		{
		}

		private static Dictionary<string, string> CollectStyleAndThemeData(string actionName, string styleName, string themeName, string optionalParam)
		{
			return null;
		}

		private static string Timestamp
		{
			get
			{
				return null;
			}
		}

		private static string GetURLSafeBase64String(string originalString)
		{
			return null;
		}

		public static void SendMultiInstanceStatsAsync(string vmId, string oem, string cloneType, string eventType, string timeCompletion, string exitCode, bool wait)
		{
		}

		public static void SendMultiInstanceStatsAsync(string eventName, string displayName, string performance, string resolution, int abiValue, string dpi, int instanceCount, string oemOption, string prodVerOption, string arg1, string arg2, string vmId, string utmCampaign, bool isMim)
		{
		}

		public static void SendTroubleshooterStatsASync(string eventType, string issueName, string ver, string vm)
		{
		}

		public static Dictionary<string, string> GetUnifiedInstallStatsCommonData()
		{
			return null;
		}

		public static void SendUnifiedInstallStatsAsync(string eventName, string email = "")
		{
		}

		public static string SendUnifiedInstallStats(string eventName, string email = "")
		{
			return null;
		}

		public const string AppInstall = "true";

		public const string AppUninstall = "false";

		private static string sSessionId;

		public enum AppType
		{
			app,
			market,
			suggestedapps,
			web
		}
	}
}


