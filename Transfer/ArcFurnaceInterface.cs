using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Techarria.Content.Tiles;
using Techarria.Content.Tiles.Machines;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Techarria.Transfer
{
	public class ArcFurnaceInterface : ContainerInterface
    {
        public ArcFurnaceInterface(int i, int j)
        {
            x = i;
            y = j;
        }

        public static Point FindTopLeft(int i, int j)
        {
            if (i < 0 || j < 0)
            {
                return new Point();
            }
            Tile tile = Main.tile[i, j];
            if (ModContent.GetModTile(tile.TileType) is ArcFurnace)
            {
                i -= tile.TileFrameX / 18 % 5;
                j -= tile.TileFrameY / 18 % 4;

                return new Point(i, j);
            }
            return new Point();
        }

        public override List<Item> GetItems()
        {
            TileEntity.ByPosition.TryGetValue(new Point16(x, y), out TileEntity TE);
            ArcFurnaceTE tileEntity = TE as ArcFurnaceTE;
            if (tileEntity == null) return new List<Item>();
			List<Item> items = new();
			if (!tileEntity.output.IsAir) {
				items.Add(tileEntity.output);
			}
			foreach (Item item in tileEntity.inputs) {
				items.Add(item);
			}

            return items;
        }

        public override bool InsertItem(Item item)
        {
			TileEntity.ByPosition.TryGetValue(new Point16(x, y), out TileEntity TE);
			ArcFurnaceTE tileEntity = TE as ArcFurnaceTE;
			return tileEntity.InsertItem(item, false);
		}

    }
}
