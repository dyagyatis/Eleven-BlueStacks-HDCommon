using System;
using System.Xml.Serialization;

namespace BlueStacks.VBoxUtils
{
	[XmlRoot(ElementName = "Order", Namespace = "http://www.virtualbox.org/")]
	public class Order
	{
		[XmlAttribute(AttributeName = "position")]
		public string Position { get; set; }

		[XmlAttribute(AttributeName = "device")]
		public string Device { get; set; }
	}
}


