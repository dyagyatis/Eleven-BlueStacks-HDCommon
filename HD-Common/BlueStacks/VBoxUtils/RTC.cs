using System;
using System.Xml.Serialization;

namespace BlueStacks.VBoxUtils
{
	[XmlRoot(ElementName = "RTC", Namespace = "http://www.virtualbox.org/")]
	public class RTC
	{
		[XmlAttribute(AttributeName = "localOrUTC")]
		public string LocalOrUTC { get; set; }
	}
}


