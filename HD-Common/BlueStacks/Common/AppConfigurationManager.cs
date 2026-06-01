using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace BlueStacks.Common
{
	[Serializable]
	public class AppConfigurationManager
	{
		public static AppConfigurationManager Instance
		{
			get
			{
				if (AppConfigurationManager.sInstance == null)
				{
					object obj = AppConfigurationManager.syncRoot;
					lock (obj)
					{
						if (AppConfigurationManager.sInstance == null)
						{
							AppConfigurationManager.Init();
						}
					}
				}
				return AppConfigurationManager.sInstance;
			}
		}

		private AppConfigurationManager()
		{
		}

		private static void Init()
		{
			try
			{
				AppConfigurationManager.sInstance = JsonConvert.DeserializeObject<AppConfigurationManager>(RegistryManager.Instance.AppConfiguration, Utils.GetSerializerSettings());
			}
			catch (Exception ex)
			{
				Logger.Warning("Error loading app configurations. Ex: " + ex.ToString());
			}
		}

		public static void Save()
		{
			if (AppConfigurationManager.sInstance != null)
			{
				RegistryManager.Instance.AppConfiguration = JsonConvert.SerializeObject(AppConfigurationManager.sInstance, Utils.GetSerializerSettings());
			}
		}

		[JsonProperty("VmAppConfig", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Ignore)]
		public Dictionary<string, Dictionary<string, AppSettings>> VmAppConfig { get; } = new Dictionary<string, Dictionary<string, AppSettings>>();

		public bool CheckIfTrueInAnyVm(string package, Predicate<AppSettings> rule)
		{
			foreach (Dictionary<string, AppSettings> dictionary in this.VmAppConfig.Values)
			{
				if (dictionary.ContainsKey(package) && rule(dictionary[package]))
				{
					return true;
				}
			}
			return false;
		}

		private static volatile AppConfigurationManager sInstance;

		private static object syncRoot = new object();
	}
}


