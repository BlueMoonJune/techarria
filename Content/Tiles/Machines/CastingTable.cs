﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Techarria.Content.Dusts;
using Techarria.Content.Items.Materials;
using Techarria.Content.Items.Materials.Molten;
using Techarria.Content.Items.RecipeItems;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;

namespace Techarria.Content.Tiles.Machines
{
	public class CastingTableRecipe
	{
		public static List<CastingTableRecipe> recipes = new List<CastingTableRecipe>();

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

	public class CastingTableTE : ModTileEntity
	{
		public Item item = new Item();
		public Item mold = new Item();
		public static float baseTemp = 25f;
		public float temp = baseTemp;

		public override bool IsTileValidForEntity(int x, int y) {
			return Main.tile[x, y].TileType == ModContent.TileType<CastingTable>();
		}

		public bool LeftCovered() {
			Tile LeftTile = Main.tile[Position.X, Position.Y - 1];
			return
				LeftTile.HasTile &&
				Main.tileSolid[LeftTile.TileType] &&
				!Main.tileSolidTop[LeftTile.TileType] &&
				!LeftTile.IsActuated
			;
		}

		public bool RightCovered() {
			Tile RightTile = Main.tile[Position.X + 1, Position.Y - 1];
			return
				RightTile.HasTile &&
				Main.tileSolid[RightTile.TileType] &&
				!Main.tileSolidTop[RightTile.TileType] &&
				!RightTile.IsActuated
			;
		}

		public bool IsCovered() {
			return RightCovered() || LeftCovered();
		}

		public bool IsFullyCovered() {
			return LeftCovered() && RightCovered();
		}

		public bool InsertMolten(Item i) {
			if (i.ModItem is MoltenBlob blob) {
				if (!IsFullyCovered()) {
					WorldGen.PlaceLiquid(Position.X, Position.Y - 1, LiquidID.Lava, 32);
					WorldGen.PlaceLiquid(Position.X + 1, Position.Y - 1, LiquidID.Lava, 32);
					temp = Math.Max(blob.temp, temp);
					return true;

				}

				if (item.IsAir && !mold.IsAir) {
					temp = blob.temp;
					item = i.Clone();
					return true;
				}
			}
			return false;
		}

		public override void Update() {
			temp = (temp - baseTemp) * ((1500f - baseTemp) / (1501f - baseTemp)) + baseTemp;

			foreach (CastingTableRecipe recipe in CastingTableRecipe.recipes) {
				if (item.type == recipe.input && temp <= recipe.temperature) {
					item = new Item(recipe.output);
				}
			}
		}

		public override void SaveData(TagCompound tag) {
			tag.Add("item", item);
			tag.Add("mold", mold);
			base.SaveData(tag);
		}

		public override void LoadData(TagCompound tag) {
			item = tag.Get<Item>("item");
			mold = tag.Get<Item>("mold");
			if (item.ModItem is MoltenBlob blob)
				temp = blob.temp;
			else
				temp = baseTemp;
			base.LoadData(tag);
		}
	}

	// Where the TE ends and the Tile starts
	public class CastingTable : ModTile
	{
		public override void SetStaticDefaults() {
			Main.tileLavaDeath[Type] = false;
			Main.tileNoAttach[Type] = true;
			Main.tileFrameImportant[Type] = true;
			TileID.Sets.DisableSmartCursor[Type] = true;
			TileID.Sets.IgnoredByNpcStepUp[Type] = true; // This line makes NPCs not try to step up this tile during their movement. Only use this for furniture with solid tops.

			DustType = ModContent.DustType<Wormhole>();
			AdjTiles = new int[] { TileID.Tables };

			// Placement
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x1);
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.LavaDeath = false;
			TileObjectData.addTile(Type);

			// Etc
			ModTranslation name = CreateMapEntryName();
			name.SetDefault("Player Interface");
			AddMapEntry(new Color(200, 200, 200), name);
		}

		public static CastingTableTE GetTileEntity(int i, int j) {
			Tile tile = Framing.GetTileSafely(i, j);
			i -= tile.TileFrameX / 18 % 2;
			j -= tile.TileFrameY / 18 % 1;
			return TileEntity.ByPosition[new Point16(i, j)] as CastingTableTE;
		}

		public override void PlaceInWorld(int i, int j, Item item) {
			Tile tile = Framing.GetTileSafely(i, j);
			i -= tile.TileFrameX / 18 % 2;
			j -= tile.TileFrameY / 18 % 1;
			ModContent.GetInstance<CastingTableTE>().Place(i, j);
		}

		public override void KillMultiTile(int i, int j, int frameX, int frameY) {
			CastingTableTE tileEntity = GetTileEntity(i, j);
			Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 16, ModContent.ItemType<Items.Placeables.Machines.CastingTable>());
			Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 16, tileEntity.item);
			Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 16, tileEntity.mold);
			ModContent.GetInstance<CastingTableTE>().Kill(i, j);
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
				if (tileEntity.InsertMolten(tempItem)) {
					playerItem.stack--;
					if (playerItem.stack <= 0) {
						playerItem.TurnToAir();
					}
					return true;
				}
			}
			if (!tileEntity.item.IsAir && !tileEntity.IsCovered()) {
				Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 16, tileEntity.item);
				tileEntity.item.TurnToAir();
				return true;
			}

			if (!tileEntity.mold.IsAir && !tileEntity.IsCovered()) {
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
			player.cursorItemIconText = tileEntity.temp + "ºC";
		}
	}
}