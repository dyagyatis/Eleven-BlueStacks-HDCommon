using System;
using System.Xml.Serialization;

namespace BlueStacks.VBoxUtils
{
	[XmlRoot(ElementName = "ExtraDataItem", Namespace = "http://www.virtualbox.org/")]
	public class ExtraDataItem
	{
		[XmlAttribute(AttributeName = "name")]
		public string Name { get; set; }

		[XmlAttribute(AttributeName = "value")]
		public string Value { get; set; }
	}
}


