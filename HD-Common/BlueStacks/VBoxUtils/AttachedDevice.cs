using System;
using System.Xml.Serialization;

namespace BlueStacks.VBoxUtils
{
	[XmlRoot(ElementName = "AttachedDevice", Namespace = "http://www.virtualbox.org/")]
	public class AttachedDevice
	{
		[XmlElement(ElementName = "Image", Namespace = "http://www.virtualbox.org/")]
		public Image Image { get; set; }

		[XmlAttribute(AttributeName = "type")]
		public string Type { get; set; }

		[XmlAttribute(AttributeName = "port")]
		public string Port { get; set; }

		[XmlAttribute(AttributeName = "device")]
		public string Device { get; set; }
	}
}


