using System;
using Newtonsoft.Json.Linq;

namespace BlueStacks.Common
{
	public class AppPlayerClosingEventArgs : BrowserEventArgs
	{
		public AppPlayerClosingEventArgs(BrowserControlTags tag, string vmName, JObject extraData)
			: base(tag, vmName, extraData)
		{
		}
	}
}


