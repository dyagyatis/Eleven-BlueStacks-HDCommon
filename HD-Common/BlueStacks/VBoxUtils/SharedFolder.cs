using System;
using System.Xml.Serialization;

namespace BlueStacks.VBoxUtils
{
	[XmlRoot(ElementName = "SharedFolder", Namespace = "http://www.virtualbox.org/")]
	public class SharedFolder
	{
		[XmlAttribute(AttributeName = "name")]
		public string Name { get; set; }

		[XmlAttribute(AttributeName = "hostPath")]
		public string HostPath { get; set; }

		[XmlAttribute(AttributeName = "writable")]
		public string Writable { get; set; }

		[XmlAttribute(AttributeName = "autoMount")]
		public string AutoMount { get; set; }
	}
}


