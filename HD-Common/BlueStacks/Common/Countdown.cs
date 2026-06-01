using System;
using System.Threading;

namespace BlueStacks.Common
{
	public class Countdown
	{
		public Countdown()
		{
		}

		public Countdown(int initialCount)
		{
			this._value = initialCount;
		}

		public void Signal()
		{
			this.AddCount(-1);
		}

		public void AddCount(int amount)
		{
			object locker = this._locker;
			lock (locker)
			{
				this._value += amount;
				if (this._value <= 0)
				{
					Monitor.PulseAll(this._locker);
				}
			}
		}

		public void Wait()
		{
			object locker = this._locker;
			lock (locker)
			{
				while (this._value > 0)
				{
					Monitor.Wait(this._locker);
				}
			}
		}

		private object _locker = new object();

		private int _value;
	}
}


