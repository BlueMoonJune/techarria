using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Techarria.Content.Dusts;
using Techarria.Content.Items.RecipeItems;
using Techarria.Content.Tiles;
using Techarria.Content.Tiles.Machines;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;

namespace Techarria.Content.Tiles.Machines
{
    public class CrystalGrowthChamberTE : ModTileEntity
    {
        public int powerNeeded;
        public float progress = 0;
        public Item item = new();

        public Dictionary<int, int> gemPowerCost = new Dictionary<int, int>()
        {
            {ItemID.Diamond, 10000},
            {ItemID.LifeCrystal, 50000 }
        };
        public override bool IsTileValidForEntity(int x, int y)
        {
            return Main.tile[x, y].TileType == ModContent.TileType<CrystalGrowthChamber>();
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

    public class CrystalGrowthChamber : EntityTile<CrystalGrowthChamberTE>, IPowerConsumer
    {
        public override void SetStaticDefaults()
        {
            Main.tileNoAttach[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = false;
            TileID.Sets.DisableSmartCursor[Type] = true;
            TileID.Sets.IgnoredByNpcStepUp[Type] = true; // This line makes NPCs not try to step up this tile during their movement. Only use this for furniture with solid tops.

            DustType = DustID.IceGolem;

            // Placement
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x4);
            TileObjectData.newTile.StyleHorizontal = false;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.addTile(Type);

            // Etc
            LocalizedText name = CreateMapEntryName();
            // name.SetDefault("Cryo Chamber");
            AddMapEntry(new Color(200, 200, 200), name);
        }

        public static CrystalGrowthChamberTE GetTileEntity(int i, int j)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            i -= tile.TileFrameX / 18 % 3;
            j -= tile.TileFrameY / 18 % 4;
            return TileEntity.ByPosition[new Point16(i, j)] as CrystalGrowthChamberTE;
        }

        public override void PlaceInWorld(int i, int j, Item item)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            i -= tile.TileFrameX / 18 % 3;
            j -= tile.TileFrameY / 18 % 4;
            ModContent.GetInstance<CrystalGrowthChamberTE>().Place(i, j);
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {

            CrystalGrowthChamberTE tileEntity = GetTileEntity(i, j);
            Item.NewItem(new EntitySource_TileBreak(i, j), new Rectangle(i * 16, j * 16, 48, 64), tileEntity.item.type, tileEntity.item.stack);

            ModContent.GetInstance<CrystalGrowthChamberTE>().Kill(i, j);
        }

        public override bool RightClick(int i, int j)
        {
            CrystalGrowthChamberTE tileEntity = GetTileEntity(i, j);
            Item item = tileEntity.item;
            Point16 subTile = new Point16(i, j) - tileEntity.Position;
            // click position
            if (subTile.X <= 2 && subTile.Y <= 2)
            {
                Item playerItem;
                if (!Main.mouseItem.IsAir)
                    playerItem = Main.mouseItem;
                else
                    playerItem = Main.player[Main.myPlayer].HeldItem;

                if (item.IsAir)
                {
                    // checks for valid item and inserts it
                    if (tileEntity.gemPowerCost.Keys.Contains(playerItem.type))
                    {
                        item = playerItem.Clone();
                        item.stack = 1;
                        tileEntity.item = item;
                        playerItem.stack--;


                        // determines how much power is needed for 1 growth
                        tileEntity.powerNeeded = tileEntity.gemPowerCost[playerItem.type];
                        Main.NewText(tileEntity.powerNeeded);

                        // clears player item
                        if (playerItem.stack <= 0)
                        {
                            playerItem.TurnToAir();
                        }
                        return true;

                    }
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
                        // data resetting
                        tileEntity.progress = 0;
                        tileEntity.powerNeeded = 0;
                    }
                    return true;
                }
            }

            return false;

        }
        public override void MouseOver(int i, int j)
        {
            CrystalGrowthChamberTE tileEntity = GetTileEntity(i, j);

            Item item = tileEntity.item;
            Point16 subTile = new Point16(i, j) - tileEntity.Position;
            Player player = Main.LocalPlayer;

            Item playerItem;

            if (!Main.mouseItem.IsAir)
                playerItem = Main.mouseItem;
            else
                playerItem = Main.player[Main.myPlayer].HeldItem;

            player.noThrow = 2;
            if (subTile.X <= 2 && subTile.Y >= 3)
            {
                player.cursorItemIconEnabled = true;
                player.cursorItemIconText = tileEntity.progress + " / " + tileEntity.powerNeeded;
                player.cursorItemIconID = ModContent.ItemType<Items.RecipeItems.Power>();
            }

            if (item != null && !item.IsAir && subTile.X <= 2 && subTile.Y <= 2)
            {
                player.cursorItemIconEnabled = true;
                player.cursorItemIconText = item.stack + "";
                player.cursorItemIconID = item.type;
            }
        }

        public void InsertPower(int i, int j, int amount)
        {
            CrystalGrowthChamberTE tileEntity = GetTileEntity(i, j);

            if (!tileEntity.item.IsAir && tileEntity.powerNeeded > 0)
            {
                tileEntity.progress += amount;
                while (tileEntity.progress >= tileEntity.powerNeeded)
                {

                    Console.WriteLine(tileEntity.progress);

                    tileEntity.item.stack++;
                    
                    tileEntity.progress -= tileEntity.powerNeeded;
                }
            }
        }
    }
}
