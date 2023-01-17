using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Techarria.Content.Dusts;
using Techarria.Content.Items.RecipeItems;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;

namespace Techarria.Content.Tiles
{

    public class BlastFurnaceTE : ModTileEntity
    {
		public Item output = new Item();
		public List<Item> inputs = new List<Item>();
		public float progress = 0;
		public float temp = 25f;
		public int frame = 0;
		public static Rectangle particleRect = new Rectangle(6, 6, 24, 16);

		public int cooldown;

        public override bool IsTileValidForEntity(int x, int y)
        {
            return Main.tile[x, y].TileType == ModContent.TileType<BlastFurnace>();
        }

        public override void Update()
        {
			temp = (temp - 25) * (1475f / 1476f) + 25f;
        }

        public override void SaveData(TagCompound tag)
        {
			tag.Add("inputs", inputs);
			base.SaveData(tag);
        }

        public override void LoadData(TagCompound tag)
        {
			inputs = tag.Get<List<Item>>("inputs");
            base.LoadData(tag);
        }
    }

    public class BlastFurnace : PowerConsumer
    {
		public override void SetStaticDefaults()
		{
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;
			Main.tileFrameImportant[Type] = true;
			TileID.Sets.DisableSmartCursor[Type] = true;
			TileID.Sets.IgnoredByNpcStepUp[Type] = true; // This line makes NPCs not try to step up this tile during their movement. Only use this for furniture with solid tops.

			DustType = ModContent.DustType<Dusts.Wormhole>();
			AdjTiles = new int[] { TileID.Tables };

			// Placement
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x4);
			TileObjectData.newTile.StyleHorizontal = false;
			TileObjectData.addTile(Type);

			// Etc
			ModTranslation name = CreateMapEntryName();
			name.SetDefault("Blast Furnace");
			AddMapEntry(new Color(200, 200, 200), name);
		}

		public static BlastFurnaceTE GetTileEntity(int i, int j)
		{
			Tile tile = Framing.GetTileSafely(i, j);
			i -= tile.TileFrameX / 18 % 3;
			j -= tile.TileFrameY / 18 % 4;
			return TileEntity.ByPosition[new Point16(i, j)] as BlastFurnaceTE;
		}

        public override void PlaceInWorld(int i, int j, Item item)
        {
			Tile tile = Framing.GetTileSafely(i, j);
			i -= tile.TileFrameX / 18 % 3;
			j -= tile.TileFrameY / 18 % 4;
			Main.NewText(i + " " + j);
			ModContent.GetInstance<BlastFurnaceTE>().Place(i, j);
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{
			Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 48, 64, ModContent.ItemType<Items.Placeables.BlastFurnace>());
			ModContent.GetInstance<BlastFurnaceTE>().Kill(i, j);
		}

		public static bool AcceptsItem(Item item)
        {
			return item.type == Terraria.ID.ItemID.Gel || item.type == Terraria.ID.ItemID.PinkGel;
        }

        public override bool RightClick(int i, int j)
        {
			return false;
		}
		public override void MouseOver(int i, int j)
		{
			BlastFurnaceTE tileEntity = GetTileEntity(i, j);
			Point16 subTile = new Point16(i, j) - tileEntity.Position;
			Player player = Main.LocalPlayer;
			player.noThrow = 2;
			if (subTile.X == 1 && subTile.Y >= 2)
            {
				player.cursorItemIconEnabled = true;
				player.cursorItemIconText = tileEntity.temp + "ºC";
				player.cursorItemIconID = ModContent.ItemType<Temperature>();
			}
		}

        public override void InsertPower(int i, int j, int amount)
		{
			BlastFurnaceTE tileEntity = GetTileEntity(i, j);
			tileEntity.temp += amount;
		}

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
		{
			BlastFurnaceTE tileEntity = GetTileEntity(i, j);
			Point16 subTile = new Point16(i, j) - tileEntity.Position;
			if (subTile.X == 1 && subTile.Y == 3)
			{
				float temp = tileEntity.temp;
				Main.NewText("Drawing temp: " + (int)temp / 500);

				Vector2 TileOffset = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
				Vector2 pos = new Vector2(i, j) * 16 - Main.screenPosition + TileOffset;

				Rectangle sourceRect = new Rectangle(0, 30 - (int)temp / 500 * 2, 16, (int)temp / 500 * 2);
				Rectangle destRect = new Rectangle((int)pos.X, (int)pos.Y - 16 + 30 - (int)temp / 500 * 2, 16, (int)temp / 500 * 2);
				spriteBatch.Draw(ModContent.Request<Texture2D>("Techarria/Content/Tiles/BlastFurnace_Overlay").Value, destRect, sourceRect, Lighting.GetColor(i, j));

			}

		}
    }
}
