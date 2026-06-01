using System;
using Newtonsoft.Json.Linq;

namespace BlueStacks.Common
{
	public class ThemeChangeEventArgs : BrowserEventArgs
	{
		public ThemeChangeEventArgs(BrowserControlTags tag, string vmName, JObject extraData)
			: base(tag, vmName, extraData)
		{
		}
	}
}


