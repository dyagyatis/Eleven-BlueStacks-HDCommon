using System;
using Newtonsoft.Json.Linq;

namespace BlueStacks.Common
{
	public class GoogleSignInCompleteEventArgs : BrowserEventArgs
	{
		public GoogleSignInCompleteEventArgs(BrowserControlTags tag, string vmName, JObject extraData)
			: base(tag, vmName, extraData)
		{
		}
	}
}


