using System;
using Newtonsoft.Json.Linq;

namespace BlueStacks.Common
{
	public class ApkDownloadCurrentProgressEventArgs : BrowserEventArgs
	{
		public ApkDownloadCurrentProgressEventArgs(BrowserControlTags tag, string vmName, JObject extraData)
			: base(tag, vmName, extraData)
		{
		}
	}
}


