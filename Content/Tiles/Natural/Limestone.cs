using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Techarria.Content.Tiles.Natural
{
	public class Limestone : ModTile
    {
		public override void SetStaticDefaults()
		{
			Main.tileSolid[Type] = true;
			TileID.Sets.Ore[Type] = true;
			Main.tileBlockLight[Type] = true;

			// tile merge blending
			// blends to dirt
			Main.tileMergeDirt[Type] = true;
			// blends like stone
			Main.tileBrick[Type] = true;

			// map stuff
			AddMapEntry(new Color(191, 185, 182));


			DustType = 84;
			//ItemDrop = ModContent.ItemType<Items.Placeables.Natural.Limestone>();
			HitSound = SoundID.Tink;
		}
	}
}
