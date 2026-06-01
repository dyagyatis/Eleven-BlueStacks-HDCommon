using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace BlueStacks.Common
{
	[Serializable]
	public class DynamicOverlayConfigControls
	{
		public static DynamicOverlayConfigControls Instance
		{
			get
			{
				if (DynamicOverlayConfigControls.sInstance == null)
				{
					object obj = DynamicOverlayConfigControls.syncRoot;
					lock (obj)
					{
						if (DynamicOverlayConfigControls.sInstance == null)
						{
							DynamicOverlayConfigControls.sInstance = new DynamicOverlayConfigControls();
						}
					}
				}
				return DynamicOverlayConfigControls.sInstance;
			}
		}

		private DynamicOverlayConfigControls()
		{
		}

		public static void Init(string data)
		{
			try
			{
				DynamicOverlayConfigControls.sInstance = JsonConvert.DeserializeObject<DynamicOverlayConfigControls>(data, Utils.GetSerializerSettings());
			}
			catch (Exception ex)
			{
				Logger.Warning("Error loading dynamic overlay data. Ex: " + ex.ToString());
			}
		}

		private static volatile DynamicOverlayConfigControls sInstance;

		private static object syncRoot = new object();

		public List<DynamicOverlayConfig> GameControls = new List<DynamicOverlayConfig>();
	}
}


