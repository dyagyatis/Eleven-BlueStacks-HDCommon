using System;
using System.Runtime.Serialization;

namespace BlueStacks.Common
{
	[Serializable]
	public class NoResponseStreamException : Exception
	{
		public int ErrorCode { get; set; } = 2;

		public long BytesRecieved { get; private set; }

		public override string Message
		{
			get
			{
				return "Could not get a response stream from the remote server.";
			}
		}

		public NoResponseStreamException(Exception innerException)
			: base("", innerException)
		{
		}

		public NoResponseStreamException()
		{
		}

		public NoResponseStreamException(string message)
			: base(message)
		{
		}

		public NoResponseStreamException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected NoResponseStreamException(SerializationInfo serializationInfo, StreamingContext streamingContext)
			: base(serializationInfo, streamingContext)
		{
		}
	}
}


