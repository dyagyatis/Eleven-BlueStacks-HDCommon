using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace BlueStacks.Common
{
	public class CustomWindow : Window
	{
		public bool IsDraggable { get; set; } = true;

		public bool IsClosed { get; private set; }

		public virtual bool ShowWithParentWindow
		{
			get
			{
				return this.mShowWithParentWindow;
			}
			set
			{
				this.mShowWithParentWindow = value;
			}
		}

		public bool IsShowGLWindow { get; set; }

		public CustomWindow()
		{
			base.FontFamily = new FontFamily(AppDomain.CurrentDomain.BaseDirectory + "Assets/#Greycliff CF Bold");
			this.SetWindowTitle();
			base.SourceInitialized += this.CustomWindow_SourceInitialized;
		}

		private void SetWindowTitle()
		{
			base.Title = base.GetType().Name;
		}

		private void CustomWindow_SourceInitialized(object sender, EventArgs e)
		{
			RenderHelper.ChangeRenderModeToSoftware(sender);
		}

		protected override void OnSourceInitialized(EventArgs e)
		{
			base.OnSourceInitialized(e);
			HwndSource hwndSource = PresentationSource.FromVisual(this) as HwndSource;
			if (hwndSource != null)
			{
				hwndSource.AddHook(new HwndSourceHook(CustomWindow.WndProc));
			}
		}

		private static IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			if (msg == 260 && (wParam == (IntPtr)18 || wParam == (IntPtr)121))
			{
				handled = true;
			}
			if (msg == 262 && wParam == (IntPtr)32)
			{
				handled = true;
			}
			return IntPtr.Zero;
		}

		protected override void OnClosed(EventArgs e)
		{
			this.IsClosed = true;
			base.OnClosed(e);
		}

		private bool mShowWithParentWindow;
	}
}


