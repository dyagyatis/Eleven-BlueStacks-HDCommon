using System;
using System.Xml.Serialization;

namespace BlueStacks.VBoxUtils
{
	[XmlRoot(ElementName = "LPT", Namespace = "http://www.virtualbox.org/")]
	public class LPT
	{
		[XmlElement(ElementName = "Port", Namespace = "http://www.virtualbox.org/")]
		public Port Port { get; set; }
	}
}


