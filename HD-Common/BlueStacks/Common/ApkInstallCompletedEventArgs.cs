using System;
using Newtonsoft.Json.Linq;

namespace BlueStacks.Common
{
	public class ApkInstallCompletedEventArgs : BrowserEventArgs
	{
		public ApkInstallCompletedEventArgs(BrowserControlTags tag, string vmName, JObject extraData)
			: base(tag, vmName, extraData)
		{
		}
	}
}


