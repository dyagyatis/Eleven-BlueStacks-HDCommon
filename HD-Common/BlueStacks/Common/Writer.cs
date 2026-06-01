using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace BlueStacks.Common
{
	public class Writer : TextWriter
	{
		public Writer(Writer.WriteFunc writeFunc)
		{
			this.writeFunc = writeFunc;
		}

		public override Encoding Encoding
		{
			get
			{
				return Encoding.UTF8;
			}
		}

		public override void WriteLine(string msg)
		{
			this.writeFunc(msg);
		}

		public override void WriteLine(string fmt, object obj)
		{
			this.writeFunc(string.Format(CultureInfo.InvariantCulture, fmt, new object[] { obj }));
		}

		public override void WriteLine(string fmt, params object[] objs)
		{
			this.writeFunc(string.Format(CultureInfo.InvariantCulture, fmt, objs));
		}

		private Writer.WriteFunc writeFunc;


		public delegate void WriteFunc(string msg);
	}
}


