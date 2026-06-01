using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;

namespace BlueStacks.Common
{
	public static class Globalization
	{
		public static void InitLocalization(string resourceLocation = null)
		{
			if (string.IsNullOrEmpty(resourceLocation))
			{
				Globalization.sResourceLocation = Path.Combine(Globalization.sUserDefinedDir, "Locales");
			}
			else
			{
				Globalization.sResourceLocation = resourceLocation;
			}
			Globalization.sLocalizedStringsDict = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
			Globalization.sLocale = Globalization.GetCurrentCultureSupportedLocaleName();
			if (Globalization.PopulateLocaleStrings(Globalization.sResourceLocation, Globalization.sLocalizedStringsDict, "en-US"))
			{
				Logger.Info("Successfully populated {0} strings", new object[] { "en-US" });
			}
			if (string.Compare(Globalization.sLocale, "en-US", StringComparison.OrdinalIgnoreCase) != 0)
			{
				bool flag = Globalization.PopulateLocaleStrings(Globalization.sResourceLocation, Globalization.sLocalizedStringsDict, Globalization.sLocale);
				Logger.Info("Populated strings for {0}: {1}", new object[]
				{
					Globalization.sLocale,
					flag
				});
			}
		}

		private static string GetCurrentCultureSupportedLocaleName()
		{
			string text = Thread.CurrentThread.CurrentCulture.Name;
			if (!Globalization.sSupportedLocales.ContainsKey(text))
			{
				text = "en-US";
				string text2 = Globalization.sSupportedLocales.Keys.FirstOrDefault((string x) => x.StartsWith(Thread.CurrentThread.CurrentCulture.Parent.Name, StringComparison.OrdinalIgnoreCase));
				if (!string.IsNullOrEmpty(text2))
				{
					text = text2;
				}
			}
			return text;
		}

