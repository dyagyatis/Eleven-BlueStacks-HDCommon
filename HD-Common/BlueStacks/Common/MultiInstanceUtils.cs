using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlueStacks.Common
{
	public static class MultiInstanceUtils
	{
		public static bool VerifyVmId(string vmId)
		{
			string[] vmList = RegistryManager.Instance.VmList;
			for (int i = 0; i < vmList.Length; i++)
			{
				if (vmList[i].Equals(vmId, StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
			}
			return false;
		}

		public static void SetDeviceCapsRegistry(string legacyReason, string engine)
		{
			MultiInstanceUtils.SetDeviceCapsRegistry(legacyReason, engine, CpuHvmState.Unknown, BiosHvmState.Unknown);
		}

		public static void SetDeviceCapsRegistry(string legacyReason, string engine, CpuHvmState cpuHvm, BiosHvmState biosHvm)
		{
			string deviceCaps = RegistryManager.Instance.DeviceCaps;
			JObject jobject = new JObject
			{
				{ "engine_enabled", engine },
				{ "legacy_reason", legacyReason },
				{
					"cpu_hvm",
					cpuHvm.ToString()
				},
				{
					"bios_hvm",
					biosHvm.ToString()
				}
			};
			JObject jobject2 = JObject.Parse(deviceCaps);
			if (!string.IsNullOrEmpty(deviceCaps))
			{
				string text = jobject2["engine_enabled"].ToString();
				Logger.Info("Old engine was {0}", new object[] { text });
				if (!text.Equals(engine, StringComparison.OrdinalIgnoreCase))
				{
					RegistryManager.Instance.SystemInfoStats2 = 1;
				}
			}
			RegistryManager.Instance.CurrentEngine = engine;
			RegistryManager.Instance.DeviceCaps = jobject.ToString(Formatting.None, new JsonConverter[0]);
		}
	}
}


