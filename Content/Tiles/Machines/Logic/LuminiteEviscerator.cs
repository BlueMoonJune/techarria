using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;
using Terraria.ModLoader;

namespace Techarria.Content.Tiles.Machines.Logic
{
	internal class LuminiteEviscerator : BeamDrill
	{
		public static Color[] colors = new Color[4] {
			new(254, 158, 35),
			new(0, 242, 170),
			new(254, 126, 229),
			new(0, 174, 238)
		};

		public override void SetStaticDefaults() {
			power = 225;
			range = 8;
			cost = 4;

			effect = ModContent.Request<Effect>("Techarria/Assets/Effects/LunarBeam", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

			base.SetStaticDefaults();

			AddMapEntry(Color.DarkSlateGray, CreateMapEntryName());

			DustType = DustID.Stone;
			ItemDrop = ModContent.ItemType<Items.Placeables.Machines.LuminiteEviscerator>();

			HitSound = SoundID.Tink;

			base.SetStaticDefaults();
		}
	}
}
