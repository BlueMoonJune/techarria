using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;


namespace Techarria.Content.Tiles
{
	public class JourneyCrateTE : StorageCrateTE
    {
		public override bool IsTileValidForEntity(int x, int y)
        {
            return Main.tile[x, y].TileType == ModContent.TileType<JourneyCrate>();
        }

        public override void Update()
        {
			if (CreativeItemSacrificesCatalog.Instance.TryGetSacrificeCountCapToUnlockInfiniteItems(item.type, out int researchAmount) && item.stack >= researchAmount)
			{
				item.stack = 99999;
			}
		}
	}

    // Where the TE ends and the Tile starts
    public class JourneyCrate : CashCompactor
    {
        public override void PlaceInWorld(int i, int j, Item item)
		{
			Tile tile = Framing.GetTileSafely(i, j);
			i -= tile.TileFrameX / 18 % 2;
			j -= tile.TileFrameY / 18 % 2;
			ModContent.GetInstance<JourneyCrateTE>().Place(i, j);
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
				if (CreativeItemSacrificesCatalog.Instance.TryGetSacrificeCountCapToUnlockInfiniteItems(item.type, out int researchAmount) && item.stack >= researchAmount)
                {
					Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 32, item.type, item.maxStack);
					item.TurnToAir();
				} else
                {
					Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 32, item.type, amount);
				}
				if (item.stack <= 0)
				{
					item.TurnToAir();
				}
			}

			base.KillTile(i, j, ref fail, ref effectOnly, ref noItem);
		}
		public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{
			Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 32, ModContent.ItemType<Items.Placeables.JourneyCrate>());
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
				if (CreativeItemSacrificesCatalog.Instance.TryGetSacrificeCountCapToUnlockInfiniteItems(item.type, out int researchAmount) && item.stack >= researchAmount)
				{
					player.cursorItemIconText = "∞";
				} else if (researchAmount == 0)
                {
					player.cursorItemIconText = item.stack + "\nThis item can not be researched";
				} else
                {
					player.cursorItemIconText = "" + item.stack + " / " + CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[item.type];
				}
				player.cursorItemIconID = item.type;
			}
		}
	}
}
