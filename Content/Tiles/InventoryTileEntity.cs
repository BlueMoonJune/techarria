﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Techarria.Transfer;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Techarria.Content.Tiles
{
    public abstract class InventoryTileEntity : ModTileEntity
    {
        public int X => Position.X;
        public int Y => Position.Y;
        public EntityTile TileType => EntityTile.te2Tile[GetType()];
        public int Width => TileType.width;
        public int Height => TileType.height;

        /// <summary>
        /// An array of all extractable items in this container
        /// </summary>
        /// <returns></returns>
        public virtual Item[] ExtractableItems
        {
            get { return Array.Empty<Item>(); }
        }

        public virtual Item[] GetExtractableItemsForInterface(ContainerInterface interf) => ExtractableItems;

        /// <summary>
        /// An array of all items in this container
        /// All these items will be dropped when destroyed
        /// </summary>
        /// <returns></returns>
        public virtual Item[] AllItems => ExtractableItems;

        /// <summary>
        /// Inserts an item into this container
        /// </summary>
        /// <param name="item">The item to insert</param>
        /// <returns>Whether this was sucessful or not</returns>
        public virtual bool InsertItem(Item item)
        {
            return false;
        }

        /// <summary>
        /// Inserts an item into this container
        /// Version that takes the insertion info into consideration
        /// DO NOT override both methods
        /// </summary>
        /// <param name="item">The item to insert</param>
        /// <param name="info">The ContainerInterface that is inserting items</param>
        /// <returns>Whether this was sucessful or not</returns>
        public virtual bool InsertItem(Item item, ContainerInterface info) {
            return false; 
        }

        /// <summary>
        /// Extracts an item from this container
        /// By default, decrements the stack size by one
        /// </summary>
        /// <returns>If the extraction was successful</returns>
        public virtual bool ExtractItem(Item item)
        {
            decrementItem(item);
            return true;
        }


        /// <summary>
        /// Extracts an item from this container
        /// By default, decrements the stack size by one
        /// Version that takes the insertion info into consideration
        /// DO NOT override both methods
        /// </summary>
        /// <param name="item">The item to insert</param>
        /// <param name="info">The ContainerInterface that is extracting items</param>
        /// <returns>Whether this was sucessful or not</returns>
        public virtual bool ExtractItem(Item item, ContainerInterface info)
        {
            return false;
        }

        /// <summary>
        /// Checks if the container is empty
        /// </summary>
        /// <returns></returns>
        public virtual bool IsEmpty() { return ExtractableItems.Count() == 0; }

        public override bool IsTileValidForEntity(int x, int y)
        {
            Debug.WriteLine($"\n\nChecking for valid tile for {GetType()}\n\n");
            ModTile mt = ModContent.GetModTile(Main.tile[x, y].TileType);
            if (mt is EntityTile)
            {
                return EntityTile.tileType2TE[Main.tile[x, y].TileType] == GetType();
            }
            return false;
        }

        public static Item decrementItem(Item item)
        {
            item.stack--;
            if (item.stack <= 0)
                item.TurnToAir();
            return item;
        }

        public override void OnKill()
        {
            foreach (Item item in AllItems)
            {
                Item.NewItem(new EntitySource_TileBreak(X, Y), X * 16, Y * 16, Width * 16, Height * 16, item);
            }
        }
    }
}
