using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Techarria.Content.Dusts;
using Techarria.Content.Items.RecipeItems;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;

namespace Techarria.Content.Tiles.Machines
{

	public class ArcFurnaceTE : ModTileEntity
	{
		public float baseTemp = 25;

		public Item output = new();
		public List<Item> inputs = new();

		public int charge = 0;

		public override bool IsTileValidForEntity(int x, int y) {
			return Main.tile[x, y].TileType == ModContent.TileType<ArcFurnace>();
		}

		public override void Update() 
		{
			/*
			if (oldY != Position.Y) {
				oldY = Position.Y;
				baseTemp = HelperMethods.GetBaseTemp(Position.Y);
			}
			temp = (temp - baseTemp) * ((449f) / (450f)) + baseTemp;
			*/
		}

		public void Craft() {
		}

		public Recipe GetRecipe() {
			return null;
		}

		public bool InsertItem(Item item) {
			return false;
		}

		public Item ExtractItem() {
			if (inputs.Count > 0) {
				Item extracted = inputs[0].Clone();
				inputs[0].stack--;
				if (inputs[0].stack <= 0) {
					inputs.RemoveAt(0);
				}
				extracted.stack = 1;
				return extracted;
			}

			return null;
		}

		public override void SaveData(TagCompound tag) {
			tag.Add("inputs", inputs);
			base.SaveData(tag);
		}

		public override void LoadData(TagCompound tag) {
			inputs = tag.Get<List<Item>>("inputs");
			base.LoadData(tag);
		}
	}

	public class ArcFurnace : PowerConsumer
	{
		public override void SetStaticDefaults() {
			Main.tileNoAttach[Type] = true;
			Main.tileFrameImportant[Type] = true;
			Main.tileLavaDeath[Type] = false;
			TileID.Sets.DisableSmartCursor[Type] = true;

			DustType = ModContent.DustType<Wormhole>();
			AdjTiles = new int[] { TileID.Tables };

			// Placement
			TileObjectData.newTile.CopyFrom(TileObjectData.Style5x4);
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.LavaDeath = false;
			TileObjectData.addTile(Type);

			// Etc
			LocalizedText name = CreateMapEntryName();
			// name.SetDefault("Blast Furnace");
			AddMapEntry(new Color(200, 200, 200), name);
		}

		public static ArcFurnaceTE GetTileEntity(int i, int j) {
			Tile tile = Framing.GetTileSafely(i, j);
			i -= tile.TileFrameX / 18 % 5;
			j -= tile.TileFrameY / 18 % 4;
			return TileEntity.ByPosition[new Point16(i, j)] as ArcFurnaceTE;
		}

		public override void PlaceInWorld(int i, int j, Item item) {
			Tile tile = Framing.GetTileSafely(i, j);
			i -= tile.TileFrameX / 18 % 5;
			j -= tile.TileFrameY / 18 % 4;
			ModContent.GetInstance<ArcFurnaceTE>().Place(i, j);
		}

		public override void KillMultiTile(int i, int j, int frameX, int frameY) {

			ArcFurnaceTE tileEntity = GetTileEntity(i, j);
			foreach (Item input in tileEntity.inputs) {
				Item.NewItem(new EntitySource_TileBreak(i, j), new Rectangle(i * 16, j * 16, 48, 64), input);
			}

			ModContent.GetInstance<ArcFurnaceTE>().Kill(i, j);
		}

		public override bool RightClick(int i, int j) {
			ArcFurnaceTE tileEntity = GetTileEntity(i, j);
			Point16 subTile = new Point16(i, j) - tileEntity.Position;
			if (subTile.X == 1 && subTile.Y <= 1) {
				Item playerItem;
				if (!Main.mouseItem.IsAir)
					playerItem = Main.mouseItem;
				else
					playerItem = Main.player[Main.myPlayer].HeldItem;

				if (tileEntity.InsertItem(playerItem)) {
					if (--playerItem.stack <= 0) {
						playerItem.TurnToAir();
					}
					return true;
				}

				Item item = tileEntity.ExtractItem();
				if (item != null) {
					Item.NewItem(new EntitySource_TileInteraction(Main.player[Main.myPlayer], i, j), new Rectangle(i * 16, j * 16, 16, 16), item);
				}
			}
			if (subTile.X != 1 && subTile.Y == 3) {
				Recipe recipe = tileEntity.GetRecipe();
				if (recipe == null) {
					return false;
				}
				Item.NewItem(new EntitySource_TileInteraction(Main.player[Main.myPlayer], i, j), new Rectangle(i * 16, j * 16, 16, 16), recipe.createItem.type);
				tileEntity.Craft();
			}


			return false;

		}
		public override void MouseOver(int i, int j) {
			ArcFurnaceTE tileEntity = GetTileEntity(i, j);
			Point16 subTile = new Point16(i, j) - tileEntity.Position;
			Player player = Main.LocalPlayer;
		}

		public override void InsertPower(int i, int j, int amount) {
			ArcFurnaceTE tileEntity = GetTileEntity(i, j);

			tileEntity.charge += amount;
		}

		public override void PostDraw(int i, int j, SpriteBatch spriteBatch) {
			/*
			ArcFurnaceTE tileEntity = GetTileEntity(i, j);
			Point16 subTile = new Point16(i, j) - tileEntity.Position;
			if (subTile.X == 1 && subTile.Y == 3) {
				float temp = tileEntity.temp;

				Vector2 TileOffset = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
				Vector2 pos = new Vector2(i, j) * 16 - Main.screenPosition + TileOffset;

				Rectangle sourceRect = new Rectangle(0, 30 - (int)temp / 500 * 2, 16, (int)temp / 500 * 2);
				Rectangle destRect = new Rectangle((int)pos.X, (int)pos.Y - 16 + 30 - (int)temp / 500 * 2, 16, (int)temp / 500 * 2);
				spriteBatch.Draw(ModContent.Request<Texture2D>("Techarria/Content/Tiles/Machines/ArcFurnace_Overlay").Value, destRect, sourceRect, Lighting.GetColor(i, j));

			}*/

		}
	}
}