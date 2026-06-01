using System;
using BlueStacks.Common;

[Serializable]
public class ShortcutKeys : ShortcutConfig
{
	public string ShortcutCategory { get; set; }

	public string ShortcutName { get; set; }

	public string ShortcutKey { get; set; }

	public bool ReadOnlyTextbox { get; set; }
}


