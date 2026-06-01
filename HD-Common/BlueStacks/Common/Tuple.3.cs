using System;

namespace BlueStacks.Common
{
	public class Tuple<T1, T2, T3> : Tuple<T1, T2>
	{
		public Tuple(T1 item1, T2 item2, T3 item3)
			: base(item1, item2)
		{
			this.Item3 = item3;
		}

		public T3 Item3 { get; set; }
	}
}


