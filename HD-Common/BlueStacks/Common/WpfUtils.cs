using System;
using System.Windows;
using System.Windows.Media;

namespace BlueStacks.Common
{
	public static class WpfUtils
	{
		public static double PrimaryWidth
		{
			get
			{
				return SystemParameters.PrimaryScreenWidth;
			}
		}

		public static double PrimaryHeight
		{
			get
			{
				return SystemParameters.PrimaryScreenHeight;
			}
		}

		public static T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
		{
			for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
			{
				DependencyObject child = VisualTreeHelper.GetChild(obj, i);
				if (child != null && child is T)
				{
					return (T)((object)child);
				}
				T t = WpfUtils.FindVisualChild<T>(child);
				if (t != null)
				{
					return t;
				}
			}
			return default(T);
		}

		public static void GetDefaultSize(out double width, out double height, out double left, double aspectRatio, bool isGMWindow)
		{
			int num;
			if (WpfUtils.PrimaryWidth * 0.8 / aspectRatio <= WpfUtils.PrimaryHeight * 0.8)
			{
				num = (int)(WpfUtils.PrimaryWidth * 0.8);
			}
			else
			{
				num = (int)(WpfUtils.PrimaryHeight * 0.8 * aspectRatio);
			}
			if (!isGMWindow)
			{
				width = (double)(num / 4 * 3);
				left = (double)((int)(WpfUtils.PrimaryWidth - (double)num) / 2);
			}
			else
			{
				width = (double)num;
				left = (double)((int)(WpfUtils.PrimaryWidth - (double)num) / 2);
			}
			if (width < 912.0)
			{
				width = 912.0;
				left = 20.0;
			}
			height = (double)((int)width / 16 * 9);
		}

		public static T FindVisualParent<T>(DependencyObject child) where T : DependencyObject
		{
			DependencyObject parent = VisualTreeHelper.GetParent(child);
			if (parent == null)
			{
				return default(T);
			}
			T t = parent as T;
			if (t != null)
			{
				return t;
			}
			return WpfUtils.FindVisualParent<T>(parent);
		}
	}
}


