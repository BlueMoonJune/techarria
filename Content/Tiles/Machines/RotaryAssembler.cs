using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Techarria.Content.Dusts;
using Techarria.Transfer;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;

namespace Techarria.Content.Tiles.Machines
{

	public class AssemblyRecipe {

		public static List<AssemblyRecipe> recipes = new();

		public Item result;
		public RecipeIngredient seed;
		public List<RecipeIngredient>[] segments = new List<RecipeIngredient>[8];
		
		public AssemblyRecipe(Item result, Item seed) {
			this.result = result;
			this.seed = new RecipeIngredient(seed);
			recipes.Add(this);
		}

		public AssemblyRecipe AddItem(Item item, int segment) {
			Item temp = item.Clone();
			temp.stack = 1;
			if (segments[segment] == null) {
				segments[segment] = new List<RecipeIngredient>();
			}
			segments[segment].Add(new RecipeIngredient(temp));
			return this;
		}

		public AssemblyRecipe AddRecipeGroup(RecipeGroup recipeGroup, int segment) {
			segments[segment].Add(new RecipeIngredient(recipeGroup));
			return this;
		}
	}

	public class RotaryAssemblerTE : InventoryTileEntity
	{
		public List<Point> wireHits = new();

		public float degrees = 0;

        public override Item[] ExtractableItems => new Item[] { GetResult() };

        public override Item[] AllItems
		{
			get
			{
				List<Item> items = new List<Item>();
                foreach (List<Item> segment in this.items)
                {
                    if (segment == null) continue;
                    foreach (Item item in segment)
                    {
                        items.Add(item);
                    }
                }
                items.Add(seed);

                return items.ToArray();
            }
		}

        public int step { get { return (int)MathF.Round(degrees / 45f); } }

		public Item seed = new();
		public List<Item>[] items = new List<Item>[8];

		public float degreesLeft = 0;

		public override bool IsTileValidForEntity(int x, int y) {
			return Main.tile[x, y].TileType == ModContent.TileType<RotaryAssembler>();
		}

		public override bool InsertItem(Item item, ContainerInterface info) {
			Item temp = item.Clone();
			temp.stack = 1;
			if (seed.IsAir) {
				seed = temp;
				return true;
			}
			int segment = ((Direction)info.dir).Rotated(2) * 2 - step;
			segment = (segment % 8 + 8) % 8;
			if (items[segment] == null) {
				items[segment] = new List<Item>();
			}
			if (items[segment].Count < 3) {
				items[segment].Add(temp);
				return true;
			}
			return false;
		}

		public override void Update() {
			float temp = degreesLeft * 0.9f;
			degrees += temp - degreesLeft;
			degreesLeft = temp;

			if (wireHits.Count == 0) return;
			Point lp = new Point();
			foreach (Point p in wireHits) {
				if (lp == new Point()) {
					lp = p;
					continue;
				}
				Vector2 offset = new Vector2(p.X, p.Y) - new Vector2(Position.X + 1, Position.Y + 1);
				Vector2 oldOffset = new Vector2(lp.X, lp.Y) - new Vector2(Position.X + 1, Position.Y + 1);
				if (offset != Vector2.Zero && oldOffset != Vector2.Zero) {
					float angle = MathF.Atan2(offset.X, offset.Y) - MathF.Atan2(oldOffset.X, oldOffset.Y);
					if (2 * MathHelper.Pi - angle < angle) {
						angle -= 2 * MathHelper.Pi;
					}
					if (angle > 0) {
						degreesLeft += 45;
					}
					if (angle < 0) {
						degreesLeft -= 45;
					}
				}

				lp = p;
			}
			wireHits.Clear();

		}

		public Item GetResult() 
		{
			foreach (AssemblyRecipe recipe in AssemblyRecipe.recipes) {
				if (!recipe.seed.AcceptsItem(seed)) {
					Main.NewText($"Seed Invalid for {recipe.result.Name}");
					continue;
				}

				for (int step = 0; step < 8; step++) {
					for (int t = 0; t < 8; t++) {
						int theta = (t + step) % 8;
						List<RecipeIngredient> items = recipe.segments[theta];
						for (int r = 0; r < items.Count; r++) {
							RecipeIngredient item = items[r];
							if (this.items[t] == null || this.items[t].Count <= r || !item.AcceptsItem(this.items[t][r])) {

								Main.NewText($"Attempt {step+1} out of 8: item at ({r}, {theta}) failed check for {item.item.Name}");
								goto RecipeFailed;
							}
						}
					}
					return recipe.result.Clone();

				RecipeFailed:
					continue;
				}
			}
			return null;
		}

