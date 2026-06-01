using System;
using System.Collections.Generic;
using System.Windows.Input;
using Newtonsoft.Json;

namespace BlueStacks.Common
{
	[Serializable]
	public class ShortcutConfig
	{
		public List<ShortcutKeys> Shortcut { get; set; }

		public string DefaultModifier { get; set; } = IMAPKeys.GetStringForFile(Key.LeftCtrl) + "," + IMAPKeys.GetStringForFile(Key.LeftShift);

		public static ShortcutConfig LoadShortcutsConfig()
		{
			try
			{
				string text = string.Empty;
				if (!string.IsNullOrEmpty(RegistryManager.Instance.UserDefinedShortcuts))
				{
					text = RegistryManager.Instance.UserDefinedShortcuts;
				}
				else
				{
					if (!string.IsNullOrEmpty(text) || string.IsNullOrEmpty(RegistryManager.Instance.DefaultShortcuts))
					{
						throw new Exception("Shortcuts registry entry not found.");
					}
					text = RegistryManager.Instance.DefaultShortcuts;
				}
				return JsonConvert.DeserializeObject<ShortcutConfig>(text, Utils.GetSerializerSettings());
			}
			catch (Exception ex)
			{
				Logger.Error("SHORTCUT: Exception in loading shortcuts config: " + ex.ToString());
			}
			return null;
		}
	}
}


