using System;

namespace BlueStacks.Common
{
	public static class RenderHelper
	{
		private static bool SoftwareOnly
		{
			get
			{
				return false;
			}
		}

		public static void ChangeRenderModeToSoftware(object sender)
		{
		}

		private static bool? mSoftwareOnly;
	}
}


