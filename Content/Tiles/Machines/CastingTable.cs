using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Techarria.Content.Dusts;
using Techarria.Content.Items.Materials;
using Techarria.Content.Items.Materials.Molten;
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
	public class CastingTableRecipe
	{
		public static List<CastingTableRecipe> recipes = new();

		public int input;
		public int output;
		public int mold;
		public int temperature;

		public CastingTableRecipe(int _input, int _output, int _mold, int _temperature) {
			input = _input;
			output = _output;
			mold = _mold;
			temperature = _temperature;

			Recipe.Create(output)
				.AddIngredient(input)
				.AddIngredient<Temperature>(temperature)
				.AddTile<CastingTable>()
				.Register();

			recipes.Add(this);
		}
	}

	public class CastingTableTE : InventoryTileEntity
	{
		public int oldY = 0;
		public Item item = new();
		public Item mold = new();
		public float baseTemp = 25f;
		public float temp = 25f;

        public override Item[] ExtractableItems => new Item[] { item };

        public override bool IsTileValidForEntity(int x, int y) {
			return Main.tile[x, y].TileType == ModContent.TileType<CastingTable>();
		}

		public override bool InsertItem(Item i) {
			if (i.ModItem is MoltenBlob blob) {

				if (item.IsAir && !mold.IsAir) {
					temp = blob.temp;
					item = i.Clone();
					item.stack = 1;
					return true;
				}
			}
			return false;
		}


		public override void Update() {

			ulong a = 18446744073709551615;

            if (oldY != Position.Y) {
				oldY = Position.Y;
				baseTemp = HelperMethods.GetBaseTemp(Position.Y);
			}

			temp = (temp - baseTemp) * ((150f) / (151f)) + baseTemp;

			foreach (CastingTableRecipe recipe in CastingTableRecipe.recipes) {
				if (item.type == recipe.input && temp <= recipe.temperature) {
					item = new Item(recipe.output);
				}
			}
		}

		public override void SaveData(TagCompound tag) {
			tag.Add("item", item);
			tag.Add("mold", mold);
			tag.Add("temp", temp);
			base.SaveData(tag);
		}

		public override void LoadData(TagCompound tag) {
			item = tag.Get<Item>("item");
			mold = tag.Get<Item>("mold");
			temp = tag.GetFloat("temp");
			base.LoadData(tag);
		}
	}

	// Where the TE ends and the Tile starts
	public class CastingTable : EntityTile<CastingTableTE>
	{
		public override void PreStaticDefaults() {
			Main.tileLavaDeath[Type] = false;
			Main.tileNoAttach[Type] = true;
			Main.tileFrameImportant[Type] = true;
			TileID.Sets.DisableSmartCursor[Type] = true;
			TileID.Sets.IgnoredByNpcStepUp[Type] = true; // This line makes NPCs not try to step up this tile during their movement. Only use this for furniture with solid tops.

			DustType = ModContent.DustType<Spikesteel>();
			AdjTiles = new int[] { TileID.Tables };

			width = 2;
			height = 1;

			// Etc
			LocalizedText name = CreateMapEntryName();
			// name.SetDefault("Player Interface");
			AddMapEntry(new Color(200, 200, 200), name);
		}

		public static bool AcceptsItem(Item item) {
			return item.type == ItemID.Gel || item.type == ItemID.PinkGel;
		}

		public override bool RightClick(int i, int j) {
			CastingTableTE tileEntity = GetTileEntity(i, j);
			Item item = tileEntity.item;
			Item playerItem;
			if (!Main.mouseItem.IsAir) {
				playerItem = Main.mouseItem;
			}
			else {
				playerItem = Main.player[Main.myPlayer].HeldItem;
			}

			if (playerItem.ModItem is Mold && tileEntity.mold.IsAir) {
				tileEntity.mold = playerItem.Clone();
				playerItem.TurnToAir();
				return true;
			}
			if (playerItem.ModItem is MoltenBlob) {
				Item tempItem = playerItem.Clone();
				tempItem.stack = 1;
				if (tileEntity.InsertItem(tempItem)) {
					playerItem.stack--;
					if (playerItem.stack <= 0) {
						playerItem.TurnToAir();
					}
					return true;
				}
			}
			if (!tileEntity.item.IsAir) {
				Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 16, tileEntity.item);
				tileEntity.item.TurnToAir();
				return true;
			}

			if (!tileEntity.mold.IsAir) {
				Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 16, tileEntity.mold);
				tileEntity.mold.TurnToAir();
				return true;
			}

			return false;
		}
		public override void MouseOver(int i, int j) {
			CastingTableTE tileEntity = GetTileEntity(i, j);
			Item item = tileEntity.item;
			Player player = Main.LocalPlayer;
			player.noThrow = 2;
			player.cursorItemIconEnabled = true;
			player.cursorItemIconID = -1;
			if (!tileEntity.mold.IsAir) {
				player.cursorItemIconID = tileEntity.mold.type;
			}
			if (!item.IsAir && item.ModItem is not MoltenBlob) {
				player.cursorItemIconID = item.type;
			}
			player.cursorItemIconText = $"{tileEntity.temp:0.00}ºC";
		}
	}
}
