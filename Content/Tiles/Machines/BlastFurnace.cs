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

	public class BlastFurnaceTE : ModTileEntity
	{
		public float baseTemp = 25;

		public Item output = new();
		public List<Item> inputs = new();
		public float progress = 0;
		public float temp = 25f;
		public int frame = 0;
		public static Rectangle particleRect = new(6, 6, 24, 16);

		public int oldY = 0;

		public int displayCycle;

		public override bool IsTileValidForEntity(int x, int y) {
			return Main.tile[x, y].TileType == ModContent.TileType<BlastFurnace>();
		}

		public override void Update() 
		{
			if (oldY != Position.Y) {
				oldY = Position.Y;
				baseTemp = HelperMethods.GetBaseTemp(Position.Y);
			}
			temp = (temp - baseTemp) * ((449f) / (450f)) + baseTemp;
		}

		public void Craft() {
			Recipe recipe = GetRecipe();
			foreach (Item ingredient in recipe.requiredItem) {
				bool useRecipeGroup = false;
				foreach (Item item in inputs) {
					if (RecipeGroupMatch(recipe, item.type, ingredient.type)) {
						item.stack -= ingredient.stack;
						useRecipeGroup = true;
					}
				}

				if (!useRecipeGroup) {
					foreach (Item item in inputs) {
						if (item.type == ingredient.type) {
							item.stack -= ingredient.stack;
						}
					}
				}
			}

			var filteredInputs = new List<Item>();
			foreach (Item item in inputs) {
				if (item.stack > 0) {
					filteredInputs.Add(item);
				}
			}
			inputs = filteredInputs;
		}

		public Recipe GetRecipe() {
			foreach (Recipe recipe in Main.recipe) {

				if (!recipe.HasIngredient<Temperature>()) { continue; }
				if (!recipe.HasTile<BlastFurnace>()) { continue; }


				var list = new List<Item>();
				list = Power.Concat(list, inputs);
				list.Add(new Item(ModContent.ItemType<Temperature>(), (int)temp));

				bool availableRecipe = true;

				foreach (Item ingredient in recipe.requiredItem) {

					int stack = ingredient.stack;
					bool useRecipeGroup = false;
					foreach (Item item in list) {
						if (RecipeGroupMatch(recipe, item.type, ingredient.type)) {
							stack -= item.stack;
							useRecipeGroup = true;
						}
					}

					if (!useRecipeGroup) {
						foreach (Item item in list) {
							if (item.type == ingredient.type) {
								stack -= item.stack;
							}
						}
					}

					if (stack > 0) {
						availableRecipe = false;
					}
				}
				if (availableRecipe) {
					return recipe;
				}

			}
			return null;
		}

		public bool RecipeGroupMatch(Recipe recipe, int inventoryType, int requiredType) {
			foreach (int num in recipe.acceptedGroups) {
				RecipeGroup recipeGroup = RecipeGroup.recipeGroups[num];
				if (recipeGroup.ContainsItem(inventoryType) && recipeGroup.ContainsItem(requiredType))
					return true;
			}
			return false;
		}

		public bool InsertItem(Item item) {
			int itemCount = 10;
			foreach (Item input in inputs) {
				itemCount += input.stack;
			}

			temp = (temp * itemCount + baseTemp) / (itemCount + 1f);

			foreach (Item input in inputs) {
				if (item.type == input.type && input.stack < input.maxStack) {
					input.stack++;
					return true;
				}
			}

			if (item != null && !item.IsAir) {
				foreach (Recipe recipe in Main.recipe) {
					if (!recipe.HasIngredient<Temperature>())
						continue;

					foreach (Item ingredient in inputs) {
						if (!recipe.HasIngredient(ingredient.type))
							goto failed;
					}

					if (recipe.HasIngredient(item.type)) {
						Item input = item.Clone();
						input.stack = 1;
						inputs.Add(input);
						return true;
					}

				failed:
					continue;
				}
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
			tag.Add("temp", temp);
			base.SaveData(tag);
		}

		public override void LoadData(TagCompound tag) {
			inputs = tag.Get<List<Item>>("inputs");
			temp = tag.GetFloat("temp");
			base.LoadData(tag);
		}
	}

	public class BlastFurnace : PowerConsumer
	{
		public override void SetStaticDefaults() {
			Main.tileNoAttach[Type] = true;
			Main.tileFrameImportant[Type] = true;
			Main.tileLavaDeath[Type] = false;
			TileID.Sets.DisableSmartCursor[Type] = true;

			DustType = ModContent.DustType<Wormhole>();
			AdjTiles = new int[] { TileID.Tables };

			// Placement
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x4);
			TileObjectData.newTile.StyleHorizontal = false;
			TileObjectData.newTile.LavaDeath = false;
			TileObjectData.addTile(Type);

			// Etc
			LocalizedText name = CreateMapEntryName();
			// name.SetDefault("Blast Furnace");
			AddMapEntry(new Color(200, 200, 200), name);
		}

		public static BlastFurnaceTE GetTileEntity(int i, int j) {
			Tile tile = Framing.GetTileSafely(i, j);
			i -= tile.TileFrameX / 18 % 3;
			j -= tile.TileFrameY / 18 % 4;
			return TileEntity.ByPosition[new Point16(i, j)] as BlastFurnaceTE;
		}

		public override void PlaceInWorld(int i, int j, Item item) {
			Tile tile = Framing.GetTileSafely(i, j);
			i -= tile.TileFrameX / 18 % 3;
			j -= tile.TileFrameY / 18 % 4;
			ModContent.GetInstance<BlastFurnaceTE>().Place(i, j);
		}

		public override void KillMultiTile(int i, int j, int frameX, int frameY) {
			Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 48, 64, ModContent.ItemType<Items.Placeables.Machines.BlastFurnace>());

			BlastFurnaceTE tileEntity = GetTileEntity(i, j);
			foreach (Item input in tileEntity.inputs) {
				Item.NewItem(new EntitySource_TileBreak(i, j), new Rectangle(i * 16, j * 16, 48, 64), input);
			}

			ModContent.GetInstance<BlastFurnaceTE>().Kill(i, j);
		}

		public override bool RightClick(int i, int j) {
			BlastFurnaceTE tileEntity = GetTileEntity(i, j);
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
			BlastFurnaceTE tileEntity = GetTileEntity(i, j);
			Point16 subTile = new Point16(i, j) - tileEntity.Position;
			Player player = Main.LocalPlayer;

			Item playerItem;
			if (!Main.mouseItem.IsAir)
				playerItem = Main.mouseItem;
			else
				playerItem = Main.player[Main.myPlayer].HeldItem;

			player.noThrow = 2;
			if (subTile.X == 1 && subTile.Y >= 2) {
				player.cursorItemIconEnabled = true;
				player.cursorItemIconText = $"{tileEntity.temp:0.00}ºC";
				player.cursorItemIconID = ModContent.ItemType<Temperature>();
			}
			if (subTile.X == 1 && subTile.Y <= 1) {
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
			if (subTile.X != 1 && subTile.Y == 3) {
				Recipe recipe = tileEntity.GetRecipe();
				if (recipe == null)
					return;

				Item result = recipe.createItem;
				player.cursorItemIconEnabled = true;
				player.cursorItemIconID = result.type;
				player.cursorItemIconText = result.Name;

			}
		}

		public override void InsertPower(int i, int j, int amount) {
			BlastFurnaceTE tileEntity = GetTileEntity(i, j);

			int itemCount = 10;
			foreach (Item input in tileEntity.inputs) {
				itemCount += input.stack;
			}

			tileEntity.temp += amount * 100 / (float)itemCount;
		}

		public override void PostDraw(int i, int j, SpriteBatch spriteBatch) {
			BlastFurnaceTE tileEntity = GetTileEntity(i, j);
			Point16 subTile = new Point16(i, j) - tileEntity.Position;
			if (subTile.X == 1 && subTile.Y == 3) {
				float temp = tileEntity.temp;

				Vector2 TileOffset = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
				Vector2 pos = new Vector2(i, j) * 16 - Main.screenPosition + TileOffset;

				Rectangle sourceRect = new Rectangle(0, 30 - (int)temp / 500 * 2, 16, (int)temp / 500 * 2);
				Rectangle destRect = new Rectangle((int)pos.X, (int)pos.Y - 16 + 30 - (int)temp / 500 * 2, 16, (int)temp / 500 * 2);
				spriteBatch.Draw(ModContent.Request<Texture2D>("Techarria/Content/Tiles/Machines/BlastFurnace_Overlay").Value, destRect, sourceRect, Lighting.GetColor(i, j));

			}

		}
	}
}