using System;
using System.Xml.Serialization;

namespace BlueStacks.VBoxUtils
{
	[XmlRoot(ElementName = "Adapter", Namespace = "http://www.virtualbox.org/")]
	public class Adapter
	{
		[XmlElement(ElementName = "DisabledModes", Namespace = "http://www.virtualbox.org/")]
		public DisabledModes DisabledModes { get; set; }

		[XmlElement(ElementName = "NAT", Namespace = "http://www.virtualbox.org/")]
		public string NAT { get; set; }

		[XmlAttribute(AttributeName = "slot")]
		public string Slot { get; set; }

		[XmlAttribute(AttributeName = "enabled")]
		public string Enabled { get; set; }

		[XmlAttribute(AttributeName = "MACAddress")]
		public string MACAddress { get; set; }

		[XmlAttribute(AttributeName = "cable")]
		public string Cable { get; set; }

		[XmlAttribute(AttributeName = "type")]
		public string Type { get; set; }
	}
}


