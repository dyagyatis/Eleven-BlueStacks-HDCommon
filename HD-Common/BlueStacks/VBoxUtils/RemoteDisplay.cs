using System;
using System.Xml.Serialization;

namespace BlueStacks.VBoxUtils
{
	[XmlRoot(ElementName = "RemoteDisplay", Namespace = "http://www.virtualbox.org/")]
	public class RemoteDisplay
	{
		[XmlAttribute(AttributeName = "enabled")]
		public string Enabled { get; set; }
	}
}


