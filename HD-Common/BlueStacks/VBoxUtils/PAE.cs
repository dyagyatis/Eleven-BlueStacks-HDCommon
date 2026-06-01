using System;
using System.Xml.Serialization;

namespace BlueStacks.VBoxUtils
{
	[XmlRoot(ElementName = "PAE", Namespace = "http://www.virtualbox.org/")]
	public class PAE
	{
		[XmlAttribute(AttributeName = "enabled")]
		public string Enabled { get; set; }
	}
}


