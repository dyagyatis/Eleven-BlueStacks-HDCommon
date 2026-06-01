using System;
using System.Collections.Generic;
using System.Globalization;

namespace BlueStacks.Common
{
	public static class WebHelper
	{
		public static string GetServerHost()
		{
			return RegistryManager.Instance.Host + "/bs3";
		}

		public static string GetServerHostForFirebase()
		{
			return "http://127.0.0.1";
		}

		public static string GetUrlWithParams(string url)
		{
			string text = "bgp64";
			string clientVersion = RegistryManager.Instance.ClientVersion;
			string version = RegistryManager.Instance.Version;
			string userGuid = RegistryManager.Instance.UserGuid;
			string userSelectedLocale = RegistryManager.Instance.UserSelectedLocale;
			string partner = RegistryManager.Instance.Partner;
			string campaignMD = RegistryManager.Instance.CampaignMD5;
			string text2 = RegistryManager.Instance.InstallationType.ToString();
			string pkgName = GameConfig.Instance.PkgName;
			string webAppVersion = RegistryManager.Instance.WebAppVersion;
			string text3 = "oem=";
			text3 += text;
			text3 += "&prod_ver=";
			text3 += clientVersion;
			text3 += "&eng_ver=";
			text3 += version;
			text3 += "&guid=";
			text3 += userGuid;
			text3 += "&locale=";
			text3 += userSelectedLocale;
			text3 += "&launcher_version=";
			text3 += webAppVersion;
			bool flag = !string.IsNullOrEmpty(partner);
			if (flag)
			{
				text3 += "&partner=";
			}
			text3 += partner;
			bool flag2 = !string.IsNullOrEmpty(campaignMD);
			if (flag2)
			{
				text3 += "&campaign_md5=";
			}
			text3 += campaignMD;
			bool flag3 = !string.IsNullOrEmpty(url);
			if (flag3)
			{
				Uri uri = new Uri(url);
				bool flag4 = uri.Host.Equals(WebHelper.sDefaultCloudHost.Host, StringComparison.InvariantCultureIgnoreCase) || uri.Host.Equals(WebHelper.sRegistryHost.Host, StringComparison.InvariantCultureIgnoreCase);
				if (flag4)
				{
					string registeredEmail = RegistryManager.Instance.RegisteredEmail;
					bool flag5 = !string.IsNullOrEmpty(registeredEmail);
					if (flag5)
					{
						text3 += "&email=";
					}
					text3 += registeredEmail;
					string token = RegistryManager.Instance.Token;
					bool flag6 = !string.IsNullOrEmpty(token);
					if (flag6)
					{
						text3 += "&token=";
					}
					text3 += token;
				}
			}
			text3 += "&installation_type=";
			text3 += text2;
			bool flag7 = !string.IsNullOrEmpty(pkgName);
			if (flag7)
			{
				text3 += "&gaming_pkg_name=";
			}
			text3 += pkgName;
			bool flag8 = url != null && !url.Contains("://");
			if (flag8)
			{
				url = "http://" + url;
			}
			url = HTTPUtils.MergeQueryParams(url, text3, true);
			Logger.Debug("Returning updated URL: {0}", new object[] { url });
			return url;
		}

		public static string GetHelpArticleURL(string articleKey)
		{
			return WebHelper.GetUrlWithParams(string.Format(CultureInfo.InvariantCulture, "{0}/{1}", new object[]
			{
				WebHelper.GetServerHost(),
				"help_articles"
			})) + "&article=" + articleKey;
		}

		public static Dictionary<string, string> GetCommonPOSTData()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>
			{
				{ "oem", "bgp64" },
				{
					"prod_ver",
					RegistryManager.Instance.ClientVersion
				},
				{
					"eng_ver",
					RegistryManager.Instance.Version
				},
				{
					"guid",
					RegistryManager.Instance.UserGuid
				},
				{
					"locale",
					RegistryManager.Instance.UserSelectedLocale
				},
				{
					"installation_type",
					RegistryManager.Instance.InstallationType.ToString()
				}
			};
			string partner = RegistryManager.Instance.Partner;
			string campaignMD = RegistryManager.Instance.CampaignMD5;
			string pkgName = GameConfig.Instance.PkgName;
			string registeredEmail = RegistryManager.Instance.RegisteredEmail;
			string token = RegistryManager.Instance.Token;
			bool flag = !string.IsNullOrEmpty(partner);
			if (flag)
			{
				dictionary.Add("partner", partner);
			}
			bool flag2 = !string.IsNullOrEmpty(campaignMD);
			if (flag2)
			{
				dictionary.Add("campaign_md5", campaignMD);
			}
			bool flag3 = !string.IsNullOrEmpty(registeredEmail);
			if (flag3)
			{
				dictionary.Add("email", registeredEmail);
			}
			bool flag4 = !string.IsNullOrEmpty(token);
			if (flag4)
			{
				dictionary.Add("token", token);
			}
			bool flag5 = !string.IsNullOrEmpty(pkgName);
			if (flag5)
			{
				dictionary.Add("gaming_pkg_name", pkgName);
			}
			return dictionary;
		}

		private static Uri sDefaultCloudHost = new Uri("http://127.0.0.1");

		private static Uri sRegistryHost = new Uri(RegistryManager.Instance.Host);
	}
}


