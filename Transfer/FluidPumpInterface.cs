using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Techarria.Content.Tiles;
using Techarria.Content.Tiles.FluidTransfer;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Techarria.Transfer
{
	public class FluidPumpInterface : ContainerInterface
	{
		public FluidPumpInterface(int i, int j) {
			x = i;
			y = j;
		}

		public static Point FindTopLeft(int i, int j) {
			if (i < 0 || j < 0) {
				return new Point();
			}
			Tile tile = Main.tile[i, j];
			if (ModContent.GetModTile(tile.TileType) is FluidPump) {
				i -= tile.TileFrameX / 18 % 2;
				j -= tile.TileFrameY / 18 % 2;

				return new Point(i, j);
			}
			return new Point();
		}

		public override List<Item> GetItems() {
			TileEntity.ByPosition.TryGetValue(new Point16(x, y), out TileEntity TE);
			FluidPumpTE tileEntity = TE as FluidPumpTE;
			if (tileEntity == null)
				return new List<Item>();
			return new List<Item>() { tileEntity.fluid };
		}

		public override bool InsertItem(Item item) {
			return false;
		}

	}
}