		public void Craft() 
		{
			Item result = GetResult();

			if (result == null) {
				foreach (List<Item> segment in items) {
					if (segment != null) {
						foreach (Item item in segment) {
							Item.NewItem(new EntitySource_TileInteraction(Main.LocalPlayer, Position.X, Position.Y), new Rectangle(Position.X * 16, Position.Y * 16, 48, 48), item);
						}
						segment.Clear();
					}
				}
				Item.NewItem(new EntitySource_TileInteraction(Main.LocalPlayer, Position.X, Position.Y), new Rectangle(Position.X * 16, Position.Y * 16, 48, 48), seed);
				seed.TurnToAir();
				return;
			}

			Vector2 center = new Vector2(Position.X * 16 + 24, Position.Y * 16 + 24);

			int theta = 0;
			foreach (List<Item> segment in items) {
				if (segment != null) {
					int r = 1;
					foreach (Item item in segment) {
						Vector2 offset = new Vector2(8, 0) * r;
						offset = offset.RotatedBy((theta + degrees) / 180f * MathF.PI);
						Dust dust = Dust.NewDustDirect(center + offset - new Vector2(4), 0, 0, ModContent.DustType<TransferDust>());
						dust.velocity = -offset / 16;
						r++;
					}
					segment.Clear();
				}
				theta += 45;
			}
			seed.TurnToAir();
		}

        public override bool ExtractItem(Item item)
        {
            Craft();
            return true;
        }

        public override void SaveData(TagCompound tag) {
			//tag.Add("items", items);
			//tag.Add("seed", seed);
			base.SaveData(tag);
		}

		public override void LoadData(TagCompound tag) {
			items = tag.Get<List<Item>[]>("items");
			seed = tag.Get<Item>("seed");
			base.LoadData(tag);
		}
	}

	public class RotaryAssembler : EntityTile<RotaryAssemblerTE>
	{
		public override void PreStaticDefaults() {
			Main.tileNoAttach[Type] = true;
			Main.tileFrameImportant[Type] = true;
			Main.tileLavaDeath[Type] = false;
			TileID.Sets.DisableSmartCursor[Type] = true;
			TileID.Sets.IgnoredByNpcStepUp[Type] = true; // This line makes NPCs not try to step up this tile during their movement. Only use this for furniture with solid tops.

			DustType = ModContent.DustType<Spikesteel>();
			AdjTiles = new int[] { TileID.Tables };

			width = 3;
			height = 3;

			// Etc
			LocalizedText name = CreateMapEntryName();
			// name.SetDefault("Rotarty Assembler");
			AddMapEntry(new Color(200, 200, 200), name);
		}

		public override void KillMultiTile(int i, int j, int frameX, int frameY) {

			RotaryAssemblerTE tileEntity = GetTileEntity(i, j);
			foreach (List<Item> segment in tileEntity.items)
			{
				if (segment == null) continue;
				foreach (Item item in segment)
				{
					Item.NewItem(new EntitySource_TileBreak(i, j), new Rectangle(i * 16, j * 16, 48, 64), item);
				}
				Item.NewItem(new EntitySource_TileBreak(i, j), new Rectangle(i * 16, j * 16, 48, 64), tileEntity.seed);

				ModContent.GetInstance<RotaryAssemblerTE>().Kill(i, j);
			}
		}

		public override bool RightClick(int i, int j) {
			RotaryAssemblerTE tileEntity = GetTileEntity(i, j);
			Item result = tileEntity.GetResult();
			tileEntity.Craft();
			if (result != null) {
				Item.NewItem(new EntitySource_TileInteraction(Main.LocalPlayer, tileEntity.Position.X, tileEntity.Position.Y), new Rectangle(tileEntity.Position.X * 16, tileEntity.Position.Y * 16, 48, 48), result);
			}

			return true;

		}
		public override void MouseOver(int i, int j) {
			RotaryAssemblerTE tileEntity = GetTileEntity(i, j);
			Point16 subTile = new Point16(i, j) - tileEntity.Position;
			Player player = Main.LocalPlayer;
		}

		public override void HitWire(int i, int j) {

			RotaryAssemblerTE tileEntity = GetTileEntity(i, j);
			tileEntity.wireHits.Add(new Point(i, j));
		}
	}
}