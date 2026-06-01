using System;
using System.Globalization;

namespace BlueStacks.Common
{
	public class Version : IComparable<Version>
	{
		public const string STRING = "4.220.0.4001";
		public const string CLIENT_VERSION_STRING = "4.220.0.4001";
		public const int IMAP_VERSION_STRING = 17;
		public const string COMPANY = "Evolution Prestige Systems, Inc.";
		public const string PRODUCT = "Evolution Prestige";
		public const string COPYRIGHT = "Copyright ? Evolution, Inc., 2024 through 2025, All Rights Reserved.";
		public const string OEM = "bgp64";

		public int Major { get; private set; }
		public int Minor { get; private set; }
		public int Build { get; private set; }
		public int Revision { get; private set; }

		public Version()
		{
			this.Major = 0;
			this.Minor = 0;
			this.Build = 0;
			this.Revision = 0;
		}

		public Version(string version)
		{
			if (string.IsNullOrEmpty(version))
			{
				this.Major = this.Minor = this.Build = this.Revision = 0;
				return;
			}
			string[] parts = version.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
			int[] nums = new int[4];
			for (int i = 0; i < parts.Length && i < 4; i++)
			{
				int v;
				if (int.TryParse(parts[i], NumberStyles.Integer, CultureInfo.InvariantCulture, out v))
				{
					nums[i] = v;
				}
				else
				{
					nums[i] = 0;
				}
			}
			this.Major = nums[0];
			this.Minor = nums[1];
			this.Build = nums[2];
			this.Revision = nums[3];
		}

		public Version(int major, int minor, int build, int revision)
		{
			this.Major = major;
			this.Minor = minor;
			this.Build = build;
			this.Revision = revision;
		}

		public Version(System.Version v)
		{
			if (v == null)
			{
				this.Major = this.Minor = this.Build = this.Revision = 0;
			}
			else
			{
				this.Major = v.Major;
				this.Minor = v.Minor;
				this.Build = v.Build;
				this.Revision = v.Revision;
			}
		}

		public int CompareTo(Version other)
		{
			if (other == null) return 1;
			int c = this.Major.CompareTo(other.Major);
			if (c != 0) return c;
			c = this.Minor.CompareTo(other.Minor);
			if (c != 0) return c;
			c = this.Build.CompareTo(other.Build);
			if (c != 0) return c;
			return this.Revision.CompareTo(other.Revision);
		}

		public override bool Equals(object obj)
		{
			Version other = obj as Version;
			if (other == null) return false;
			return this.Major == other.Major && this.Minor == other.Minor && this.Build == other.Build && this.Revision == other.Revision;
		}

		public override int GetHashCode()
		{
			return (((this.Major * 397) ^ this.Minor) * 397 ^ this.Build) * 397 ^ this.Revision;
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}.{1}.{2}.{3}", this.Major, this.Minor, this.Build, this.Revision);
		}

		public static implicit operator Version(System.Version v)
		{
			return new Version(v);
		}

		public static implicit operator System.Version(Version v)
		{
			if (v == null) return new System.Version(0, 0, 0, 0);
			try
			{
				return new System.Version(v.Major, v.Minor, v.Build, v.Revision);
			}
			catch
			{
				return new System.Version(0, 0, 0, 0);
			}
		}

		public static bool operator >(Version a, Version b)
		{
			if (ReferenceEquals(a, null)) return false;
			return a.CompareTo(b) > 0;
		}

		public static bool operator <(Version a, Version b)
		{
			if (ReferenceEquals(a, null)) return !ReferenceEquals(b, null);
			return a.CompareTo(b) < 0;
		}

		public static bool operator >=(Version a, Version b)
		{
			if (ReferenceEquals(a, null)) return ReferenceEquals(b, null);
			return a.CompareTo(b) >= 0;
		}

		public static bool operator <=(Version a, Version b)
		{
			if (ReferenceEquals(a, null)) return true;
			return a.CompareTo(b) <= 0;
		}
	}
}


