using System;
using System.Runtime.Serialization;

namespace BlueStacks.Common
{
	[Serializable]
	public class ENoPortAvailableException : Exception
	{
		public ENoPortAvailableException(string reason)
			: base(reason)
		{
		}

		public ENoPortAvailableException()
		{
		}

		public ENoPortAvailableException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected ENoPortAvailableException(SerializationInfo serializationInfo, StreamingContext streamingContext)
			: base(serializationInfo, streamingContext)
		{
		}
	}
}


