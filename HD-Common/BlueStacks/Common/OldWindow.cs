using System;
using System.Windows.Forms;

namespace BlueStacks.Common
{
	internal class OldWindow : IWin32Window
	{
		public OldWindow(IntPtr handle)
		{
			this._handle = handle;
		}

		IntPtr IWin32Window.Handle
		{
			get
			{
				return this._handle;
			}
		}

		private readonly IntPtr _handle;
	}
}


