using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;

namespace BlueStacks.Common
{
	public static class LocaleStrings
	{
		public static event EventHandler SourceUpdatedEvent;

		public static Dictionary<string, string> DictLocalizedString
		{
			get
			{
				if (LocaleStrings.sDictLocalizedString == null)
				{
					LocaleStrings.InitLocalization(null, "Android", false);
				}
				return LocaleStrings.sDictLocalizedString;
			}
			set
			{
				LocaleStrings.sDictLocalizedString = value;
			}
		}

		public static string Locale { get; set; }

		public static void InitLocalization(string localeDir = null, string vmName = "Android", bool skipLocalePickFromRegistry = false)
		{
			if (localeDir == null)
			{
				LocaleStrings.sResourceLocation = Path.Combine(RegistryManager.Instance.UserDefinedDir, "Locales");
			}
			else
			{
				LocaleStrings.sResourceLocation = localeDir;
			}
			LocaleStrings.sDictLocalizedString = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
			LocaleStrings.Locale = LocaleStrings.GetLocaleName(vmName, skipLocalePickFromRegistry);
			Globalization.PopulateLocaleStrings(LocaleStrings.sResourceLocation, LocaleStrings.sDictLocalizedString, "en-US");
			if (string.Compare(LocaleStrings.Locale, "en-US", StringComparison.OrdinalIgnoreCase) != 0)
			{
				Globalization.PopulateLocaleStrings(LocaleStrings.sResourceLocation, LocaleStrings.sDictLocalizedString, LocaleStrings.Locale);
			}
			EventHandler sourceUpdatedEvent = LocaleStrings.SourceUpdatedEvent;
			if (sourceUpdatedEvent == null)
			{
				return;
			}
			sourceUpdatedEvent("Locale_Updated", null);
		}

		public static string GetLocaleName(string vmName, bool skipLocalePickFromRegistry = false)
		{
			string text = (skipLocalePickFromRegistry ? null : RegistryManager.Instance.Guest[vmName].Locale);
			if (string.IsNullOrEmpty(text))
			{
				if (FeatureManager.Instance.IsCustomUIForDMM)
				{
					return "ja-JP";
				}
				text = Globalization.FindClosestMatchingLocale(Thread.CurrentThread.CurrentCulture.Name);
			}
			return text;
		}

		public static string GetLocalizedString(string id, string fallbackValue = "")
		{
			if (id == null)
			{
				return string.Empty;
			}
			string text = id.Trim();
			try
			{
				if (LocaleStrings.sDictLocalizedString == null)
				{
					LocaleStrings.InitLocalization(null, "Android", false);
				}
				if (LocaleStrings.sDictLocalizedString.ContainsKey(id.ToUpper(CultureInfo.InvariantCulture)))
				{
					text = LocaleStrings.sDictLocalizedString[id.ToUpper(CultureInfo.InvariantCulture)];
				}
				else if (string.IsNullOrEmpty(fallbackValue))
				{
					text = LocaleStrings.RemoveConstants(id);
				}
				else
				{
					text = fallbackValue;
				}
			}
			catch
			{
				Logger.Warning("Localized string not available for: {0}", new object[] { id });
			}
			return text;
		}

		internal static string RemoveConstants(string path)
		{
			if (path.Contains(Constants.ImapLocaleStringsConstant))
			{
				path = path.Replace(Constants.ImapLocaleStringsConstant, "");
				path = path.Replace("_", " ");
			}
			else if (path.Contains(Constants.LocaleStringsConstant))
			{
				path = path.Replace(Constants.LocaleStringsConstant, "");
				path = path.Replace("_", " ");
			}
			return path;
		}

		public static bool AppendLocaleIfDoesntExist(string key, string value)
		{
			bool flag = false;
			try
			{
				if (!LocaleStrings.sDictLocalizedString.ContainsKey(key))
				{
					LocaleStrings.sDictLocalizedString.Add(key, value);
					flag = true;
				}
			}
			catch (Exception ex)
			{
				Logger.Warning("Error appending locale entry: {0}" + ex.Message);
			}
			return flag;
		}

		private static string sResourceLocation;

		private static Dictionary<string, string> sDictLocalizedString;
	}
}


