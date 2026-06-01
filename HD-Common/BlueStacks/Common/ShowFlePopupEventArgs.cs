using System;
using Newtonsoft.Json.Linq;

namespace BlueStacks.Common
{
	public class ShowFlePopupEventArgs : BrowserEventArgs
	{
		public ShowFlePopupEventArgs(BrowserControlTags tag, string vmName, JObject extraData)
			: base(tag, vmName, extraData)
		{
		}
	}
}


