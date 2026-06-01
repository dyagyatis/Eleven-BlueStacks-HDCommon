using System;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using System.Windows.Interop;

namespace BlueStacks.Common
{
	public static class InteropWindow
	{
		[DllImport("user32.dll")]
		public static extern uint MapVirtualKey(uint uCode, uint uMapType);

		[DllImport("user32.dll")]
		public static extern IntPtr GetKeyboardLayout(uint idThread);

		[DllImport("user32.dll")]
		public static extern int ToUnicodeEx(uint wVirtKey, uint wScanCode, byte[] lpKeyState, [MarshalAs(UnmanagedType.LPWStr)] [Out] StringBuilder pwszBuff, int cchBuff, uint wFlags, IntPtr dwhkl);

		public static IntPtr GetHwnd(Popup popup)
		{
			if (popup == null || popup.Child == null)
				return IntPtr.Zero;
			HwndSource src = (HwndSource)PresentationSource.FromVisual(popup.Child);
			return (src != null) ? src.Handle : IntPtr.Zero;
		}

		[DllImport("user32.dll")]
		public static extern IntPtr SetFocus(IntPtr hWnd);

		[DllImport("imm32.dll")]
		public static extern bool ImmSetOpenStatus(IntPtr hIMC, bool open);

		[DllImport("Imm32.dll", CharSet = CharSet.Auto)]
		public static extern IntPtr ImmGetContext(IntPtr hWnd);

		[DllImport("Imm32.dll")]
		public static extern bool ImmReleaseContext(IntPtr hWnd, IntPtr hIMC);

		[DllImport("Imm32.dll", CharSet = CharSet.Unicode)]
		private static extern int ImmGetCompositionStringW(IntPtr hIMC, int dwIndex, byte[] lpBuf, int dwBufLen);

		[DllImport("imm32.dll")]
		public static extern bool ImmSetCompositionWindow(IntPtr hIMC, out COMPOSITIONFORM lpptPos);

		[DllImport("user32.dll")]
		public static extern int GetSystemMetrics(int which);

		[DllImport("user32.dll")]
		public static extern bool SetWindowPos(IntPtr hwnd, IntPtr hwndInsertAfter, int x, int y, int w, int h, uint flags);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool AdjustWindowRect(out RECT lpRect, int dwStyle, bool bMenu);

		[DllImport("user32.dll")]
		public static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

		[DllImport("gdi32.dll")]
		private static extern IntPtr CreateDC(string driver, string name, string output, IntPtr mode);

		[DllImport("gdi32.dll")]
		private static extern bool DeleteDC(IntPtr hdc);

		[DllImport("gdi32.dll")]
		private static extern int GetDeviceCaps(IntPtr hdc, int index);

		public static int ScreenWidth => GetSystemMetrics(0);
		public static int ScreenHeight => GetSystemMetrics(1);

		[DllImport("user32.dll")]
		public static extern bool HideCaret(IntPtr hWnd);

		[DllImport("user32.dll")]
		public static extern int SendMessage(IntPtr hWnd, int wMsg, bool wParam, int lParam);

		[DllImport("user32.dll")]
		public static extern int SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

		[DllImport("user32.dll")]
		public static extern int SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, ref COPYGAMEPADDATASTRUCT cds);