		public static string FindClosestMatchingLocale(string requestedLocale)
		{
			string text = "en-US";
			Logger.Info("Finding closest locale match to {0}", new object[] { requestedLocale });
			try
			{
				List<string> list = Globalization.sSupportedLocales.Keys.ToList<string>();
				bool flag = false;
				string twoLetterISOLanguageNameFromLocale = Globalization.GetTwoLetterISOLanguageNameFromLocale(requestedLocale);
				string regionFromLocale = Globalization.GetRegionFromLocale(requestedLocale);
				foreach (string text2 in list)
				{
					if (string.Equals(regionFromLocale, Globalization.GetRegionFromLocale(text2), StringComparison.InvariantCultureIgnoreCase))
					{
						Logger.Info("Match found by region: {0}", new object[] { text2 });
						text = text2;
						flag = true;
						break;
					}
					if (string.Equals(twoLetterISOLanguageNameFromLocale, Globalization.GetTwoLetterISOLanguageNameFromLocale(text2), StringComparison.InvariantCultureIgnoreCase))
					{
						Logger.Info("Match found by ISO language name: {0}", new object[] { text2 });
						text = text2;
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					Logger.Warning("No locale match could be found, defaulting to: {0}", new object[] { "en-US" });
					text = "en-US";
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Some error occured. Ex: {0}", new object[] { ex.ToString() });
				Logger.Warning("Defaulting to: {0}", new object[] { "en-US" });
				text = "en-US";
			}
			return text;
		}

		public static string GetTwoLetterISOLanguageNameFromLocale(string requestedLocale)
		{
			try
			{
				return new CultureInfo(requestedLocale).TwoLetterISOLanguageName;
			}
			catch
			{
			}
			return ((requestedLocale != null) ? requestedLocale.Split(new char[] { '-' }) : null)[0];
		}

		public static string GetRegionFromLocale(string requestedLocale)
		{
			try
			{
				return new RegionInfo(new CultureInfo(requestedLocale).LCID).Name;
			}
			catch
			{
			}
			string[] array = ((requestedLocale != null) ? requestedLocale.Split(new char[] { '-' }) : null);
			return array[array.Length - 1];
		}

		public static string GetLocalizedString(string id)
		{
			string text = ((id != null) ? id.Trim() : null);
			try
			{
				if (Globalization.sLocalizedStringsDict == null)
				{
					Globalization.InitLocalization(null);
				}
				if (Globalization.sLocalizedStringsDict.ContainsKey((id != null) ? id.ToUpper(CultureInfo.InvariantCulture) : null))
				{
					text = Globalization.sLocalizedStringsDict[(id != null) ? id.ToUpper(CultureInfo.InvariantCulture) : null];
				}
			}
			catch (Exception ex)
			{
				Logger.Warning("Localized string not available for: {0}. Ex: {1}", new object[] { id, ex.Message });
			}
			return text;
		}

		public static bool PopulateLocaleStrings(string resourceLocation, Dictionary<string, string> dict, string locale)
		{
			bool flag;
			try
			{
				string text = Path.Combine(resourceLocation, "i18n." + locale + ".txt");
				if (!File.Exists(text))
				{
					Logger.Info("String file {0} does not exist", new object[] { text });
					flag = false;
				}
				else
				{
					Globalization.FillDictionary(dict, text);
					Logger.Info("Successfully populated {0} strings", new object[] { locale });
					flag = true;
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Could not populate localized strings. Error: {0}", new object[] { ex });
				flag = false;
			}
			return flag;
		}

		public static void FillDictionary(Dictionary<string, string> dict, string filePath)
		{
			try
			{
				if (dict == null)
				{
					throw new NullReferenceException("Dictionary to fill cannot be null");
				}
				foreach (string text in File.ReadAllLines(filePath))
				{
					if (text.IndexOf("=", StringComparison.OrdinalIgnoreCase) != -1)
					{
						string[] array2 = text.Split(new char[] { '=' });
						string text2 = array2[1].Trim();
						if (text2.Contains("@@STRING_PRODUCT_NAME@@"))
						{
							text2 = text2.Replace("@@STRING_PRODUCT_NAME@@", Strings.ProductDisplayName);
						}
						dict[array2[0].Trim().ToUpper(CultureInfo.InvariantCulture)] = text2;
					}
				}
			}
			catch
			{
				throw;
			}
		}

		public const string DEFAULT_LOCALE = "en-US";

		private static string sLocale;

		private static string sResourceLocation;

		private static string sUserDefinedDir = (string)RegistryUtils.GetRegistryValue(Strings.RegistryBaseKeyPath, "UserDefinedDir", "", RegistryKeyKind.HKEY_LOCAL_MACHINE);

		private static Dictionary<string, string> sLocalizedStringsDict = null;

		public static readonly Dictionary<string, string> sSupportedLocales = new Dictionary<string, string>
		{
			{
				"en-US",
				new CultureInfo("en-US").NativeName
			},
			{
				"ar-EG",
				new CultureInfo("ar-EG").NativeName
			},
			{
				"de-DE",
				new CultureInfo("de-DE").NativeName
			},
			{
				"es-ES",
				new CultureInfo("es-ES").NativeName
			},
			{
				"fr-FR",
				new CultureInfo("fr-FR").NativeName
			},
			{
				"it-IT",
				new CultureInfo("it-IT").NativeName
			},
			{
				"ja-JP",
				new CultureInfo("ja-JP").NativeName
			},
			{
				"ko-KR",
				new CultureInfo("ko-KR").NativeName
			},
			{
				"pl-PL",
				new CultureInfo("pl-PL").NativeName
			},
			{
				"pt-BR",
				new CultureInfo("pt-BR").NativeName
			},
			{
				"ru-RU",
				new CultureInfo("ru-RU").NativeName
			},
			{
				"th-TH",
				new CultureInfo("th-TH").NativeName
			},
			{
				"tr-TR",
				new CultureInfo("tr-TR").NativeName
			},
			{
				"vi-VN",
				new CultureInfo("vi-VN").NativeName
			},
			{
				"zh-TW",
				new CultureInfo("zh-TW").NativeName
			}
		};
	}
}


