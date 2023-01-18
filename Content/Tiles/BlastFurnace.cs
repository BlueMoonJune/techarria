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
		public static float baseTemp = 25;

		public Item output = new Item();
		public List<Item> inputs = new List<Item>();
		public float progress = 0;
		public float temp = 25f;
		public int frame = 0;
		public static Rectangle particleRect = new Rectangle(6, 6, 24, 16);

		public int displayCycle;

		public override bool IsTileValidForEntity(int x, int y)
		{
			return Main.tile[x, y].TileType == ModContent.TileType<BlastFurnace>();
		}

		public override void Update()
		{
			temp = (temp - baseTemp) * ((1500f - baseTemp) / (1501f - baseTemp)) + baseTemp;
		}

		public bool InsertItem(Item item)
		{
			item.stack = 1;

			return false;
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
			ModContent.GetInstance<BlastFurnaceTE>().Place(i, j);
		}

		public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{
			Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 48, 64, ModContent.ItemType<Items.Placeables.BlastFurnace>());

			BlastFurnaceTE tileEntity = GetTileEntity(i, j);
			foreach (Item input in tileEntity.inputs)
			{
				Item.NewItem(new EntitySource_TileBreak(i, j), new Rectangle(i * 16, j * 16, 48, 64), input.type, input.stack);
			}

			ModContent.GetInstance<BlastFurnaceTE>().Kill(i, j);
		}

		public override bool RightClick(int i, int j)
		{
			BlastFurnaceTE tileEntity = GetTileEntity(i, j);
			Point16 subTile = new Point16(i, j) - tileEntity.Position;
			if (subTile.X == 1 && subTile.Y <= 1)
			{
				Item playerItem;
				if (!Main.mouseItem.IsAir)
					playerItem = Main.mouseItem;
				else
					playerItem = Main.player[Main.myPlayer].HeldItem;

				foreach (Item input in tileEntity.inputs)
				{
					if (playerItem.type == input.type && input.stack < input.maxStack)
					{
						input.stack++;
						playerItem.stack--;
						if (playerItem.stack <= 0)
							playerItem.TurnToAir();
						return true;
					}
				}

				if (playerItem != null && !playerItem.IsAir)
				{
					foreach (Recipe recipe in Main.recipe)
					{
						if (!recipe.HasIngredient<Temperature>())
							continue;

						foreach (Item ingredient in tileEntity.inputs)
						{
							if (!recipe.HasIngredient(ingredient.type))
								goto failed;
						}

						if (recipe.HasIngredient(playerItem.type))
						{
							Item input = playerItem.Clone();
							input.stack = 1;
							tileEntity.inputs.Add(input);
							playerItem.stack--;
							if (playerItem.stack <= 0)
								playerItem.TurnToAir();
							return true;
						}

					failed: continue;
					}
				}
			}
			if (tileEntity.inputs.Count > 0)
			{
				Item extracted = tileEntity.inputs[0];
				Item.NewItem(new EntitySource_TileInteraction(Main.player[Main.myPlayer], i, j), new Rectangle(i * 16, j * 16, 16, 16), extracted.type);
				extracted.stack--;
				if (extracted.stack <= 0)
				{
					tileEntity.inputs.RemoveAt(0);
				}
				return true;
			}

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
			if (subTile.X == 1 && subTile.Y <= 1)
			{
				List<Item> inputs = tileEntity.inputs;
				if (inputs.Count <= 0)
				{
					return;
				}
				tileEntity.displayCycle++;
				Item item = inputs[tileEntity.displayCycle / 60 % inputs.Count];
				player.cursorItemIconEnabled = true;
				player.cursorItemIconText = item.stack.ToString();
				player.cursorItemIconID = item.type;
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

				Vector2 TileOffset = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
				Vector2 pos = new Vector2(i, j) * 16 - Main.screenPosition + TileOffset;

				Rectangle sourceRect = new Rectangle(0, 30 - (int)temp / 500 * 2, 16, (int)temp / 500 * 2);
				Rectangle destRect = new Rectangle((int)pos.X, (int)pos.Y - 16 + 30 - (int)temp / 500 * 2, 16, (int)temp / 500 * 2);
				spriteBatch.Draw(ModContent.Request<Texture2D>("Techarria/Content/Tiles/BlastFurnace_Overlay").Value, destRect, sourceRect, Lighting.GetColor(i, j));

			}

		}
	}
}