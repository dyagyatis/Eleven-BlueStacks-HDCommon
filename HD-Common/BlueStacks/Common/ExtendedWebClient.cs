using System;
using System.Net;

namespace BlueStacks.Common
{
	public class ExtendedWebClient : WebClient
	{
		public ExtendedWebClient(int timeout)
		{
			this.mTimeout = timeout;
		}

		protected override WebRequest GetWebRequest(Uri address)
		{
			WebRequest webRequest = base.GetWebRequest(address);
			webRequest.Timeout = this.mTimeout;
			return webRequest;
		}

		private int mTimeout;
	}
}


