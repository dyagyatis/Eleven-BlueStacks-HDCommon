using System;
using System.Runtime.Serialization;

namespace BlueStacks.Common.Interop
{
	[Serializable]
	public class EUUIDException : Exception
	{
		public EUUIDException()
		{
		}

		public EUUIDException(string reason)
			: base(reason)
		{
		}

		public EUUIDException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected EUUIDException(SerializationInfo serializationInfo, StreamingContext streamingContext)
			: base(serializationInfo, streamingContext)
		{
		}
	}
}


