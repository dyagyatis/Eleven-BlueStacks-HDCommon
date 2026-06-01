using System;
using Newtonsoft.Json.Linq;

namespace BlueStacks.Common
{
	public class BootCompleteEventArgs : BrowserEventArgs
	{
		public BootCompleteEventArgs(BrowserControlTags tag, string vmName, JObject extraData)
			: base(tag, vmName, extraData)
		{
		}
	}
}


