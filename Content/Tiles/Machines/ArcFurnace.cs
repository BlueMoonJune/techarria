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
	public class ArcFurnaceRecipe {
		public static List<ArcFurnaceRecipe> recipes = new();

		public Item result;
		public int voltage = 0;
		public List<RecipeIngredient> ingredients = new();

		public ArcFurnaceRecipe(Item result, int voltage, List<RecipeIngredient> ingredients = null) {
			this.result = result;
			this.voltage = voltage;
			if (ingredients != null) {
				this.ingredients = ingredients;
			}
			recipes.Add(this);
		}

		public ArcFurnaceRecipe AddIngredient(Item item, int count) {
			ingredients.Add(new(item, count));
            return this;
        }

		public ArcFurnaceRecipe AddIngredient(RecipeGroup recipeGroup, int count) {
			ingredients.Add(new(recipeGroup, count));
			return this;
		}

		public ArcFurnaceRecipe AddIngredient(RecipeIngredient ingredient) {
			ingredients.Add(ingredient);
            return this;
        }

		public bool CanCraft(List<Item> items, int voltage) {
			if (voltage < this.voltage) {
				return false;
			}
			foreach (RecipeIngredient ing in ingredients) {
				int countLeft = ing.count;
				foreach (Item item in items) {
					if (ing.AcceptsItem(item)) {
						if (countLeft <= item.stack) {
							countLeft = 0;
							break;
						}
						countLeft -= item.stack;
					}
				}
				if (countLeft > 0) {
					return false;
				}
			}
			return true;
		}

		public Item Craft(List<Item> items) {
			foreach (RecipeIngredient ing in ingredients) {
				int countLeft = ing.count;
				foreach (Item item in items) {
					if (ing.AcceptsItem(item)) {
						if (countLeft <= item.stack) {
							item.stack -= countLeft;
							countLeft = 0;
							if (item.stack <= 0) {
								items.Remove(item);
							}
							break;
						}
						countLeft -= item.stack;
						items.Remove(item);
					}
				}
			}
			return result;
		}

		public Recipe Register()
		{
            Recipe recipe = Recipe.Create(result.type);
			recipe.AddTile<ArcFurnace>();
			foreach (RecipeIngredient item in ingredients)
			{
				if (item.item != null)
				{
					recipe.AddIngredient(item.item.type, item.count);
				} else
                {
                    recipe.AddRecipeGroup(item.recipeGroup, item.count);
                }
			}
			recipe.AddIngredient<Volts>(voltage);
			recipe.Register();
			return recipe;
        }
	}

	public class ArcFurnaceTE : ModTileEntity
	{
		public float baseTemp = 25;

		public Item output = new();
		public List<Item> inputs = new();

		public int charge = 0;

		public int displayCycle = 0;

		public override bool IsTileValidForEntity(int x, int y) {
			return Main.tile[x, y].TileType == ModContent.TileType<ArcFurnace>();
		}

		public override void Update() 
		{
			if (charge > 0) {
				foreach (ArcFurnaceRecipe recipe in ArcFurnaceRecipe.recipes) {
					if (recipe.CanCraft(inputs, charge)) {
						Item result = recipe.Craft(inputs);
						if (result.type == output.type) {
							output.stack += result.stack;
						} else {
							output = result;
						}
					}
				}
			}
			charge = 0;
		}

		public bool InsertItem(Item item, bool decrement = true) {

			foreach (Item input in inputs) {
				if (item.type == input.type && input.stack < input.maxStack) {
					input.stack++;
					if (decrement) {
						item.stack--;
						if (item.stack <= 0) {
							item.TurnToAir();
						}
					}
					return true;
				}
			}

			if (item != null && !item.IsAir) {
				Item copy = item.Clone();
				copy.stack = 1;
				inputs.Add(copy);
				item.stack--;
				if (item.stack <= 0) {
					item.TurnToAir();
				}
				return true;
			}
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

	public class ArcFurnace : PowerConsumer<ArcFurnaceTE>
	{
		public override void SetStaticDefaults() {
			Main.tileNoAttach[Type] = true;
			Main.tileFrameImportant[Type] = true;
			Main.tileLavaDeath[Type] = false;
			TileID.Sets.DisableSmartCursor[Type] = true;

			DustType = ModContent.DustType<Spikesteel>();
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
			Item playerItem;
			if (!Main.mouseItem.IsAir)
				playerItem = Main.mouseItem;
			else
				playerItem = Main.player[Main.myPlayer].HeldItem;

			if (tileEntity.InsertItem(playerItem)) {
				return true;
			}

			Item item = tileEntity.ExtractItem();
			if (item != null) {
				Item.NewItem(new EntitySource_TileInteraction(Main.player[Main.myPlayer], i, j), new Rectangle(i * 16, j * 16, 16, 16), item);
			}
			if (!tileEntity.output.IsAir) {
				Item clone = tileEntity.output.Clone();
				clone.stack = 1;
				tileEntity.output.stack--;
				if (tileEntity.output.stack <= 0) {
					tileEntity.output.TurnToAir();
				}
				Item.NewItem(new EntitySource_TileInteraction(Main.player[Main.myPlayer], i, j), new Rectangle(i * 16, j * 16, 16, 16), clone);
			}

			return false;

		}
		public override void MouseOver(int i, int j) {
			ArcFurnaceTE tileEntity = GetTileEntity(i, j);
			Point16 subTile = new Point16(i, j) - tileEntity.Position;
			Player player = Main.LocalPlayer;

			player.noThrow = 2;

			if (!tileEntity.output.IsAir) {
				player.cursorItemIconEnabled = true;
				player.cursorItemIconText = tileEntity.output.stack.ToString();
				player.cursorItemIconID = tileEntity.output.type;
			}

			Item playerItem;
			if (!Main.mouseItem.IsAir)
				playerItem = Main.mouseItem;
			else
				playerItem = Main.player[Main.myPlayer].HeldItem;

			List<Item> inputs = tileEntity.inputs;

			Item item = null;

			foreach (Item input in inputs) {
				if (playerItem.type == input.type)
					item = input;
			}

			if (item == null) {
				if (inputs.Count <= 0) {
					return;
				}
				tileEntity.displayCycle++;
				item = inputs[tileEntity.displayCycle / 60 % inputs.Count];
			}


			player.cursorItemIconEnabled = true;
			player.cursorItemIconText = item.stack.ToString();
			player.cursorItemIconID = item.type;
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