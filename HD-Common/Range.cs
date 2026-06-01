using System;

public class Range
{
	public Range(long from, long to)
	{
		this.From = from;
		this.To = to;
	}

	public long From { get; }

	public long To { get; }

	public long Length
	{
		get
		{
			return this.To - this.From + 1L;
		}
	}
}


