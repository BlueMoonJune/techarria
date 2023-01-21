using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

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
			Main.tileOreFinderPriority[Type] = 410; // Metal Detector value, see https://terraria.gamepedia.com/Metal_Detector
			Main.tileShine2[Type] = true; // Modifies the draw color slightly.
			Main.tileShine[Type] = 975; // How often tiny dust appear off this tile. Larger is less frequently
			Main.tileMergeDirt[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;

			ModTranslation name = CreateMapEntryName();
			name.SetDefault("Coal Ore");
			AddMapEntry(new Color(255, 0, 255), name);

			DustType = 84;
			//ItemDrop = ModContent.ItemType<Items.Placeable.IndustrialCoal>();
			HitSound = SoundID.Tink;
			// MineResist = 4f;
			// MinPick = 200;
		}
	}
}