		[DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern IntPtr FindWindow(string cls, string name);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool SetForegroundWindow(IntPtr handle);

		[DllImport("user32.dll")]
		public static extern bool ShowWindow(IntPtr handle, int cmd);

		[DllImport("user32.dll", ExactSpelling = true)]
		public static extern IntPtr GetAncestor(IntPtr hwnd, GetAncestorFlags flags);

		[DllImport("user32.dll")]
		public static extern IntPtr GetParent(IntPtr handle);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

		[DllImport("user32.dll")]
		public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

		[DllImport("kernel32.dll")]
		public static extern bool FreeConsole();

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool IsWindowVisible(IntPtr hWnd);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool IsWindow(IntPtr hWnd);

		[DllImport("user32.dll")]
		public static extern IntPtr GetForegroundWindow();

		[DllImport("user32.dll")]
		public static extern uint GetWindowThreadProcessId(IntPtr hWnd, ref uint ProcessId);

		[DllImport("kernel32.dll")]
		public static extern uint GetCurrentThreadId();

		[DllImport("user32.dll")]
		public static extern bool AttachThreadInput(uint idAttach, uint idAttachTo, bool fAttach);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

		[DllImport("user32.dll")]
		public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern int GetWindowTextLength(IntPtr hWnd);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

		[DllImport("user32.dll")]
		public static extern bool GetWindowRect(IntPtr hwnd, ref RECT rect);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern bool EnableWindow(IntPtr hwnd, bool enable);

		public static IntPtr MinimizeWindow(string name)
		{
			IntPtr hwnd = FindWindow(null, name);
			if (hwnd == IntPtr.Zero)
				throw new SystemException("Cannot find window '" + name + "'", new Win32Exception(Marshal.GetLastWin32Error()));
			ShowWindow(hwnd, 6);
			return hwnd;
		}


		public enum GetAncestorFlags
		{
			GetParent = 1,
			GetRoot = 2,
			GetRootOwner = 3
		}

		public const uint WS_DISABLED = 0x08000000;
		public const uint WS_POPUP = 0x80000000;
		public const int WM_INPUT = 0x00FF;
		public const int WM_MOUSEMOVE = 0x0200;
		public const int WM_NCHITTEST = 0x0084;
		public const int WM_USER = 0x0400;
		public const int WM_SETREDRAW = 0x000B;
		public const int GWL_EXSTYLE = -20;
		public const int GWL_STYLE = -16;
		public const int SW_SHOWMAXIMIZED = 3;
		public const int SW_RESTORE = 9;
		public const int SW_MINIMIZE = 6;
		public const int SW_SHOW = 5;
		public const int SW_HIDE = 0;
		public static readonly IntPtr HWND_TOP = IntPtr.Zero;
		public const int WS_THICKFRAME = 0x00040000;
		public const int WS_CAPTION = 0x00C00000;
		public const int WS_OVERLAPPEDWINDOW = 0x00CF0000;
		public const int SWP_NOREDRAW = 0x0008;
		public const int SWP_HIDEWINDOW = 0x0080;
		public const int SWP_NOZORDER = 0x0004;
		public const int SWP_DRAWFRAME = 0x0020;
		public const int SWP_SHOWWINDOW = 0x0040;
		public const int SWP_NOACTIVATE = 0x0010;
		public const int SWP_NOSIZE = 0x0001;
		public const int SWP_NOMOVE = 0x0002;

		[DllImport("user32.dll")]
		public static extern IntPtr CreateIconIndirect(ref IconInfo iconInfo);

		[DllImport("user32.dll")]
		public static extern bool GetIconInfo(IntPtr hIcon, ref IconInfo pIconInfo);

		[DllImport("user32.dll")]
		public static extern IntPtr LoadCursorFromFile(string lpFileName);

		public static Cursor LoadCustomCursor(string path)
		{
			if (string.IsNullOrEmpty(path))
				throw new ArgumentNullException(nameof(path));
			IntPtr h = LoadCursorFromFile(path);
			if (h == IntPtr.Zero)
				throw new Win32Exception(Marshal.GetLastWin32Error());
			return new Cursor(h);
		}

		public static Cursor CreateCursor(Bitmap bmp, int xHotSpot, int yHotSpot, float scalingFactor)
		{
			if (bmp == null)
				throw new ArgumentNullException(nameof(bmp));
			int width = (int)(32f * scalingFactor);
			int height = (int)(32f * scalingFactor);
			using (Bitmap resized = new Bitmap(bmp, width, height))
			{
				IntPtr hIcon = resized.GetHicon();
				IconInfo iconInfo = new IconInfo();
				GetIconInfo(hIcon, ref iconInfo);
				iconInfo.fIcon = false;
				iconInfo.xHotspot = xHotSpot;
				iconInfo.yHotspot = yHotSpot;
				IntPtr hCursor = CreateIconIndirect(ref iconInfo);
				return new Cursor(hCursor);
			}
		}

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		private static extern long GetKeyboardLayoutName(StringBuilder pwszKLID);

		private static string GetLayoutCode()
		{
			StringBuilder sb = new StringBuilder(9);
			GetKeyboardLayoutName(sb);
			return sb.ToString();
		}

		public static string MapLayoutName(string code = null)
		{
			if (code == null)
				code = GetLayoutCode();
			switch (code)
			{
				case "00000409": return "English (United States)";
				case "00000809": return "English (United Kingdom)";
				case "0000040C": return "French";
				case "0000040A": return "Spanish";
				default: return code;
			}
		}

		public static string CurrentCompStr(IntPtr handle)
		{
			IntPtr imc = ImmGetContext(handle);
			try
			{
				int len = ImmGetCompositionStringW(imc, 8, null, 0);
				if (len <= 0) return string.Empty;
				byte[] buf = new byte[len];
				ImmGetCompositionStringW(imc, 8, buf, len);
				return Encoding.Unicode.GetString(buf).TrimEnd('\0');
			}
			finally
			{
				ImmReleaseContext(handle, imc);
			}
		}

		public static void SetFullScreen(IntPtr hwnd, int X, int Y, int cx, int cy)
		{
			if (!SetWindowPos(hwnd, HWND_TOP, X, Y, cx, cy, 0x0040))
				throw new SystemException("Cannot call SetWindowPos()", new Win32Exception(Marshal.GetLastWin32Error()));
		}

		public static int GetScreenDpi()
		{
			IntPtr hdc = CreateDC("DISPLAY", null, null, IntPtr.Zero);
			if (hdc == IntPtr.Zero) return -1;
			int dpi = GetDeviceCaps(hdc, 88);
			DeleteDC(hdc);
			return dpi > 0 ? dpi : 96;
		}

		public static bool ForceSetForegroundWindow(IntPtr h)
		{
			IntPtr foregroundWindow = GetForegroundWindow();
			if (foregroundWindow == h) return true;
			uint currentThreadId = GetCurrentThreadId();
			uint windowThreadId = 0;
			GetWindowThreadProcessId(foregroundWindow, ref windowThreadId);
			if (currentThreadId != windowThreadId)
			{
				AttachThreadInput(currentThreadId, windowThreadId, true);
				bool result = SetForegroundWindow(h);
				AttachThreadInput(currentThreadId, windowThreadId, false);
				return result;
			}
			return SetForegroundWindow(h);
		}

		public static IntPtr BringWindowToFront(string windowName, bool tryClassFirst = false, bool restoreIfMinimized = true)
		{
			if (string.IsNullOrEmpty(windowName)) return IntPtr.Zero;
			IntPtr hwnd = FindWindow(null, windowName);
			if (hwnd == IntPtr.Zero && tryClassFirst)
			{
				hwnd = FindWindow(windowName, null);
			}
			if (hwnd == IntPtr.Zero)
			{
				// try enumerating children of desktop (FindWindowEx)
				IntPtr desktop = IntPtr.Zero;
				hwnd = FindWindowEx(desktop, IntPtr.Zero, null, windowName);
			}
			if (hwnd == IntPtr.Zero) return IntPtr.Zero;
			if (restoreIfMinimized)
			{
				ShowWindow(hwnd, SW_RESTORE);
			}
			try
			{
				ForceSetForegroundWindow(hwnd);
			}
			catch
			{
				SetForegroundWindow(hwnd);
			}
			return hwnd;
		}

		public static IntPtr GetWindowHandle(Window window)
		{
			HwndSource hwndSource = (HwndSource)PresentationSource.FromVisual(window);
			if (hwndSource != null)
			{
				return hwndSource.Handle;
			}
			return IntPtr.Zero;
		}

		public static bool IsWindowTopMost(IntPtr hWnd)
		{
			return (InteropWindow.GetWindowLong(hWnd, -20) & 8) == 8;
		}

		public static Window GetTopmostOwnerWindow(Window window)
		{
			while (window.Owner != null)
			{
				window = window.Owner;
			}
			return window;
		}

		public static int GetAForegroundApplicationProcessId()
		{
			IntPtr foregroundWindow = InteropWindow.GetForegroundWindow();
			if (foregroundWindow == IntPtr.Zero)
			{
				return 0;
			}
			uint num = 0U;
			InteropWindow.GetWindowThreadProcessId(foregroundWindow, ref num);
			return (int)num;
		}

		public static void RemoveWindowFromAltTabUI(IntPtr handle)
		{
			int windowLong = InteropWindow.GetWindowLong(handle, -20);
			InteropWindow.SetWindowLong(handle, -20, windowLong | 128);
		}

		public static IntPtr GetWindowHandle(string name)
		{
			IntPtr intPtr = InteropWindow.FindWindow(null, name);
			if (intPtr == IntPtr.Zero)
			{
				throw new SystemException("Cannot find window '" + name + "'", new Win32Exception(Marshal.GetLastWin32Error()));
			}
			return intPtr;
		}

		public static WindowState FindMainWindowState(Window window)
		{
			if (((window != null) ? window.Owner : null) == null)
			{
				return window.WindowState;
			}
			return InteropWindow.FindMainWindowState(window.Owner);
		}
	}
}
