using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Techarria.Content.Dusts;
using Techarria.Content.Tiles;
using Techarria.Content.Tiles.Machines.Logic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;

namespace Techarria.Content.Tiles.Machines
{
	public class FusionCombinerIngredient {

		public bool isMultiple = false;
		public bool isFluid = false;
		public int type = 0;
		public List<int> types = new();
		public byte amount = 0; // only used for fluids

		public FusionCombinerIngredient(int type, bool isFluid, byte amount = 0) {
			this.type = type;
			this.isFluid = isFluid;
			this.amount = amount;
		}
		public FusionCombinerIngredient(List<int> types, bool isFluid, byte amount = 0) {
			this.types = types;
			this.isFluid = isFluid;
			this.amount = amount;
			this.isMultiple = true;
		}

		public bool hasIngredient(Point p) {
			if (isFluid) {
				if (isMultiple) {
					return types.Contains(Main.tile[p].LiquidType) && Main.tile[p].LiquidAmount > amount;
				}
				return type == Main.tile[p].LiquidType && Main.tile[p].LiquidAmount > amount;
			}
			if (isMultiple) {
				return types.Contains(Main.tile[p].TileType);
			}
			return type == Main.tile[p].TileType;
		}

		public void consumeIngredient(Point p) {
			if (isFluid) {
				Main.tile[p].LiquidAmount -= amount;
				WorldGen.SquareTileFrame(p.X, p.Y);
			} else {
				WorldGen.KillTile(p.X, p.Y, noItem: true);
			}
		}
	}

	public class FusionCombinerRecipe {
		public FusionCombinerIngredient inputA;
		public FusionCombinerIngredient inputB;
		public int result;

		public FusionCombinerRecipe(int result, FusionCombinerIngredient inputA, FusionCombinerIngredient inputB) {
			this.inputA = inputA;
			this.inputB = inputB;
			this.result = result;
		}

		public static List<FusionCombinerRecipe> recipes = new List<FusionCombinerRecipe>() {
			new FusionCombinerRecipe(TileID.Stone, new(LiquidID.Lava, true, 16), new(LiquidID.Water, true, 16))
		};
	};

	public class FusionCombinerTE : ModTileEntity
	{
		public static int REQUIRED_CHARGE = 60;
		public int progress = 0;
		public FusionCombinerRecipe recipe;
		public int status = 0; // 0 = empty, 1 = processing, 2 = output blocked
		public int index;

		public override bool IsTileValidForEntity(int x, int y) {
			return Main.tile[x, y].TileType == ModContent.TileType<FusionCombiner>();
		}

		public void InsertPower(int amount) {
			Point pos = new(Position.X, Position.Y);

			Point inputL = pos + new Point(-1, 1);
			Point inputR = pos + new Point(3, 1);

			Dust.NewDust(new Vector2(inputL.X, inputL.Y), 0, 0, DustID.Adamantite);

			switch (status) {
				case 0: // empty
					foreach (FusionCombinerRecipe recipe in FusionCombinerRecipe.recipes) {
						if (recipe.inputA.hasIngredient(inputL)) {
							if (recipe.inputB.hasIngredient(inputR)) {
								recipe.inputA.consumeIngredient(inputL);
								recipe.inputB.consumeIngredient(inputR);
								this.recipe = recipe;
								status = 1;
								break;
							}
						}

						if (recipe.inputA.hasIngredient(inputR)) {
							if (recipe.inputB.hasIngredient(inputL)) {
								recipe.inputA.consumeIngredient(inputR);
								recipe.inputB.consumeIngredient(inputL);
								this.recipe = recipe;
								status = 1;
								break;
							}
						}
					}
					break;
				case 1:
					progress += amount;
					if (progress >= REQUIRED_CHARGE) {
						status = 2;
						progress = 0;
					}
					break;
				case 2:
					Point output = pos + new Point(1, -1);
					Piston.StaticPush(output.X, output.Y+1, new(3));
					if (!Main.tile[output].HasTile) {
						WorldGen.PlaceTile(output.X, output.Y, recipe.result, forced:true);
						recipe = null;
						status = 0;
					}
					break;
			}
		}

		public override void SaveData(TagCompound tag) {
			tag.Add("progress", progress);
			tag.Add("recipe", FusionCombinerRecipe.recipes.FindIndex(new Predicate<FusionCombinerRecipe>( value => value == recipe)));
			base.SaveData(tag);
		}

		public override void LoadData(TagCompound tag) {
			progress = tag.Get<int>("progress");
			if (tag.Get<int>("recipe") != -1)
				recipe = FusionCombinerRecipe.recipes[tag.Get<int>("recipe")];
			base.LoadData(tag);
		}
	}

	// Where the TE ends and the Tile starts
	public class FusionCombiner : EntityTile<FusionCombinerTE>, IPowerConsumer
	{
		public override void SetStaticDefaults() {
			Main.tileLavaDeath[Type] = false;
			Main.tileNoAttach[Type] = true;
			Main.tileFrameImportant[Type] = true;
			TileID.Sets.DisableSmartCursor[Type] = true;
			Main.tileSolid[Type] = true;

			DustType = ModContent.DustType<Spikesteel>();

			// Placement
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.LavaDeath = false;
			TileObjectData.addTile(Type);

			// Etc
			AddMapEntry(new Color(200, 200, 200), Language.GetText("Gelatinous Turbine"));
		}

		public FusionCombinerTE GetTileEntity(int i, int j) 
		{
			Tile tile = Framing.GetTileSafely(i, j);
			i -= tile.TileFrameX / 18 % 3;
			j -= tile.TileFrameY / 18 % 2;
			return TileEntity.ByPosition[new Point16(i, j)] as FusionCombinerTE;
		}

		public override void PlaceInWorld(int i, int j, Item item) {
			Tile tile = Framing.GetTileSafely(i, j);
			i -= tile.TileFrameX / 18 % 3;
			j -= tile.TileFrameY / 18 % 2;
			ModContent.GetInstance<FusionCombinerTE>().Place(i, j);
		}

		public override void KillMultiTile(int i, int j, int frameX, int frameY) {
			FusionCombinerTE tileEntity = GetTileEntity(i, j);
			ModContent.GetInstance<FusionCombinerTE>().Kill(i, j);
		}
		public override void MouseOver(int i, int j) {
			FusionCombinerTE tileEntity = GetTileEntity(i, j);
		}

		public void InsertPower(int i, int j, int amount) {
			GetTileEntity(i, j).InsertPower(amount);
		}
	}
}
