using System;
using System.Collections.Generic;

namespace BlueStacks.Common
{
	public class GenericNotificationDesignItem
	{
		public double AutoHideTime { get; set; } = 3500.0;

		public string LeftGifName { get; set; }

		public string LeftGifUrl { get; set; }

		public string TitleForeGroundColor { get; set; }

		public string MessageForeGroundColor { get; set; }

		public string BorderColor { get; set; }

		public string Ribboncolor { get; set; }

		public string HoverBorderColor { get; set; }

		public string HoverRibboncolor { get; set; }

		public List<SerializableKeyValuePair<string, double>> BackgroundGradient { get; } = new List<SerializableKeyValuePair<string, double>>();

		public List<SerializableKeyValuePair<string, double>> HoverBackGroundGradient { get; } = new List<SerializableKeyValuePair<string, double>>();
	}
}


