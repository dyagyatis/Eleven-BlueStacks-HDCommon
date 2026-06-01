using System;
using System.Runtime.Serialization;

[Serializable]
public class WorkerException : Exception
{
	public WorkerException(string msg, Exception e)
		: base(msg, e)
	{
	}

	public WorkerException()
	{
	}

	public WorkerException(string message)
		: base(message)
	{
	}

	protected WorkerException(SerializationInfo serializationInfo, StreamingContext streamingContext)
		: base(serializationInfo, streamingContext)
	{
	}
}


