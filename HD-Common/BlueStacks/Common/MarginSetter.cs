using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace BlueStacks.Common
{
	public class MarginSetter : MarkupExtension
	{
		private static Thickness GetMargin(DependencyObject obj)
		{
			return (Thickness)obj.GetValue(MarginSetter.MarginProperty);
		}

		public static void SetMargin(DependencyObject obj, Thickness value)
		{
			if (obj != null)
			{
				obj.SetValue(MarginSetter.MarginProperty, value);
			}
		}

		public static void CreateThicknesForChildren(object sender, DependencyPropertyChangedEventArgs e)
		{
			Panel panel = sender as Panel;
			if (panel == null)
			{
				return;
			}
			foreach (object obj in panel.Children)
			{
				FrameworkElement frameworkElement = obj as FrameworkElement;
				if (frameworkElement != null)
				{
					frameworkElement.Margin = MarginSetter.GetMargin(panel);
				}
			}
		}

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return this;
		}

		public static readonly DependencyProperty MarginProperty = DependencyProperty.RegisterAttached("Margin", typeof(Thickness), typeof(MarginSetter), new UIPropertyMetadata(default(Thickness), new PropertyChangedCallback(MarginSetter.CreateThicknesForChildren)));
	}
}


