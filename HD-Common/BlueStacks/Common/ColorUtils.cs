using System;
using System.Drawing;
using System.Windows.Media;

namespace BlueStacks.Common
{
	[Serializable]
	public class ColorUtils
	{
		public byte R { get; set; }

		public byte G { get; set; }

		public byte B { get; set; }

		public byte A { get; set; }

		public ColorUtils()
		{
			this.R = byte.MaxValue;
			this.G = byte.MaxValue;
			this.B = byte.MaxValue;
			this.A = byte.MaxValue;
		}

		public ColorUtils(global::System.Windows.Media.Color value)
		{
			this.R = value.R;
			this.G = value.G;
			this.B = value.B;
			this.A = value.A;
		}

		public ColorUtils(global::System.Drawing.Color value)
		{
			this.R = value.R;
			this.G = value.G;
			this.B = value.B;
			this.A = value.A;
		}

		public static implicit operator global::System.Drawing.Color(ColorUtils rgb)
		{
			if (rgb != null)
			{
				return global::System.Drawing.Color.FromArgb((int)rgb.A, (int)rgb.R, (int)rgb.G, (int)rgb.B);
			}
			return default(global::System.Drawing.Color);
		}

		public static explicit operator ColorUtils(global::System.Drawing.Color c)
		{
			return new ColorUtils(c);
		}

		public static ColorUtils FromHSL(double H, double S, double L)
		{
			return ColorUtils.FromHSLA(H, S, L, 1.0);
		}

		public static ColorUtils FromHSLA(double H, double S, double L, double A)
		{
			if (H > 1.0)
			{
				H = 1.0;
			}
			if (S > 1.0)
			{
				S = 1.0;
			}
			if (L > 1.0)
			{
				L = 1.0;
			}
			if (H < 0.0)
			{
				H = 0.0;
			}
			if (S < 0.0)
			{
				S = 0.0;
			}
			if (L < 0.0)
			{
				L = 0.0;
			}
			if (A > 1.0)
			{
				A = 1.0;
			}
			double num = L;
			double num2 = L;
			double num3 = L;
			double num4 = ((L <= 0.5) ? (L * (1.0 + S)) : (L + S - L * S));
			if (num4 > 0.0)
			{
				double num5 = L + L - num4;
				double num6 = (num4 - num5) / num4;
				H *= 6.0;
				int num7 = (int)H;
				double num8 = H - (double)num7;
				double num9 = num4 * num6 * num8;
				double num10 = num5 + num9;
				double num11 = num4 - num9;
				switch (num7)
				{
				case 0:
					num = num4;
					num2 = num10;
					num3 = num5;
					break;
				case 1:
					num = num11;
					num2 = num4;
					num3 = num5;
					break;
				case 2:
					num = num5;
					num2 = num4;
					num3 = num10;
					break;
				case 3:
					num = num5;
					num2 = num11;
					num3 = num4;
					break;
				case 4:
					num = num10;
					num2 = num5;
					num3 = num4;
					break;
				case 5:
					num = num4;
					num2 = num5;
					num3 = num11;
					break;
				}
			}
			return new ColorUtils
			{
				R = Convert.ToByte(num * 255.0),
				G = Convert.ToByte(num2 * 255.0),
				B = Convert.ToByte(num3 * 255.0),
				A = Convert.ToByte(A * 255.0)
			};
		}

		public float H
		{
			get
			{
				return this.GetHue() / 360f;
			}
		}

		public float S
		{
			get
			{
				return this.GetSaturation();
			}
		}

		public float L
		{
			get
			{
				return this.GetBrightness();
			}
		}

		private float GetHue()
		{
			return global::System.Drawing.Color.FromArgb((int)this.A, (int)this.R, (int)this.G, (int)this.B).GetHue();
		}

		private float GetSaturation()
		{
			return global::System.Drawing.Color.FromArgb((int)this.A, (int)this.R, (int)this.G, (int)this.B).GetSaturation();
		}

		private float GetBrightness()
		{
			return global::System.Drawing.Color.FromArgb((int)this.A, (int)this.R, (int)this.G, (int)this.B).GetBrightness();
		}

		public global::System.Windows.Media.Color WPFColor
		{
			get
			{
				return global::System.Windows.Media.Color.FromArgb(this.A, this.R, this.G, this.B);
			}
		}
	}
}


