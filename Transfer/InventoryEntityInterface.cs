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
            EntityTile modTile = (ModContent.GetModTile(Main.tile[x, y].TileType) as EntityTile);
            if (modTile == null) return new List<Item>();
            TileEntity.ByPosition.TryGetValue(modTile.GetTopLeft(x, y), out TileEntity TE);
            InventoryTileEntity tileEntity = TE as InventoryTileEntity;
            if (tileEntity == null) return new List<Item>();
            return tileEntity.GetExtractableItemsForInterface(this).ToList();
        }

        public override bool InsertItem(Item item)
        {
            EntityTile modTile = (ModContent.GetModTile(Main.tile[x, y].TileType) as EntityTile);
            if (modTile == null) return false;
            TileEntity.ByPosition.TryGetValue(modTile.GetTopLeft(x, y), out TileEntity TE);
            InventoryTileEntity tileEntity = TE as InventoryTileEntity;
            if (tileEntity == null) return false;
            return tileEntity.InsertItem(item) || tileEntity.InsertItem(item, this);
        }

        public override bool ExtractItem(Item item)
        {
            EntityTile modTile = (ModContent.GetModTile(Main.tile[x, y].TileType) as EntityTile);
            if (modTile == null) return false;
            TileEntity.ByPosition.TryGetValue(modTile.GetTopLeft(x, y), out TileEntity TE);
            InventoryTileEntity tileEntity = TE as InventoryTileEntity;
            if (tileEntity == null) return false;
            return tileEntity.ExtractItem(item) || tileEntity.ExtractItem(item, this);
        }
    }
}
