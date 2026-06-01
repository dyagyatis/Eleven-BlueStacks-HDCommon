using System;
using System.Runtime.InteropServices;

namespace BlueStacks.Common
{
	public static class ConsoleControl
	{
		[DllImport("Kernel32")]
		private static extern bool SetConsoleCtrlHandler(ConsoleControl.Handler handler, bool Add);

		public static void SetHandler(ConsoleControl.Handler handler)
		{
			ConsoleControl.SetConsoleCtrlHandler(handler, true);
		}


		public delegate bool Handler(CtrlType ctrlType);
	}
}


