using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;

namespace Techarria.Content.Tiles.StorageCrates
{
    public abstract class CrateEntity : InventoryTileEntity
    {
        public Item item = new();
        public override Item[] Items => new Item[] { item };

        public override bool InsertItem(Item item)
        {
            TileEntity.ByPosition.TryGetValue(Position, out TileEntity TE);
            StorageCrateTE tileEntity = TE as StorageCrateTE;
            if (tileEntity == null) return false;

            Item myItem = tileEntity.item;
            if (myItem == null || myItem.IsAir)
            {
                myItem = item.Clone();
                myItem.stack = 1;
                tileEntity.item = myItem;
                return true;
            }
            if (item.type == myItem.type && myItem.stack < 9999999 /* <- max storage within a single storage crate */)
            {
                myItem.stack++;
                return true;
            }
            return false;

        }

        public override void SaveData(TagCompound tag)
        {
            tag.Add("item", item);
            base.SaveData(tag);
        }

        public override void LoadData(TagCompound tag)
        {
            item = tag.Get<Item>("item");
            base.LoadData(tag);
        }
    }

    public abstract class CrateTile<T> : EntityTile<T> where T : CrateEntity
    {
        public static int maxStorage = 9999999;
        public override void PreStaticDefaults()
        {
            // Spelunker
            Main.tileSpelunker[Type] = true;

            // Properties
            Main.tileLavaDeath[Type] = false;
            Main.tileFrameImportant[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;
            TileID.Sets.IgnoredByNpcStepUp[Type] = true;
            TileID.Sets.PreventsTileRemovalIfOnTopOfIt[Type] = true;

            Main.tileFrameImportant[Type] = true;
            Main.tileSolidTop[Type] = true;
            Main.tileTable[Type] = true;

            DustType = DustID.Stone;

            width = 2;
            height = 2;
        }

        public override void ModifyTileObjectData()
        {
            TileObjectData.newTile.Origin = new Point16(0, 1);
            TileObjectData.newTile.CoordinateHeights = new[] { 16, 18 };
            TileObjectData.newTile.AnchorInvalidTiles = new int[] { TileID.MagicalIceBlock };
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.LavaDeath = false;
        }

        public override bool RightClick(int i, int j)
        {
            CrateEntity tileEntity = GetTileEntity(i, j);
            Item item = tileEntity.item;
            Item playerItem;
            if (!Main.mouseItem.IsAir)
            {
                playerItem = Main.mouseItem;
            }
            else
            {
                playerItem = Main.player[Main.myPlayer].HeldItem;
            }

            if (item.IsAir)
            {
                item = playerItem.Clone();
                item.stack = playerItem.stack;
                tileEntity.item = item;
                playerItem.stack -= playerItem.stack;
                // overflow prevention
                if (item.stack > maxStorage)
                {
                    int overflow = item.stack - maxStorage;
                    playerItem.stack += overflow;
                    item.stack -= overflow;
                }

                if (playerItem.stack <= 0)
                {
                    playerItem.TurnToAir();
                }
                return true;
            }
            if (!item.IsAir && playerItem.type == item.type && item.stack < maxStorage /* <- max storage within a single storage crate */)
            {
                item.stack += playerItem.stack;
                playerItem.stack -= playerItem.stack;
                // overflow prevention
                if (item.stack > maxStorage)
                {
                    int overflow = item.stack - maxStorage;
                    playerItem.stack += overflow;
                    item.stack -= overflow;
                }

                if (playerItem.stack <= 0)
                {
                    playerItem.TurnToAir();
                }
                return true;
            }
            if (!item.IsAir)
            {
                item.stack--;
                Item dropItem = item.Clone();
                dropItem.stack = 1;
                Item.NewItem(new EntitySource_TileInteraction(Main.player[Main.myPlayer], i, j), new Rectangle(i * 16, j * 16, 32, 32), dropItem);
                if (item.stack <= 0)
                {
                    item.TurnToAir();
                }
                return true;
            }
            return false;
        }
    }
}
