using System;

namespace BlueStacks.Common
{
	public class Tuple<T1, T2, T3, T4> : Tuple<T1, T2, T3>
	{
		public Tuple(T1 item1, T2 item2, T3 item3, T4 item4)
			: base(item1, item2, item3)
		{
			this.Item4 = item4;
		}

		public T4 Item4 { get; set; }
	}
}


