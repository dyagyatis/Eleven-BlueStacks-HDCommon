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
	public partial class CustomComboBox : ComboBox
	{
		public CustomComboBox()
		{
			this.InitializeComponent();
		}

		private void OnRequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
		{
			if (Keyboard.IsKeyDown(Key.Down) || Keyboard.IsKeyDown(Key.Up))
			{
				return;
			}
			e.Handled = true;
		}

		public bool Highlight
		{
			get
			{
				return (bool)base.GetValue(CustomComboBox.HighlightProperty);
			}
			set
			{
				base.SetValue(CustomComboBox.HighlightProperty, value);
			}
		}

		public string ToolTipText
		{
			get
			{
				return (string)base.GetValue(CustomComboBox.ToolTipTextProperty);
			}
			set
			{
				base.SetValue(CustomComboBox.ToolTipTextProperty, value);
			}
		}

		public bool SetToolTip
		{
			get
			{
				return (bool)base.GetValue(CustomComboBox.SetToolTipProperty);
			}
			set
			{
				base.SetValue(CustomComboBox.SetToolTipProperty, value);
			}
		}

		public bool ToolTipWhenTrimmed
		{
			get
			{
				return (bool)base.GetValue(CustomComboBox.ToolTipWhenTrimmedProperty);
			}
			set
			{
				base.SetValue(CustomComboBox.ToolTipWhenTrimmedProperty, value);
			}
		}

		private static void OnSetToolTipChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			CustomComboBox customComboBox = d as CustomComboBox;
			if (customComboBox.ToolTipWhenTrimmed)
			{
				customComboBox.OnSetToolTipChanged(e);
			}
		}

		private void OnSetToolTipChanged(DependencyPropertyChangedEventArgs args)
		{
			bool flag;
			if (bool.TryParse(args.NewValue.ToString(), out flag) && flag && this.IsTextTrimmed(this.ToolTipText))
			{
				ToolTipService.SetIsEnabled(this, true);
				base.ToolTip = this.ToolTipText;
				return;
			}
			ToolTipService.SetIsEnabled(this, false);
		}

		public static readonly DependencyProperty ToolTipTextProperty = DependencyProperty.Register("ToolTipText", typeof(string), typeof(CustomComboBox), new PropertyMetadata(string.Empty));

		public static readonly DependencyProperty HighlightProperty = DependencyProperty.Register("Highlight", typeof(bool), typeof(CustomComboBox), new PropertyMetadata(false));

		public static readonly DependencyProperty SetToolTipProperty = DependencyProperty.Register("SetToolTip", typeof(bool), typeof(CustomComboBox), new PropertyMetadata(false, new PropertyChangedCallback(CustomComboBox.OnSetToolTipChanged)));

		public static readonly DependencyProperty ToolTipWhenTrimmedProperty = DependencyProperty.Register("ToolTipWhenTrimmed", typeof(bool), typeof(CustomComboBox), new PropertyMetadata(false));

	}
}


