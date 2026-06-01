using System;
using System.Xml.Serialization;

namespace BlueStacks.VBoxUtils
{
	[XmlRoot(ElementName = "Controller", Namespace = "http://www.virtualbox.org/")]
	public class Controller
	{
		[XmlAttribute(AttributeName = "name")]
		public string Name { get; set; }

		[XmlAttribute(AttributeName = "type")]
		public string Type { get; set; }
	}
}


