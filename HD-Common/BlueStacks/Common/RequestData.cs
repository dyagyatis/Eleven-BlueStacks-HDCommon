using System;
using System.Collections.Specialized;

namespace BlueStacks.Common
{
	public class RequestData
	{
		public NameValueCollection Headers { get; set; }

		public NameValueCollection QueryString { get; set; }

		public NameValueCollection Data { get; set; }

		public NameValueCollection Files { get; set; }

		public string RequestVmName { get; set; }

		public int RequestVmId { get; set; }

		public string Oem { get; set; }

		public RequestData()
		{
			this.Headers = new NameValueCollection();
			this.QueryString = new NameValueCollection();
			this.Data = new NameValueCollection();
			this.Files = new NameValueCollection();
		}
	}
}


