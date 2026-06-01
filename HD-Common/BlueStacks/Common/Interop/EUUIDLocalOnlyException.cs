using System;
using System.Runtime.Serialization;

namespace BlueStacks.Common.Interop
{
	[Serializable]
	public class EUUIDLocalOnlyException : EUUIDException
	{
		public EUUIDLocalOnlyException()
		{
		}

		public EUUIDLocalOnlyException(string message)
			: base(message)
		{
		}

		public EUUIDLocalOnlyException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected EUUIDLocalOnlyException(SerializationInfo serializationInfo, StreamingContext streamingContext)
			: base(serializationInfo, streamingContext)
		{
		}
	}
}


