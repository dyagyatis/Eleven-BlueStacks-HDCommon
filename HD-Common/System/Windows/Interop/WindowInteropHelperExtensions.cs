using System;
using System.Globalization;
using System.Reflection;

namespace System.Windows.Interop
{
	public static class WindowInteropHelperExtensions
	{
		public static IntPtr EnsureHandle(this WindowInteropHelper helper)
		{
			if (helper == null)
			{
				throw new ArgumentNullException("helper");
			}
			if (helper.Handle == IntPtr.Zero)
			{
				Window window = (Window)typeof(WindowInteropHelper).InvokeMember("_window", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField, null, helper, null, CultureInfo.InvariantCulture);
				try
				{
					typeof(Window).InvokeMember("SafeCreateWindow", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.InvokeMethod, null, window, null, CultureInfo.InvariantCulture);
				}
				catch (MissingMethodException)
				{
					typeof(Window).InvokeMember("CreateSourceWindow", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.InvokeMethod, null, window, new object[] { false }, CultureInfo.InvariantCulture);
				}
			}
			return helper.Handle;
		}
	}
}


