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
	public class BlastFurnaceRecipe
    {
		public static List<BlastFurnaceRecipe> recipes = new List<BlastFurnaceRecipe>();

		public List<int> ingredients;
		public int result;
		public float temp;
		public bool molten;

		public BlastFurnaceRecipe(List<int> ing, int res, float t, bool m)
        {
			ingredients = ing;
			result = res;
			temp = t;
			molten = m;
        }
    }

    public class BlastFurnaceTE : ModTileEntity
    {
		public Item output = new Item();
		public List<Item> inputs = new List<Item>();
		public float progress = 0;
		public float temp;
		public int frame = 0;
		public static Rectangle particleRect = new Rectangle(6, 6, 24, 16);

		public int cooldown;

        public override bool IsTileValidForEntity(int x, int y)
        {
            return Main.tile[x, y].TileType == ModContent.TileType<BlastFurnace>();
        }

        public override void Update()
        {
			temp = temp * (1500f / 1501f);
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

			ItemDrop = ModContent.ItemType<Items.Placeables.BlastFurnace>();
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
			Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 48, 64, ModContent.ItemType<Items.Placeables.ExampleTable>());
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
    }
}
