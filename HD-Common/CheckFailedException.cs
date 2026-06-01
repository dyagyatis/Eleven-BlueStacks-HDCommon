using System;
using System.Runtime.Serialization;

[Serializable]
public class CheckFailedException : Exception
{
	public CheckFailedException()
	{
	}

	public CheckFailedException(string message)
		: base(message)
	{
	}

	public CheckFailedException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	protected CheckFailedException(SerializationInfo serializationInfo, StreamingContext streamingContext)
		: base(serializationInfo, streamingContext)
	{
	}
}


