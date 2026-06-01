using System;

namespace BlueStacks.Common
{
	public class Tuple<T1>
	{
		public Tuple(T1 item1)
		{
			this.Item1 = item1;
		}

		public T1 Item1 { get; set; }
	}
}


