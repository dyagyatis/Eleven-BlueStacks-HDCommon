using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace BlueStacks.VBoxUtils
{
	[XmlRoot(ElementName = "ExtraData", Namespace = "http://www.virtualbox.org/")]
	public class ExtraData
	{
		[XmlElement(ElementName = "ExtraDataItem", Namespace = "http://www.virtualbox.org/")]
		public List<ExtraDataItem> ExtraDataItem { get; set; }
	}
}


