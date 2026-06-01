using System;
using System.Xml.Serialization;

namespace BlueStacks.VBoxUtils
{
	[XmlRoot(ElementName = "CPU", Namespace = "http://www.virtualbox.org/")]
	public class CPU
	{
		[XmlElement(ElementName = "PAE", Namespace = "http://www.virtualbox.org/")]
		public PAE PAE { get; set; }

		[XmlElement(ElementName = "LongMode", Namespace = "http://www.virtualbox.org/")]
		public LongMode LongMode { get; set; }

		[XmlElement(ElementName = "HardwareVirtExLargePages", Namespace = "http://www.virtualbox.org/")]
		public HardwareVirtExLargePages HardwareVirtExLargePages { get; set; }

		[XmlAttribute(AttributeName = "count")]
		public string Count { get; set; }
	}
}


