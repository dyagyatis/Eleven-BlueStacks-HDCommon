using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BlueStacks.Common
{
	public class SerialWorkQueue : IDisposable
	{
		public SerialWorkQueue()
			: this(null)
		{
		}

		public SerialWorkQueue(string name)
		{
			this.mQueue = new Queue<SerialWorkQueue.Work>();
			this.mLock = new object();
			this.mRunning = true;
			bool flag = name == null;
			if (flag)
			{
				name = string.Format("SerialWorkQueue.{0}", Interlocked.Increment(ref SerialWorkQueue.sAutoId));
			}
			this.mThread = new Thread(new ThreadStart(this.Run))
			{
				Name = name,
				IsBackground = true,
				Priority = ThreadPriority.Highest
			};
		}

		public SerialWorkQueue.ExceptionHandlerCallback ExceptionHandler
		{
			set
			{
				this.mExceptionHandler = value;
			}
		}

		public void Start()
		{
			bool flag = !this.mThread.IsAlive;
			if (flag)
			{
				this.mThread.Start();
			}
		}

		public void Join()
		{
			this.mThread.Join();
		}

		public void Stop()
		{
			object obj = this.mLock;
			lock (obj)
			{
				this.mRunning = false;
				Monitor.PulseAll(this.mLock);
			}
		}

		public void Enqueue(SerialWorkQueue.Work work)
		{
			bool flag = work == null;
			if (!flag)
			{
				object obj = this.mLock;
				lock (obj)
				{
					this.mQueue.Enqueue(work);
					Monitor.Pulse(this.mLock);
				}
			}
		}

		public void DispatchAsync(SerialWorkQueue.Work work)
		{
			this.Enqueue(work);
		}

		public void DispatchAfter(double delay, SerialWorkQueue.Work work)
		{
			Task.Delay((int)delay).ContinueWith(delegate(Task t)
			{
				bool flag = this.mRunning;
				if (flag)
				{
					this.Enqueue(work);
				}
			});
		}

		public void DispatchSync(SerialWorkQueue.Work work)
		{
			if (work == null) return;
			
			using (ManualResetEventSlim waitHandle = new ManualResetEventSlim(false))
			{
				this.Enqueue(delegate
				{
					try
					{
						work();
					}
					finally
					{
						waitHandle.Set();
					}
				});
				waitHandle.Wait();
			}
		}

		public bool IsCurrentWorkQueue()
		{
			return Thread.CurrentThread == this.mThread;
		}

		private void Run()
		{
			SpinWait spinWait = default(SpinWait);
			while (this.mRunning)
			{
				SerialWorkQueue.Work work = null;
				object obj = this.mLock;
				lock (obj)
				{
					bool flag2 = this.mQueue.Count > 0;
					if (flag2)
					{
						work = this.mQueue.Dequeue();
					}
					else
					{
						bool flag3 = !this.mRunning;
						if (flag3)
						{
							break;
						}
					}
				}
				bool flag4 = work != null;
				if (flag4)
				{
					try
					{
						work();
					}
					catch (Exception ex)
					{
						bool flag5 = this.mExceptionHandler != null;
						if (flag5)
						{
							this.mExceptionHandler(ex);
						}
					}
					spinWait.Reset();
				}
				else
				{
					bool flag6 = !spinWait.NextSpinWillYield;
					if (flag6)
					{
						spinWait.SpinOnce();
					}
					else
					{
						object obj2 = this.mLock;
						lock (obj2)
						{
							bool flag8 = this.mQueue.Count == 0 && this.mRunning;
							if (flag8)
							{
								Monitor.Wait(this.mLock);
							}
						}
						spinWait.Reset();
					}
				}
			}
		}

		public void Dispose()
		{
			this.Stop();
		}

		private static int sAutoId;

		private readonly Thread mThread;

		private readonly Queue<SerialWorkQueue.Work> mQueue;

		private readonly object mLock;

		private SerialWorkQueue.ExceptionHandlerCallback mExceptionHandler;

		private volatile bool mRunning;


		public delegate void Work();


		public delegate void ExceptionHandlerCallback(Exception exc);
	}
}


