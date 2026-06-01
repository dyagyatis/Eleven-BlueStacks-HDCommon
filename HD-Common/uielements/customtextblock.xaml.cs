using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace BlueStacks.Common
{
	public partial class CustomTextBlock : TextBlock
	{
		public CustomTextBlock()
		{
			this.InitializeComponent();
		}

		public bool ForcedTooltip
		{
			get
			{
				return (bool)base.GetValue(CustomTextBlock.ForcedTooltipProperty);
			}
			set
			{
				base.SetValue(CustomTextBlock.ForcedTooltipProperty, value);
			}
		}

		public bool SetToolTip
		{
			get
			{
				return (bool)base.GetValue(CustomTextBlock.SetToolTipProperty);
			}
			set
			{
				base.SetValue(CustomTextBlock.SetToolTipProperty, value);
			}
		}

		public bool HoverForegroundProperty
		{
			get
			{
				return (bool)base.GetValue(CustomTextBlock.SetToolTipProperty);
			}
			set
			{
				base.SetValue(CustomTextBlock.SetToolTipProperty, value);
			}
		}

		private static void OnSetToolTipChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			(d as CustomTextBlock).OnSetToolTipChanged(e);
		}

		private void OnSetToolTipChanged(DependencyPropertyChangedEventArgs args)
		{
			if (!this.ForcedTooltip)
			{
				bool flag;
				if (bool.TryParse(args.NewValue.ToString(), out flag) && flag && this.IsTextTrimmed())
				{
					ToolTipService.SetIsEnabled(this, true);
					if (base.ToolTip == null)
					{
						base.ToolTip = base.Text;
						return;
					}
				}
				else
				{
					ToolTipService.SetIsEnabled(this, false);
				}
			}
		}

		public static readonly DependencyProperty SetToolTipProperty = DependencyProperty.Register("SetToolTip", typeof(bool), typeof(CustomTextBlock), new PropertyMetadata(false, new PropertyChangedCallback(CustomTextBlock.OnSetToolTipChanged)));

		public static readonly DependencyProperty MouseOverForegroundChangedProperty = DependencyProperty.RegisterAttached("HoverForegroundProperty", typeof(bool), typeof(CustomTextBlock), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));

		public static readonly DependencyProperty ForcedTooltipProperty = DependencyProperty.Register("ForcedTooltip", typeof(bool), typeof(CustomButton), new PropertyMetadata(false));

	}
}


