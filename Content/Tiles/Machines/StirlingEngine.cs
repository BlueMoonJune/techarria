using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Techarria.Content.Dusts;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;
using Terraria.GameContent;

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
        public int animFrame = 0;
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
                    for (int j = 0; j < 3; j++)
                    {
                        Tile tile = Main.tile[Position.X + i, Position.Y + j];
                        tile.TileFrameX = (short)(i * 18);
                    }
                }

                frames = 0;
            }

            if (candleLit)
            {
                animFrame = frames / 4 % 4;
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        Tile tile = Main.tile[Position.X + i, Position.Y + j];
                        tile.TileFrameX = (short)(54 + 18 * i);
                        tile.TileFrameY = (short)(animFrame * 54 + j * 18);
                    }
                }

                Power.TransferCharge(STIRLING_GENERATION, Position.X, Position.Y, 3, 2);
                frames++;

            } else if (!candleLit)
            {
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
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
            tag.Add("frames", frames);
            tag.Add("randTime", randTime);
            base.SaveData(tag);
        }

        public override void LoadData(TagCompound tag)
        {
            candleLit = tag.GetBool("candleLit");
            frames = tag.GetInt("frames");
            randTime = tag.GetInt("randTime");
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
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.addTile(Type);

            // Etc
            LocalizedText name = CreateMapEntryName();
            // name.SetDefault("Stirling Engine");
            AddMapEntry(new Color(200, 200, 200), name);
        }

        public StirlingEngineTE GetTileEntity(int i, int j)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            i -= tile.TileFrameX / 18 % 3;
            j -= tile.TileFrameY / 18 % 3;
            return TileEntity.ByPosition[new Point16(i, j)] as StirlingEngineTE;
        }
        public override void PlaceInWorld(int i, int j, Item item)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            i -= tile.TileFrameX / 18 % 3;
            j -= tile.TileFrameY / 18 % 3;
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

                tileEntity.frames = 0;
            }
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch) {
			StirlingEngineTE tileEntity = GetTileEntity(i, j);
			Point16 subTile = new Point16(i, j) - tileEntity.Position;
			if (subTile.X == 2 && subTile.Y == 1 && tileEntity.candleLit) {

				Vector2 offset = new(4, 8);

				Vector2 TileOffset = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
				Vector2 pos = new Vector2(i, j) * 16 - Main.screenPosition + TileOffset;

				ulong randSeed = Main.TileFrameSeed ^ (ulong)((long)j << 32 | (long)(uint)i); // Don't remove any casts.

				for (int k = 0; k < 7; k++) {
					float xx = Utils.RandomInt(ref randSeed, -10, 11) * 0.15f;
					float yy = Utils.RandomInt(ref randSeed, -10, 1) * 0.35f;

					spriteBatch.Draw(ModContent.Request<Texture2D>("Techarria/Content/Tiles/Machines/IHateModding").Value, pos + new Vector2(xx, yy) + offset, new Color(100, 100, 100, 0));
				}
			}
        }


        public override bool RightClick(int i, int j)
        {
            StirlingEngineTE tileEntity = GetTileEntity((int)i, j);
            
            tileEntity.candleLit = !tileEntity.candleLit;

            if (tileEntity.candleLit)
            {
                tileEntity.randTime = new Random().Next(StirlingEngineTE.LOW_SECONDS, StirlingEngineTE.HIGH_SECONDS) * 60;
            } else
            {
                tileEntity.frames = 0;
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
