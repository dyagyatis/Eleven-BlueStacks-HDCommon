using System;
using Newtonsoft.Json.Linq;

namespace BlueStacks.Common
{
	public class GrmAppListUpdateEventArgs : BrowserEventArgs
	{
		public GrmAppListUpdateEventArgs(BrowserControlTags tag, string vmName, JObject extraData)
			: base(tag, vmName, extraData)
		{
		}
	}
}


