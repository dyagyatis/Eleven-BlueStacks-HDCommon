using System;
using System.Runtime.Serialization;

namespace BlueStacks.Common
{
	[Serializable]
	public class TimeoutException : Exception
	{
		public int ErrorCode { get; set; } = 3;

		public override string Message
		{
			get
			{
				return "No response was received during the time-out period for the request.";
			}
		}

		public TimeoutException()
		{
		}

		public TimeoutException(string message)
			: base(message)
		{
		}

		public TimeoutException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected TimeoutException(SerializationInfo serializationInfo, StreamingContext streamingContext)
			: base(serializationInfo, streamingContext)
		{
		}
	}
}


