﻿using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
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

	public class ArcFurnaceTE : InventoryTileEntity
	{
		public Item output = new();
		public List<Item> inputs = new();

		public override Item[] ExtractableItems => new Item[] { output };

        public override Item[] AllItems 
		{
			get
			{
				Item[] result = new Item[inputs.Count+1];
				result[0] = output;
				inputs.CopyTo(result, 1);
				return result;
			}
		}

		public int displayCycle = 0;

		public override bool IsTileValidForEntity(int x, int y) {
			return Main.tile[x, y].TileType == ModContent.TileType<ArcFurnace>();
		}

		public void InsertCharge(int amount) 
		{
			foreach (ArcFurnaceRecipe recipe in ArcFurnaceRecipe.recipes) {
				if (recipe.CanCraft(inputs, amount)) {
					Item result = recipe.Craft(inputs);
					if (result.type == output.type) {
						output.stack += result.stack;
					} else {
						output = result;
					}
				}
			}
		}

        public override void Update()
        {
            displayCycle++;
        }

        public override bool InsertItem(Item item) {

            if (item == null || item.IsAir) return false;
            foreach (Item input in inputs) {
				if (item.type == input.type && input.stack < input.maxStack) {
					input.stack++;
					return true;
				}
			}

			Item copy = item.Clone();
			copy.stack = 1;
			inputs.Add(copy);
			return true;
		}

		public Item TakeItem() {
			Item extracted = null;
            if (output != null && !output.IsAir)
			{ 
				extracted = output.Clone();
				output.TurnToAir();
			}
			else if (inputs.Count > 0) 
			{
				extracted = inputs[0].Clone();
				inputs[0].stack--;
				if (inputs[0].stack <= 0) {
					inputs.RemoveAt(0);
				}
				extracted.stack = 1;
			}

			return extracted;
		}

		public bool PutItem(Item item)
		{
			if (item == null || item.IsAir || item.favorited) return false;
			foreach (Item input in inputs)
			{
				if (input.type == item.type)
				{
					input.stack += item.stack;
					item.TurnToAir();
					return true;
				}
			}

            inputs.Add(item.Clone());
			item.TurnToAir();
			return true;
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

	public class ArcFurnace : EntityTile<ArcFurnaceTE>, IPowerConsumer 
	{
		public override void PreStaticDefaults() {
			Main.tileNoAttach[Type] = true;
			Main.tileFrameImportant[Type] = true;
			Main.tileLavaDeath[Type] = false;
			TileID.Sets.DisableSmartCursor[Type] = true;

			DustType = ModContent.DustType<Spikesteel>();
			AdjTiles = new int[] { TileID.Tables };

			width = 5;
			height = 4;

			// Etc
			LocalizedText name = CreateMapEntryName();
			// name.SetDefault("Blast Furnace");
			AddMapEntry(new Color(200, 200, 200), name);
		}

		public override bool RightClick(int i, int j) {
			ArcFurnaceTE tileEntity = GetTileEntity(i, j);
			Point16 subTile = new Point16(i, j) - tileEntity.Position;
			Item playerItem;
			if (!Main.mouseItem.IsAir)
				playerItem = Main.mouseItem;
			else
				playerItem = Main.player[Main.myPlayer].HeldItem;

			if (tileEntity.PutItem(playerItem)) {
				return true;
			}

			Item item = tileEntity.TakeItem();
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

		public void InsertPower(int i, int j, int amount) {
			GetTileEntity(i, j).InsertCharge(amount);
		}
	}
}