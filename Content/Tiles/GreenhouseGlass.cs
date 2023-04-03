using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Techarria.Content.Tiles
{
	public class GreenhouseGlass : ModTile
	{
		public override void SetStaticDefaults() {
			for (int i = 0; i < Main.tileMerge.Length; i++) {
				if (Main.tileMerge[i][1]) {
					Main.tileMerge[i][Type] = true;
				}
			}

			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = false;

			// blends like stone
			Main.tileBrick[Type] = true;

			// map stuff
			LocalizedText name = CreateMapEntryName();
			AddMapEntry(new Color(254, 208, 200), name);

			DustType = 84;
			ItemDrop = ModContent.ItemType<Items.Placeables.GreenhouseGlass>();
			HitSound = SoundID.Tink;
		}
	}

	public class GreenhouseAccentGlass : GreenhouseGlass
    {
		public override void SetStaticDefaults()
		{
			for (int i = 0; i < Main.tileMerge.Length; i++)
			{
				if (Main.tileMerge[i][1])
				{
					Main.tileMerge[i][Type] = true;
				}
			}

			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = false;

			// blends like stone
			//Main.tileBrick[Type] = true;

			// map stuff
			LocalizedText name = CreateMapEntryName();
			AddMapEntry(new Color(254, 208, 200), name);

			DustType = 84;
			ItemDrop = ModContent.ItemType<Items.Placeables.GreenhouseAccentGlass>();
			HitSound = SoundID.Tink;
		}
	}
}
