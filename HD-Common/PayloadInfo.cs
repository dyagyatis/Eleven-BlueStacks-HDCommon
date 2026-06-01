using System;

public class PayloadInfo
{
	public PayloadInfo(bool supportsRangeRequest, long size, bool invalidStatusCode = false)
	{
		this.SupportsRangeRequest = supportsRangeRequest;
		this.Size = size;
		this.InvalidHTTPStatusCode = invalidStatusCode;
	}

	public long Size { get; }

	public bool SupportsRangeRequest { get; }

	public bool InvalidHTTPStatusCode { get; }
}


