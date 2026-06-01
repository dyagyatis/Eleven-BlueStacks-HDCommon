using System;
using Newtonsoft.Json.Linq;

namespace BlueStacks.Common
{
	public class GetVmInfoEventArgs : BrowserEventArgs
	{
		public GetVmInfoEventArgs(BrowserControlTags tag, string vmName, JObject extraData)
			: base(tag, vmName, extraData)
		{
		}
	}
}


