using System;
using System.Xml.Serialization;

namespace BlueStacks.VBoxUtils
{
	[XmlRoot(ElementName = "NATNetwork", Namespace = "http://www.virtualbox.org/")]
	public class NATNetwork
	{
		[XmlAttribute(AttributeName = "name")]
		public string Name { get; set; }
	}
}


