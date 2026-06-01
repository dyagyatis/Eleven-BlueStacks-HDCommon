using System;
using Newtonsoft.Json.Linq;

namespace BlueStacks.Common
{
	public class OemDownloadCurrentProgressEventArgs : BrowserEventArgs
	{
		public OemDownloadCurrentProgressEventArgs(BrowserControlTags tag, string vmName, JObject extraData)
			: base(tag, vmName, extraData)
		{
		}
	}
}


