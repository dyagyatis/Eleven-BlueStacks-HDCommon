using System;
using System.Xml.Serialization;

namespace BlueStacks.VBoxUtils
{
	[XmlRoot(ElementName = "Image", Namespace = "http://www.virtualbox.org/")]
	public class Image
	{
		[XmlAttribute(AttributeName = "uuid")]
		public string Uuid { get; set; }
	}
}


