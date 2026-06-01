using System;
using System.Xml.Serialization;

namespace BlueStacks.VBoxUtils
{
	[XmlRoot(ElementName = "InternalNetwork", Namespace = "http://www.virtualbox.org/")]
	public class InternalNetwork
	{
		[XmlAttribute(AttributeName = "name")]
		public string Name { get; set; }
	}
}


