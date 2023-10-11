using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Techarria.Content.Tiles.Misc;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Techarria.Transfer
{
    public class CashCompactorInterface : ContainerInterface
    {
        public CashCompactorInterface(int i, int j)
        {
            x = i;
            y = j;
        }

        public static Point FindTopLeft(int i, int j) {
			if (Main.tile[i, j].TileType != ModContent.TileType<CashCompactor>())
				return new Point();
			return new Point(i, j);
		}

		public override bool ExtractItem(Item item) {
			CashCompactorTE tileEntity = CashCompactor.GetTileEntity(x, y);
			tileEntity.amount -= CashCompactorTE.coinValues[new List<int>(CashCompactorTE.coinTypes).FindIndex(value => value == item.type)];
			return true;
		}

		public override List<Item> GetItems() {
			CashCompactorTE tileEntity = CashCompactor.GetTileEntity(x, y);
			Point16 subTile = new Point16(x, y) - tileEntity.Position;
			Item[] coins = tileEntity.GetPossibleCoins();
			if (subTile.Y > 0) {
				if (coins[subTile.X].stack > 0) {
					return new List<Item>() { coins[subTile.X] };
				}
				return new List<Item>();
			}

			List<Item> coinList = new List<Item>(coins);
			coinList.Reverse();
			coinList.RemoveAll(coin => coin.stack <= 0);
			return coinList;
        }

        public override bool InsertItem(Item item) {
			CashCompactorTE tileEntity = CashCompactor.GetTileEntity(x, y);

			List<int> coinTypes = new List<int>(CashCompactorTE.coinTypes);
			int index = coinTypes.FindIndex(value => value == item.type);
			if (index >= 0) {
				tileEntity.amount += CashCompactorTE.coinValues[index];
				return true;
			}

			return false;
		}

	}
}
