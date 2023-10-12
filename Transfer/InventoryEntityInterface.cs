using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Techarria.Content.Tiles;
using Techarria.Content.Tiles.Machines;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Techarria.Transfer
{
	internal class InventoryEntityInterface : ContainerInterface
    {
        public InventoryEntityInterface(int i, int j)
        {
            x = i;
            y = j;
        }

        public static Point16 GetTopLeft(int i, int j)
        {
            if (!Main.tile[i, j].HasTile) return negOne;
            int type = Main.tile[i, j].TileType;
            EntityTile entityTile;
            if (EntityTile.tileType2Tile.TryGetValue(type, out entityTile))
                return entityTile.GetTopLeft(i, j);
            return negOne;
        }

        public override List<Item> GetItems()
        {
            TileEntity.ByPosition.TryGetValue(new Point16(x, y), out TileEntity TE);
            InventoryTileEntity tileEntity = TE as InventoryTileEntity;
            if (tileEntity == null) return new List<Item>();
            return tileEntity.Items.ToList();
        }

        public override bool InsertItem(Item item)
        {
            TileEntity.ByPosition.TryGetValue(new Point16(x, y), out TileEntity TE);
            InventoryTileEntity tileEntity = TE as InventoryTileEntity;
            if (tileEntity == null) return false;
            return tileEntity.InsertItem(item) || tileEntity.InsertItem(item, dir);
        }

        public override bool ExtractItem(Item item)
        {
            TileEntity.ByPosition.TryGetValue(new Point16(x, y), out TileEntity TE);
            InventoryTileEntity tileEntity = TE as InventoryTileEntity;
            if (tileEntity == null) return false;
            return tileEntity.ExtractItem(item);
        }
    }
}
