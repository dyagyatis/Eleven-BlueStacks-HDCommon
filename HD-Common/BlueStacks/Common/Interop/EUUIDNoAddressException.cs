using System;
using System.Runtime.Serialization;

namespace BlueStacks.Common.Interop
{
	[Serializable]
	public class EUUIDNoAddressException : EUUIDException
	{
		public EUUIDNoAddressException()
		{
		}

		public EUUIDNoAddressException(string message)
			: base(message)
		{
		}

		public EUUIDNoAddressException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected EUUIDNoAddressException(SerializationInfo serializationInfo, StreamingContext streamingContext)
			: base(serializationInfo, streamingContext)
		{
		}
	}
}


