using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Techarria.Content.Tiles.Machines;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Techarria.Transfer
{
	internal class GelatinousTurbineInterface : ContainerInterface
    {
        public GelatinousTurbineInterface(int i, int j)
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
            if (tile.TileType == ModContent.TileType<GelatinousTurbine>())
            {
                i -= tile.TileFrameX / 18 % 3;
                j -= tile.TileFrameY / 18 % 2;

                return new Point(i, j);
            }
            return new Point();
        }

        public override List<Item> GetItems()
        {
            TileEntity.ByPosition.TryGetValue(new Point16(x, y), out TileEntity TE);
            GelatinousTurbineTE tileEntity = TE as GelatinousTurbineTE;
            if (tileEntity == null) return new List<Item>();
            return new List<Item>() { tileEntity.item };
        }

        public override bool InsertItem(Item item)
        {
            TileEntity.ByPosition.TryGetValue(new Point16(x, y), out TileEntity TE);
            GelatinousTurbineTE tileEntity = TE as GelatinousTurbineTE;
            if (tileEntity == null) return false;

            Item myItem = tileEntity.item;
            if (myItem == null || myItem.IsAir)
            {
                myItem = item.Clone();
                myItem.stack = 1;
                tileEntity.item = myItem;
                return true;
            }
            if (item.type == myItem.type && myItem.stack < myItem.maxStack)
            {
                myItem.stack++;
                return true;
            }
            return false;
        }
    }
}
