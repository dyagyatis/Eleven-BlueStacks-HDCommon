using System;
using System.Runtime.Serialization;

namespace BlueStacks.Common
{
	[Serializable]
	public class UnknownErrorException : Exception
	{
		public int ErrorCode { get; set; } = 99;

		public override string Message
		{
			get
			{
				return "An exception of an unknown type has occurred.";
			}
		}

		public UnknownErrorException(Exception innerException)
			: base("", innerException)
		{
		}

		public UnknownErrorException()
		{
		}

		public UnknownErrorException(string message)
			: base(message)
		{
		}

		public UnknownErrorException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected UnknownErrorException(SerializationInfo serializationInfo, StreamingContext streamingContext)
			: base(serializationInfo, streamingContext)
		{
		}
	}
}


