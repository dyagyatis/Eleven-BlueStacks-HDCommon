using System;
using System.Net;
using System.Runtime.Serialization;

namespace BlueStacks.Common
{
	[Serializable]
	public class NonResumableStreamException : Exception
	{
		public int ErrorCode { get; set; } = 5;

		public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;

		public override string Message
		{
			get
			{
				return "The remote server does not support partial content stream.";
			}
		}

		public NonResumableStreamException(HttpStatusCode statusCode)
		{
			this.StatusCode = statusCode;
		}

		public NonResumableStreamException()
		{
		}

		public NonResumableStreamException(string message)
			: base(message)
		{
		}

		public NonResumableStreamException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected NonResumableStreamException(SerializationInfo serializationInfo, StreamingContext streamingContext)
			: base(serializationInfo, streamingContext)
		{
		}
	}
}


