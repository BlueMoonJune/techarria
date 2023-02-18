using Microsoft.Xna.Framework;
using System;
using Techarria.Content.Dusts;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;

namespace Techarria.Content.Tiles.Machines
{
    public class StirlingEngineTE : ModTileEntity
    {
        public static int STIRLING_GENERATION = 1;
        // used for randTime
        public static int LOW_SECONDS = 63;
        // 1 lower than what is set
        public static int HIGH_SECONDS = 189;
        public bool candleLit;
        public int frames;
        public int randTime;
        public override bool IsTileValidForEntity(int x, int y)
        {
            return Main.tile[x, y].TileType == ModContent.TileType<StirlingEngine>();
        }

        public override void Update()
        {
            if (frames >= randTime)
            {
                Main.NewText("stirling turned off");
                candleLit = false;

                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        Tile tile = Main.tile[Position.X + i, Position.Y + j];
                        tile.TileFrameX = (short)(i * 18);
                    }
                }

                frames = 0;
            }

            if (candleLit)
            {
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        Tile tile = Main.tile[Position.X + i, Position.Y + j];
                        tile.TileFrameX = (short)(54 + 18 * i);
                    }
                }

                Power.TransferCharge(STIRLING_GENERATION, Position.X, Position.Y, 3, 2);
                frames++;

            } else if (!candleLit)
            {
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        Tile tile = Main.tile[Position.X + i, Position.Y + j];
                        tile.TileFrameX = (short)(i * 18);
                    }
                }
            }
        }

        public override void SaveData(TagCompound tag)
        {
            tag.Add("candleLit", candleLit);
            base.SaveData(tag);
        }

        public override void LoadData(TagCompound tag)
        {
            candleLit = tag.GetBool("candleLit");
            base.LoadData(tag);
        }
    }


    public class StirlingEngine : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileLavaDeath[Type] = false;
            Main.tileNoAttach[Type] = true;
            Main.tileFrameImportant[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;
            TileID.Sets.IgnoredByNpcStepUp[Type] = true; // This line makes NPCs not try to step up this tile during their movement. Only use this for furniture with solid tops.

            DustType = ModContent.DustType<Wormhole>();
            AdjTiles = new int[] { TileID.Tables };

            // Placement
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.addTile(Type);

            // Etc
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Stirling Engine");
            AddMapEntry(new Color(200, 200, 200), name);
        }

        public StirlingEngineTE GetTileEntity(int i, int j)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            i -= tile.TileFrameX / 18 % 3;
            j -= tile.TileFrameY / 18 % 2;
            return TileEntity.ByPosition[new Point16(i, j)] as StirlingEngineTE;
        }
        public override void PlaceInWorld(int i, int j, Item item)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            i -= tile.TileFrameX / 18 % 3;
            j -= tile.TileFrameY / 18 % 2;
            ModContent.GetInstance<StirlingEngineTE>().Place(i, j);
            StirlingEngineTE tileEntity = GetTileEntity(i, j);
            tileEntity.candleLit = true;
            tileEntity.randTime = new Random().Next(StirlingEngineTE.LOW_SECONDS, StirlingEngineTE.HIGH_SECONDS) * 60;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            StirlingEngineTE tileEntity = GetTileEntity(i, j);
            Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 48, 64, ModContent.ItemType<Items.Placeables.Machines.StirlingEngine>());
            ModContent.GetInstance<StirlingEngineTE>().Kill(i, j);
        }

        public override void HitWire(int i, int j)
        {
            StirlingEngineTE tileEntity = GetTileEntity(i, j);
            tileEntity.candleLit = !tileEntity.candleLit;
            if (tileEntity.candleLit)
            {
                tileEntity.randTime = new Random().Next(StirlingEngineTE.LOW_SECONDS, StirlingEngineTE.HIGH_SECONDS) * 60;
            }
        }

        public override bool RightClick(int i, int j)
        {
            StirlingEngineTE tileEntity = GetTileEntity((int)i, j);
            
            tileEntity.candleLit = !tileEntity.candleLit;

            if (tileEntity.candleLit)
            {
                tileEntity.randTime = new Random().Next(StirlingEngineTE.LOW_SECONDS, StirlingEngineTE.HIGH_SECONDS) * 60;
            }

            return true;
        }

        public override void MouseOver(int i, int j)
        {
            StirlingEngineTE tileEntity = GetTileEntity(i, j);
            Player player = Main.LocalPlayer;
            player.noThrow = 2;

            player.cursorItemIconEnabled = true;
            player.cursorItemIconText = "" + tileEntity.candleLit;
            player.cursorItemIconID = ItemID.Candle;
        }
    }
}
