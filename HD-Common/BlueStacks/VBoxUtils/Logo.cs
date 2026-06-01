using System;
using System.Xml.Serialization;

namespace BlueStacks.VBoxUtils
{
	[XmlRoot(ElementName = "Logo", Namespace = "http://www.virtualbox.org/")]
	public class Logo
	{
		[XmlAttribute(AttributeName = "fadeIn")]
		public string FadeIn { get; set; }

		[XmlAttribute(AttributeName = "fadeOut")]
		public string FadeOut { get; set; }

		[XmlAttribute(AttributeName = "displayTime")]
		public string DisplayTime { get; set; }
	}
}


