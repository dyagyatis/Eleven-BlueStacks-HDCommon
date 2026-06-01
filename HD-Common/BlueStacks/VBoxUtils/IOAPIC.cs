using System;
using System.Xml.Serialization;

namespace BlueStacks.VBoxUtils
{
	[XmlRoot(ElementName = "IOAPIC", Namespace = "http://www.virtualbox.org/")]
	public class IOAPIC
	{
		[XmlAttribute(AttributeName = "enabled")]
		public string Enabled { get; set; }
	}
}


