using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Navigation;

namespace BlueStacks.Common
{
	public partial class EngineSettingBase : UserControl
	{
		public EngineSettingBase()
		{
			this.InitializeComponent();
		}

		private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
		{
			try
			{
				Logger.Info("Opening url: " + e.Uri.AbsoluteUri);
				Utils.OpenUrl(e.Uri.AbsoluteUri);
				e.Handled = true;
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in opening url" + ex.ToString());
			}
		}

		internal void SetAdvancedGraphicMode(bool useAdvancedGraphicEngine)
		{
		}

		public void SetGraphicMode(GraphicsMode mode)
		{
		}

		private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
		{
			BluestacksUIColor.ScrollBarScrollChanged(sender, e);
		}

		private void AutoUnlockKeyBox_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			e.Handled = true;
			Key key = (e.Key == Key.System) ? e.SystemKey : e.Key;
			if (key == Key.LeftShift || key == Key.RightShift ||
				key == Key.LeftCtrl || key == Key.RightCtrl ||
				key == Key.LeftAlt || key == Key.RightAlt ||
				key == Key.LWin || key == Key.RWin)
			{
				return;
			}
			EngineSettingBaseViewModel vm = this.DataContext as EngineSettingBaseViewModel;
			if (vm != null)
			{
				vm.AutoUnlockKeyText = key.ToString();
			}
		}

	}
}


