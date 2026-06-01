using System;
using Newtonsoft.Json.Linq;

namespace BlueStacks.Common
{
	public class AppInfo
	{
		public string Name { get; set; }

		public string Img { get; set; }

		public string Package { get; set; }

		public string Activity { get; set; }

		public string System { get; set; }

		public string Url { get; set; }

		public string Appstore { get; set; }

		public string Version { get; set; }

		public string VersionName { get; set; } = "Unknown";

		public bool Gl3Required { get; set; }

		public bool VideoPresent { get; set; }

		public bool IsGamepadCompatible { get; set; }

		public AppInfo()
		{
		}

		public AppInfo(JObject app)
		{
			this.Name = ((app != null) ? app["name"].ToString() : null);
			this.Img = app["img"].ToString();
			this.Package = app["package"].ToString();
			this.Activity = app["activity"].ToString();
			this.System = app["system"].ToString();
			this.Url = (app.ContainsKey("url") ? app["url"].ToString() : null);
			this.Appstore = (app.ContainsKey("appstore") ? app["appstore"].ToString() : "Unknown");
			this.Version = (app.ContainsKey("version") ? app["version"].ToString() : "Unknown");
			this.Gl3Required = app.ContainsKey("gl3required") && app["gl3required"].ToObject<bool>();
			this.VideoPresent = app.ContainsKey("videopresent") && app["videopresent"].ToObject<bool>();
			this.IsGamepadCompatible = app.ContainsKey("isgamepadcompatible") && app["isgamepadcompatible"].ToObject<bool>();
			if (app.ContainsKey("versionName"))
			{
				this.VersionName = app["versionName"].ToString();
			}
		}

		public AppInfo(string InName, string InImage, string InPackage, string InActivity, string InSystem, string InAppStore, string InVersion, bool InGl3required, bool InVideoPresent, string appVersionName, bool isGamepadCompatible = false)
		{
			this.Name = InName;
			this.Img = InImage;
			this.Package = InPackage;
			this.Activity = InActivity;
			this.System = InSystem;
			this.Url = null;
			this.Appstore = InAppStore;
			this.Version = InVersion;
			this.Gl3Required = InGl3required;
			this.VideoPresent = InVideoPresent;
			this.VersionName = appVersionName;
			this.IsGamepadCompatible = isGamepadCompatible;
		}
	}
}


