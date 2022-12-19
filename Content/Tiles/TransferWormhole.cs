using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.ObjectData;
using Techarria.Content.Dusts;
using Terraria.Audio;
using System;
using Terraria.DataStructures;

namespace Techarria.Content.Tiles
{
    internal class TransferWormhole : TransferDuct
	{
		public override void SetStaticDefaults()
		{
			// Properties
			Main.tileTable[Type] = true;
			Main.tileSolidTop[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;
			Main.tileFrameImportant[Type] = true;
			TileID.Sets.DisableSmartCursor[Type] = true;
			TileID.Sets.IgnoredByNpcStepUp[Type] = true; // This line makes NPCs not try to step up this tile during their movement. Only use this for furniture with solid tops.

			AdjTiles = new int[] { TileID.Tables };

			// Placement
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.CoordinateHeights = new[] { 16, 18 };
			TileObjectData.addTile(Type);

			AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTable);

			// Etc
			ModTranslation name = CreateMapEntryName();
			name.SetDefault("Table");
			AddMapEntry(new Color(200, 200, 200), name);
		}


		public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 32, ModContent.ItemType<Items.Placeables.TransferWormhole>());
        }
    }
}