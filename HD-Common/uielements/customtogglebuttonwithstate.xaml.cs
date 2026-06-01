using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace BlueStacks.Common
{
	public partial class CustomToggleButtonWithState : UserControl
	{
		public CustomToggleButtonWithState()
		{
			this.InitializeComponent();
		}

		public bool BoolValue
		{
			get
			{
				return (bool)base.GetValue(CustomToggleButtonWithState.BoolValueProperty);
			}
			set
			{
				base.SetValue(CustomToggleButtonWithState.BoolValueProperty, value);
			}
		}

		private void mToggleButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.BoolValue = !this.BoolValue;
		}

		public static readonly DependencyProperty BoolValueProperty = DependencyProperty.Register("BoolValue", typeof(bool), typeof(CustomToggleButtonWithState), new PropertyMetadata(true));

	}
}


