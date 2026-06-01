using System;
using System.Xml.Serialization;

namespace BlueStacks.VBoxUtils
{
	[XmlRoot(ElementName = "HID", Namespace = "http://www.virtualbox.org/")]
	public class HID
	{
		[XmlAttribute(AttributeName = "Pointing")]
		public string Pointing { get; set; }
	}
}


