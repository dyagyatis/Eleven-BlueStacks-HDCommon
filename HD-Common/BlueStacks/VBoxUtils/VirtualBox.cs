using System;
using System.Xml.Serialization;

namespace BlueStacks.VBoxUtils
{
	[XmlRoot(ElementName = "VirtualBox", Namespace = "http://www.virtualbox.org/")]
	public class VirtualBox
	{
		[XmlElement(ElementName = "Machine", Namespace = "http://www.virtualbox.org/")]
		public Machine Machine { get; set; }

		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }

		[XmlAttribute(AttributeName = "version")]
		public string Version { get; set; }
	}
}


