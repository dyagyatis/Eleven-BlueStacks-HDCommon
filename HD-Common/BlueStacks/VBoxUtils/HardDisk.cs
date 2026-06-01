using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace BlueStacks.VBoxUtils
{
	[XmlRoot(ElementName = "HardDisk", Namespace = "http://www.virtualbox.org/")]
	public class HardDisk
	{
		[XmlAttribute(AttributeName = "uuid")]
		public string Uuid { get; set; }

		[XmlAttribute(AttributeName = "location")]
		public string Location { get; set; }

		[XmlAttribute(AttributeName = "format")]
		public string Format { get; set; }

		[XmlAttribute(AttributeName = "type")]
		public string Type { get; set; }

		[XmlElement(ElementName = "HardDisk", Namespace = "http://www.virtualbox.org/")]
		public List<HardDisk> HardDisk1 { get; set; }
	}
}


