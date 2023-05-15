using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Techarria.Content.Tiles.Natural
{
	public class CoalOre : ModTile
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

			TileID.Sets.Ore[Type] = true;
			Main.tileSpelunker[Type] = true; // The tile will be affected by spelunker highlighting
			Main.tileOreFinderPriority[Type] = 150; // Metal Detector value, see https://terraria.gamepedia.com/Metal_Detector
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;

			// tile merge blending
			// blends to dirt
			Main.tileMergeDirt[Type] = true;
			// blends like stone
			Main.tileBrick[Type] = true;

			// map stuff
			LocalizedText name = CreateMapEntryName();
			// name.SetDefault("Industrial Coal");
			AddMapEntry(new Color(27, 27, 31), name);

			DustType = 84;
			//ItemDrop = ModContent.ItemType<Items.Materials.IndustrialCoal>();
			HitSound = SoundID.Tink;
		}
	}
}
