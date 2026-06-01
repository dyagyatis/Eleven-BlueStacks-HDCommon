using System;

namespace BlueStacks.Common
{
	[Serializable]
	public class DynamicOverlayConfig
	{
		public string Type { get; set; }

		public string Enabled { get; set; }

		public double X { get; set; }

		public double Y { get; set; }

		public double LookAroundX { get; set; }

		public double LookAroundY { get; set; }

		public double LButtonX { get; set; }

		public double LButtonY { get; set; }

		public double CancelX { get; set; }

		public double CancelY { get; set; }

		public bool LButtonShowOnOverlay { get; set; }

		public bool LookAroundShowOnOverlay { get; set; }

		public bool CancelShowOnOverlay { get; set; }
	}
}


