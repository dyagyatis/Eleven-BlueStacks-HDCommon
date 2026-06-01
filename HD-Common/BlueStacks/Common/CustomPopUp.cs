using System;
using System.Windows.Controls.Primitives;

namespace BlueStacks.Common
{
	public class CustomPopUp : Popup
	{
		public CustomPopUp()
		{
			base.Opened += this.CustomPopUp_Initialized;
		}

		private void CustomPopUp_Initialized(object sender, EventArgs e)
		{
			RenderHelper.ChangeRenderModeToSoftware(sender);
		}
	}
}


