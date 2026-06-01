using System;
using System.Xml.Serialization;

namespace BlueStacks.VBoxUtils
{
	[XmlRoot(ElementName = "Display", Namespace = "http://www.virtualbox.org/")]
	public class Display
	{
		[XmlAttribute(AttributeName = "VRAMSize")]
		public string VRAMSize { get; set; }
	}
}


