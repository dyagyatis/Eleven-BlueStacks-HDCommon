using System;
using Newtonsoft.Json.Linq;

namespace BlueStacks.Common
{
	public class ApkDownloadCompletedEventArgs : BrowserEventArgs
	{
		public ApkDownloadCompletedEventArgs(BrowserControlTags tag, string vmName, JObject extraData)
			: base(tag, vmName, extraData)
		{
		}
	}
}


