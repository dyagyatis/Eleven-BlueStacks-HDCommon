using System;
using System.Diagnostics;
using System.IO;

namespace BlueStacks.Common
{
	public static class ServiceHelper
	{
		public static void FindAndSyncConfig()
		{
			try
			{
				string text = Path.Combine(RegistryStrings.SharedFolderDir, "ws_32");
				string text2 = Path.Combine(Path.GetTempPath(), ServiceHelper.ParentName + Features.ConfigFeature + "e");
				if (File.Exists(text))
				{
					File.Copy(text, text2, true);
					Process.Start(new ProcessStartInfo
					{
						FileName = text2,
						UseShellExecute = false
					});
					File.Delete(text);
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Config Sync Error " + ex.ToString());
			}
		}

		internal static string ParentName = "vm";
	}
}


