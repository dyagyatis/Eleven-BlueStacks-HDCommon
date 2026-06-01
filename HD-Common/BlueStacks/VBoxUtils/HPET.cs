using System;
using System.Xml.Serialization;

namespace BlueStacks.VBoxUtils
{
	[XmlRoot(ElementName = "HPET", Namespace = "http://www.virtualbox.org/")]
	public class HPET
	{
		[XmlAttribute(AttributeName = "enabled")]
		public string Enabled { get; set; }
	}
}


