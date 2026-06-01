using System;
using System.Xml.Serialization;

namespace BlueStacks.VBoxUtils
{
	[XmlRoot(ElementName = "BootMenu", Namespace = "http://www.virtualbox.org/")]
	public class BootMenu
	{
		[XmlAttribute(AttributeName = "mode")]
		public string Mode { get; set; }
	}
}


