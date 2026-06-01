using System;

namespace BlueStacks.Common
{
	public static class MultiInstanceStrings
	{
		public static string VmName
		{
			get
			{
				if (MultiInstanceStrings.sVmName == null)
				{
					return "Android";
				}
				return MultiInstanceStrings.sVmName;
			}
			set
			{
				if (MultiInstanceStrings.sVmName != null)
				{
					throw new Exception("VmName can be set only once");
				}
				MultiInstanceStrings.sVmName = value;
				if (MultiInstanceStrings.sVmName == "Android")
				{
					MultiInstanceStrings.sVmId = 0;
					return;
				}
				string text = MultiInstanceStrings.sVmName;
				if (!int.TryParse((text != null) ? text.Split(new char[] { '_' })[1] : null, out MultiInstanceStrings.sVmId))
				{
					throw new Exception("Invalid VM: " + MultiInstanceStrings.sVmName);
				}
			}
		}

		public static ushort BstServerPort
		{
			get
			{
				return (ushort)RegistryManager.Instance.Guest[MultiInstanceStrings.VmName].BstAndroidPort;
			}
			set
			{
				RegistryManager.Instance.Guest[MultiInstanceStrings.VmName].BstAndroidPort = (int)value;
			}
		}

		public static int VmId
		{
			get
			{
				return MultiInstanceStrings.sVmId;
			}
		}

		private static string sVmName;

		private static int sVmId;
	}
}


