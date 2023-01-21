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
			TileID.Sets.Ore[Type] = true;
			Main.tileSpelunker[Type] = true; // The tile will be affected by spelunker highlighting
			Main.tileOreFinderPriority[Type] = 410; // Metal Detector value, see https://terraria.gamepedia.com/Metal_Detector
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;

			// tile merge blending
			// blends to dirt
			Main.tileMergeDirt[Type] = true;
			// blends like stone
			Main.tileBrick[Type] = true;

			// map stuff
			ModTranslation name = CreateMapEntryName();
			name.SetDefault("Coal Ore");
			AddMapEntry(new Color(255, 0, 255), name);

			DustType = 84;
			ItemDrop = ModContent.ItemType<Items.Materials.IndustrialCoal>();
			HitSound = SoundID.Tink;
		}
	}
}
