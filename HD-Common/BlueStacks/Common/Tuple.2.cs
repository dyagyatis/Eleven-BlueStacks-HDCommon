using System;

namespace BlueStacks.Common
{
	public class Tuple<T1, T2> : Tuple<T1>
	{
		public Tuple(T1 item1, T2 item2)
			: base(item1)
		{
			this.Item2 = item2;
		}

		public T2 Item2 { get; set; }
	}
}


