using System;
using System.Xml.Serialization;

namespace BlueStacks.VBoxUtils
{
	[XmlRoot(ElementName = "AudioAdapter", Namespace = "http://www.virtualbox.org/")]
	public class AudioAdapter
	{
		[XmlAttribute(AttributeName = "driver")]
		public string Driver { get; set; }

		[XmlAttribute(AttributeName = "enabled")]
		public string Enabled { get; set; }
	}
}


