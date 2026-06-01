using System;
using Newtonsoft.Json.Linq;

namespace BlueStacks.Common
{
	public class ApkInstallStartedEventArgs : BrowserEventArgs
	{
		public ApkInstallStartedEventArgs(BrowserControlTags tag, string vmName, JObject extraData)
			: base(tag, vmName, extraData)
		{
		}
	}
}


