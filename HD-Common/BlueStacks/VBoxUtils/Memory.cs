using System;
using System.Xml.Serialization;

namespace BlueStacks.VBoxUtils
{
	[XmlRoot(ElementName = "Memory", Namespace = "http://www.virtualbox.org/")]
	public class Memory
	{
		[XmlAttribute(AttributeName = "RAMSize")]
		public string RAMSize { get; set; }
	}
}


