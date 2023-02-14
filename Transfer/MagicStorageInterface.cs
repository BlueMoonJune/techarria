using MagicStorage.Components;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace Techarria.Transfer
{

	[JITWhenModsEnabled("MagicStorage")]
	[ExtendsFromMod("MagicStorage")]
	public class MagicStorageInterface : ContainerInterface
    {

        public MagicStorageInterface(int i, int j)
        {
            x = i;
            y = j;
        }

        public static Point FindTopLeft(int i, int j)
        {
            Tile tile = Main.tile[i, j];
            if (i < 0 || j < 0)
                return new Point();

            if (!tile.HasTile || ModContent.GetModTile(tile.TileType) is not StorageAccess)
                return new Point();

            if (tile.TileFrameX > 0) i--;
            if (tile.TileFrameY > 0) j--;
            return new Point(i, j);
        }

        public override List<Item> GetItems()
        {
            if (ModContent.GetModTile(Main.tile[x, y].TileType) is StorageAccess modTile)
            {
                return modTile.GetHeart(x, y).GetStoredItems().ToList();
            }
            return null;
        }

        public override bool ExtractItem(Item item)
        {
            Item tempItem = item.Clone();
            tempItem.stack = 1;
            if (ModContent.GetModTile(Main.tile[x, y].TileType) is StorageAccess modTile)
            {
                TEStorageHeart heart = modTile.GetHeart(x, y);
                return !heart.Withdraw(tempItem, true).IsAir;
            }
            return false;
        }

        public override bool InsertItem(Item item)
        {
            Item deposit = item.Clone();
            deposit.stack = 1;
            if (ModContent.GetModTile(Main.tile[x, y].TileType) is StorageAccess modTile)
            {
                TEStorageHeart heart = modTile.GetHeart(x, y);
                foreach (TEAbstractStorageUnit storageUnit in heart.GetStorageUnits())
                {
                    if (storageUnit is TEStorageUnit unit && !unit.Inactive && unit.HasSpaceFor(item))
                    {
                        heart.TryDeposit(deposit);
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
