using Terraria;

namespace Techarria
{
	public enum GlueTypes : byte {
		Slimy = 1,
		Frigid = 2,
		Elastic = 4,
		Sweet = 8,
	}

	public struct Glue : ITileData
	{
		public GlueTypes types;

		public bool Slimy
		{
			get => (types & GlueTypes.Slimy) != 0;
			set => types = value ? types | GlueTypes.Slimy : types & ~GlueTypes.Slimy;
		}
		public bool Frigid {
			get => (types & GlueTypes.Frigid) != 0;
			set => types = value ? types | GlueTypes.Frigid : types & ~GlueTypes.Frigid;
		}
		public bool Elastic {
			get => (types & GlueTypes.Elastic) != 0;
			set => types = value ? types | GlueTypes.Elastic : types & ~GlueTypes.Elastic;
		}
		public bool Sweet {
			get => (types & GlueTypes.Sweet) != 0;
			set => types = value ? types | GlueTypes.Sweet : types & ~GlueTypes.Sweet;
		}

		public bool GetChannel(int channel) {
			switch (channel) {
				case 0:
					return Slimy;
				case 1:
					return Frigid;
				case 2:
					return Elastic;
				case 3:
					return Sweet;
				default:
					return false;
			}

		}

		public bool SetChannel(int channel, bool val) {
			switch (channel) {
				case 0:
					if (Slimy ^ val) {
						Slimy = val;
						return true;
					}
					return false;
				case 1:
					if (Frigid ^ val) {
						Frigid = val;
						return true;
					}
					return false;
				case 2:
					if (Elastic ^ val) {
						Elastic = val;
						return true;
					}
					return false;
				case 3:
					if (Sweet ^ val) {
						Sweet = val;
						return true;
					}
					return false;
				default:
					return false;
			}
		}
	}
}
