using System;
using System.Runtime.Serialization;

namespace BlueStacks.Common
{
	[Serializable]
	public class ConnectFailureException : Exception
	{
		public int ErrorCode { get; set; } = 1;

		public override string Message
		{
			get
			{
				return "The remote service point could not be contacted.";
			}
		}

		public ConnectFailureException(Exception innerException)
			: base("", innerException)
		{
		}

		public ConnectFailureException()
		{
		}

		public ConnectFailureException(string message)
			: base(message)
		{
		}

		public ConnectFailureException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected ConnectFailureException(SerializationInfo serializationInfo, StreamingContext streamingContext)
			: base(serializationInfo, streamingContext)
		{
		}
	}
}


