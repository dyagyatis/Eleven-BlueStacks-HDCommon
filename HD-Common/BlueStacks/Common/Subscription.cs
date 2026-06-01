using System;

namespace BlueStacks.Common
{
	public class Subscription<T> : IDisposable
	{
		public Action<T> Action { get; private set; }

		public Subscription(Action<T> action)
		{
			this.Action = action;
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposedValue)
			{
				this.disposedValue = true;
			}
		}

		~Subscription()
		{
			this.Dispose(false);
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private bool disposedValue;
	}
}


