using System;
using System.Xml.Serialization;

namespace BlueStacks.VBoxUtils
{
	[XmlRoot(ElementName = "BIOS", Namespace = "http://www.virtualbox.org/")]
	public class BIOS
	{
		[XmlElement(ElementName = "IOAPIC", Namespace = "http://www.virtualbox.org/")]
		public IOAPIC IOAPIC { get; set; }

		[XmlElement(ElementName = "Logo", Namespace = "http://www.virtualbox.org/")]
		public Logo Logo { get; set; }

		[XmlElement(ElementName = "BootMenu", Namespace = "http://www.virtualbox.org/")]
		public BootMenu BootMenu { get; set; }
	}
}


