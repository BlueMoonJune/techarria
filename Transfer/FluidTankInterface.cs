using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Techarria.Content.Tiles;
using Techarria.Content.Tiles.FluidTransfer;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Techarria.Transfer
{
	public class FluidTankInterface : ContainerInterface
	{
		public FluidTankInterface(int i, int j) {
			x = i;
			y = j;
		}

		public static Point FindTopLeft(int i, int j) {
			if (i < 0 || j < 0) {
				return new Point();
			}
			Tile tile = Main.tile[i, j];
			if (ModContent.GetModTile(tile.TileType) is FluidTank) {
				i -= tile.TileFrameX / 18 % 2;
				j -= tile.TileFrameY / 18 % 2;

				return new Point(i, j);
			}
			return new Point();
		}

		public override List<Item> GetItems() {
			TileEntity.ByPosition.TryGetValue(new Point16(x, y), out TileEntity TE);
			FluidTankTE tileEntity = TE as FluidTankTE;
			if (tileEntity == null)
				return new List<Item>();
			return new List<Item>() { tileEntity.fluid };
		}

		public override bool InsertItem(Item item) {
			TileEntity.ByPosition.TryGetValue(new Point16(x, y), out TileEntity TE);
			FluidTankTE tileEntity = TE as FluidTankTE;
			if (tileEntity == null)
				return false;

			Item myItem = tileEntity.fluid;
			if (myItem == null || myItem.IsAir) {
				myItem = item.Clone();
				myItem.stack = 1;
				tileEntity.fluid = myItem;
				return true;
			}
			if (item.type == myItem.type && myItem.stack < FluidTank.maxStorage /* <- max storage within a single storage crate */) {
				myItem.stack++;
				return true;
			}
			return false;

		}

	}
}
