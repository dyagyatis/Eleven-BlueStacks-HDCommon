using System;
using System.Xml.Serialization;

namespace BlueStacks.VBoxUtils
{
	[XmlRoot(ElementName = "Port", Namespace = "http://www.virtualbox.org/")]
	public class Port
	{
		[XmlAttribute(AttributeName = "slot")]
		public string Slot { get; set; }

		[XmlAttribute(AttributeName = "enabled")]
		public string Enabled { get; set; }

		[XmlAttribute(AttributeName = "IOBase")]
		public string IOBase { get; set; }

		[XmlAttribute(AttributeName = "IRQ")]
		public string IRQ { get; set; }
	}
}


