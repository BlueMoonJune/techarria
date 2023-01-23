using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;

namespace Techarria.Content.Tiles
{
	public class InfinityCrateTE : StorageCrateTE
	{
        public override bool IsTileValidForEntity(int x, int y)
        {
			return Main.tile[x, y].TileType == ModContent.TileType<InfinityCrate>();
        }

        public override void Update()
        {
            item.stack = 99999;
        }
    }

	// Where the TE ends and the Tile starts
	public class InfinityCrate : StorageCrate
	{
		public override void PlaceInWorld(int i, int j, Item item)
		{
			Tile tile = Framing.GetTileSafely(i, j);
			i -= tile.TileFrameX / 18 % 2;
			j -= tile.TileFrameY / 18 % 2;
			ModContent.GetInstance<InfinityCrateTE>().Place(i, j);
		}

		public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
		{
			StorageCrateTE tileEntity = GetTileEntity(i, j);
			Item item = tileEntity.item;
			if (!item.IsAir)
			{
				fail = true;
				int amount = Math.Min(item.maxStack, item.stack);
				item.stack -= amount;
				Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 32, item.type, item.maxStack);
				tileEntity.item.TurnToAir();
			}

			base.KillTile(i, j, ref fail, ref effectOnly, ref noItem);
		}
		public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{
			Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 32, ModContent.ItemType<Items.Placeables.InfinityCrate>());
		}

		public override void MouseOver(int i, int j)
		{
			StorageCrateTE tileEntity = GetTileEntity(i, j);
			Item item = tileEntity.item;
			Player player = Main.LocalPlayer;
			player.noThrow = 2;
			if ((item != null) && (!item.IsAir))
			{
				player.cursorItemIconEnabled = true;
				player.cursorItemIconText = "∞";
				player.cursorItemIconID = item.type;
			}
		}
	}
}
