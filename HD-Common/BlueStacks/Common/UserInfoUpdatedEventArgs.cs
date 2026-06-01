using System;
using Newtonsoft.Json.Linq;

namespace BlueStacks.Common
{
	public class UserInfoUpdatedEventArgs : BrowserEventArgs
	{
		public UserInfoUpdatedEventArgs(BrowserControlTags tag, string vmName, JObject extraData)
			: base(tag, vmName, extraData)
		{
		}
	}
}


