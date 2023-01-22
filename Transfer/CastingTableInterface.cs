﻿using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Techarria.Content.Items.Materials.Molten;
using Techarria.Content.Tiles.Machines;
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
            if (item.ModItem is MoltenBlob blob)
            {
				return te.InsertMolten(item);
            }
            return false;
        }
    }
}