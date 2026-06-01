using System;
using System.Windows.Controls;
using System.Windows.Forms;

namespace BlueStacks.Common
{
	public static class UIHelper
	{
		public static void SetDispatcher(UIHelper.dispatcher gameManagerWindowDispatcher)
		{
			UIHelper.obj = gameManagerWindowDispatcher;
		}

		public static void RunOnUIThread(global::System.Windows.Forms.Control control, UIHelper.Action action)
		{
			if (UIHelper.obj == null)
			{
				if (control != null && control.InvokeRequired)
				{
					control.Invoke(action);
					return;
				}
				if (action != null)
				{
					action();
					return;
				}
			}
			else if (control != null && control.InvokeRequired)
			{
				UIHelper.obj(action, new object[0]);
			}
		}

		public static void AssertUIThread(global::System.Windows.Forms.Control control)
		{
			if (control != null && control.InvokeRequired)
			{
				throw new ApplicationException("Not running on UI thread");
			}
		}

		public static void AssertUIThread(global::System.Windows.Controls.Control control)
		{
			if (control != null && !control.Dispatcher.CheckAccess())
			{
				throw new ApplicationException("Not running on UI thread");
			}
		}

		private static UIHelper.dispatcher obj;


		public delegate object dispatcher(Delegate method, params object[] args);

		public delegate void Action();
	}
}


