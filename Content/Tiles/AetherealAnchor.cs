using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Techarria.Content.Tiles
{
	public class AetherealAnchor : ModTile
	{
		public override void SetStaticDefaults() {

			Main.tileSolid[Type] = false;
			Main.tileBlockLight[Type] = false;
			Main.tileFrameImportant[Type] = true;

			// map stuff
			LocalizedText name = CreateMapEntryName();
			AddMapEntry(new Color(254, 208, 200), name);

			DustType = DustID.ShimmerSpark;
		}

		public override void RandomUpdate(int i, int j) {
			WorldGen.KillTile(i, j);
		}
	}
}
