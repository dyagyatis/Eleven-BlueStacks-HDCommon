using System;
using System.Xml.Serialization;

namespace BlueStacks.VBoxUtils
{
	[XmlRoot(ElementName = "HardwareVirtExLargePages", Namespace = "http://www.virtualbox.org/")]
	public class HardwareVirtExLargePages
	{
		[XmlAttribute(AttributeName = "enabled")]
		public string Enabled { get; set; }
	}
}


