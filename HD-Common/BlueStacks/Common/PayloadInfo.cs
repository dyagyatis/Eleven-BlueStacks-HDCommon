using System;
using System.Net;

namespace BlueStacks.Common
{
	public class PayloadInfo
	{
		public HttpStatusCode StatusCode { get; private set; }

		public bool SupportsRangeRequest { get; set; }

		public long Size { get; set; }

		public long FileSize { get; set; }

		public PayloadInfo(HttpStatusCode statusCode, long size, long fileSize = 0L, bool supportsRangeRequest = false)
		{
			this.SupportsRangeRequest = supportsRangeRequest;
			this.Size = size;
			this.StatusCode = statusCode;
			this.FileSize = ((fileSize == 0L) ? size : fileSize);
		}
	}
}


