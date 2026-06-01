using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;
using Newtonsoft.Json.Linq;

namespace BlueStacks.Common
{
	public static class MemoryManager
	{
		[DllImport("psapi.dll")]
		private static extern int EmptyWorkingSet(IntPtr hwProc);

		public static void TrimMemory(bool isForceMemoryTrim = false)
		{
			if (isForceMemoryTrim)
			{
				MemoryManager.PerformTrim();
				return;
			}
			Thread thread = MemoryManager.s_trimMemoryThread;
			bool flag = thread != null && thread.IsAlive;
			if (!flag)
			{
				object obj = MemoryManager.s_trimLock;
				lock (obj)
				{
					Thread thread2 = MemoryManager.s_trimMemoryThread;
					bool flag3 = thread2 != null && thread2.IsAlive;
					if (!flag3)
					{
						bool flag4 = !RegistryManager.Instance.EnableMemoryTrim;
						if (!flag4)
						{
							MemoryManager.s_trimMemoryThread = new Thread(new ThreadStart(delegate
							{
								int num = RegistryManager.Instance.TrimMemoryDuration * 1000;
								Logger.Info(string.Format("Memory trim task started. Interval: {0} ms.", num));
								for (;;)
								{
									Thread.Sleep(num);
									try
									{
										bool enableMemoryTrim = RegistryManager.Instance.EnableMemoryTrim;
										if (enableMemoryTrim)
										{
											MemoryManager.PerformTrim();
										}
									}
									catch (Exception ex)
									{
										Logger.Error(string.Format("Exception in Memory Trim thread: {0}", ex));
									}
								}
							}));
							MemoryManager.s_trimMemoryThread.IsBackground = true;
							MemoryManager.s_trimMemoryThread.Name = "MemoryTrimmerThread";
							MemoryManager.s_trimMemoryThread.Start();
						}
					}
				}
			}
		}

		public static void CheckAndTrimAndroidMemory()
		{
			Thread thread = MemoryManager.s_androidTrimMemoryThread;
			bool flag = thread != null && thread.IsAlive;
			if (!flag)
			{
				object obj = MemoryManager.s_androidTrimLock;
				lock (obj)
				{
					Thread thread2 = MemoryManager.s_androidTrimMemoryThread;
					bool flag3 = thread2 != null && thread2.IsAlive;
					if (!flag3)
					{
						bool flag4 = !RegistryManager.Instance.EnableMemoryTrim;
						if (!flag4)
						{
							MemoryManager.s_androidTrimMemoryThread = new Thread(new ThreadStart(delegate
							{
								int triggerMemoryTrimTimerInterval = RegistryManager.Instance.DefaultGuest.TriggerMemoryTrimTimerInterval;
								int triggerMemoryTrimThreshold = RegistryManager.Instance.DefaultGuest.TriggerMemoryTrimThreshold;
								long num = (long)triggerMemoryTrimThreshold * 1024L * 1024L;
								Logger.Info(string.Format("Android memory check task started. Interval: {0} ms, Threshold: {1} MB.", triggerMemoryTrimTimerInterval, triggerMemoryTrimThreshold));
								for (;;)
								{
									Thread.Sleep(triggerMemoryTrimTimerInterval);
									bool flag5 = !RegistryManager.Instance.EnableMemoryTrim;
									if (flag5)
									{
										break;
									}
									long workingSet = Process.GetCurrentProcess().WorkingSet64;
									bool flag6 = workingSet > num;
									if (flag6)
									{
										Logger.Info(string.Format("Current Process Working Set ({0} MB) exceeds threshold ({1} MB). Triggering Android trim.", workingSet / 1048576L, triggerMemoryTrimThreshold));
										MemoryManager.TriggerMemoryTrimInAndroid();
									}
								}
							}));
							MemoryManager.s_androidTrimMemoryThread.IsBackground = true;
							MemoryManager.s_androidTrimMemoryThread.Name = "AndroidMemoryCheckerThread";
							MemoryManager.s_androidTrimMemoryThread.Start();
						}
					}
				}
			}
		}

		private static void TriggerMemoryTrimInAndroid()
		{
			try
			{
				int bstCommandProcessorPort = Utils.GetBstCommandProcessorPort(MultiInstanceStrings.VmName);
				string text = "triggerMemoryTrim";
				string text2 = string.Format(CultureInfo.InvariantCulture, "http://127.0.0.1:{0}/{1}", bstCommandProcessorPort, text);
				Logger.Info("Sending request to: " + text2);
				string text3 = BstHttpClient.Get(text2, null, false, MultiInstanceStrings.VmName, 0, 1, 0, false, "bgp64");
				bool flag = !string.IsNullOrEmpty(text3);
				if (flag)
				{
					JToken jtoken = JObject.Parse(text3)["result"];
					string text4 = ((jtoken != null) ? jtoken.ToString() : null) ?? "N/A";
					Logger.Info("Response from " + text + ": " + text4);
				}
			}
			catch (Exception ex)
			{
				Logger.Error(string.Format("Exception when calling triggerMemoryTrim API: {0}", ex));
			}
		}

		[DllImport("KERNEL32.DLL", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		private static extern bool SetProcessWorkingSetSize(IntPtr pProcess, IntPtr dwMinimumWorkingSetSize, IntPtr dwMaximumWorkingSetSize);

		private static void PerformTrim()
		{
			try
			{
				GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
				GC.WaitForPendingFinalizers();
				bool flag = Environment.OSVersion.Platform == PlatformID.Win32NT;
				if (flag)
				{
					MemoryManager.SetProcessWorkingSetSize(Process.GetCurrentProcess().Handle, (IntPtr)(-1), (IntPtr)(-1));
				}
				using (Process currentProcess = Process.GetCurrentProcess())
				{
					Logger.Debug("Trimming memory working set.");
					MemoryManager.EmptyWorkingSet(currentProcess.Handle);
				}
			}
			catch (Exception ex)
			{
				Logger.Error(string.Format("Exception during memory trim: {0}", ex));
			}
		}

		private static Thread s_trimMemoryThread;

		private static readonly object s_trimLock = new object();

		private static Thread s_androidTrimMemoryThread;

		private static readonly object s_androidTrimLock = new object();
	}
}


