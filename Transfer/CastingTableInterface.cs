using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Techarria.Content.Items.Materials.Molten;
using Techarria.Content.Tiles;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Techarria.Transfer
{
    internal class CastingTableInterface : ContainerInterface
    {
        public CastingTableInterface(int i, int j)
        {
            x = i;
            y = j;
        }

        public static Point FindTopLeft(int i, int j)
        {
            if (Main.tile[i, j].TileType == ModContent.TileType<CastingTable>())
            {
                Point16 position = CastingTable.GetTileEntity(i, j).Position;
                return new Point(position.X, position.Y);
            }
            return new Point();
        }

        public override List<Item> GetItems()
        {
            CastingTableTE te = CastingTable.GetTileEntity(x, y);
            if (dir == 1 && !te.item.IsAir)
            {
                return new List<Item>() { te.item };
            }
            return new List<Item>();
        }

        public override bool InsertItem(Item item)
        {
            CastingTableTE te = CastingTable.GetTileEntity(x, y);
            if (te.item.IsAir && item.ModItem is MoltenBlob blob)
            {
                te.item = item;
                te.temp = blob.temp;
                return true;
            }
            return false;
        }
    }
}
