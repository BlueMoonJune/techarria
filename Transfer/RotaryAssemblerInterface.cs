using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Techarria.Content.Tiles.Machines;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Techarria.Transfer
{
	internal class RotaryAssemblerInterface : ContainerInterface
	{
		public RotaryAssemblerInterface(int i, int j) {
			x = i;
			y = j;
		}

		public static Point FindTopLeft(int i, int j) {

			if (Main.tile[i, j].TileType == ModContent.TileType<RotaryAssembler>()) {
				Point16 position = RotaryAssembler.GetTileEntity(i, j).Position;
				return new Point(position.X, position.Y);
			}
			return new Point();
		}

		public override List<Item> GetItems() {
			RotaryAssemblerTE te = RotaryAssembler.GetTileEntity(x, y);
			if (te.GetResult != null) {
				return new List<Item>() { te.GetResult() };
			}
			return new List<Item>();
		}

		public override bool ExtractItem(Item item) {
			RotaryAssemblerTE te = RotaryAssembler.GetTileEntity(x, y);
			if (te.GetResult() != null) {
				te.Craft();
				return true;
			}
			return false;
		}

		public override bool InsertItem(Item item) {
			return RotaryAssembler.GetTileEntity(x, y).InsertItem(item, dir);
		}
	}
}
