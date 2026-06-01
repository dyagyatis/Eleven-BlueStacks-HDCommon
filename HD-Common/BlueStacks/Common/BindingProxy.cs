using System;
using System.Windows;

namespace BlueStacks.Common
{
	public class BindingProxy : Freezable
	{
		protected override Freezable CreateInstanceCore()
		{
			return new BindingProxy();
		}

		public object Data
		{
			get
			{
				return base.GetValue(BindingProxy.DataProperty);
			}
			set
			{
				base.SetValue(BindingProxy.DataProperty, value);
			}
		}

		public static readonly DependencyProperty DataProperty = DependencyProperty.Register("Data", typeof(object), typeof(BindingProxy), new PropertyMetadata(null));
	}
}


