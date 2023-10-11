using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Techarria.Content.Dusts;
using Techarria.Content.Tiles;
using Techarria.Content.Tiles.Machines.Logic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;

namespace Techarria.Content.Tiles.Machines
{
	public class StructuralDecomposerResult {

		public bool isMultiple = false;
		public bool isFluid = false;
		public int type = 0;
		public List<int> types = new();
		public byte amount = 0; // only used for fluids

		public StructuralDecomposerResult(int type, bool isFluid, byte amount = 0) {
			this.type = type;
			this.isFluid = isFluid;
			this.amount = amount;
		}
		public StructuralDecomposerResult(List<int> types, bool isFluid, byte amount = 0) {
			this.types = types;
			this.isFluid = isFluid;
			this.amount = amount;
			this.isMultiple = true;
		}

		public int getResult(int index) {
			return isMultiple ? types[index % types.Count] : type;
		}

		public bool canCreateResult(Point p, int index) {
			int x = p.X;
			int y = p.Y;
			Tile tile = Main.tile[x, y];
			if (isFluid) {
				if (tile.LiquidAmount == 0 || tile.LiquidType == getResult(index) && tile.LiquidAmount + amount <= 255) {
					return true;
				}
				return false;
			}
			return !tile.HasTile;
			
		}

		public void createResult(Point p, int index) {
			int x = p.X;
			int y = p.Y;
			if (isFluid) {
				WorldGen.PlaceLiquid(x, y, (byte)getResult(index), amount);
            } else {
				WorldGen.PlaceTile(x, y, getResult(index), forced:true);
			}
        }

	}

	public class StructuralDecomposerRecipe {
		public StructuralDecomposerResult majorResult;
		public StructuralDecomposerResult minorResult;

		public StructuralDecomposerRecipe(StructuralDecomposerResult majorResult, StructuralDecomposerResult minorResult) {
			this.majorResult = majorResult;
			this.minorResult = minorResult;
		}

		public static Dictionary<int, StructuralDecomposerRecipe> recipes = new Dictionary<int, StructuralDecomposerRecipe>() {
			{ TileID.Obsidian, new StructuralDecomposerRecipe(new(LiquidID.Lava, true, 16), new(LiquidID.Water, true, 16)) },
			{ TileID.HoneyBlock, new StructuralDecomposerRecipe(new(LiquidID.Honey, true, 16), new(LiquidID.Water, true, 16)) },
			{ TileID.CrispyHoneyBlock, new StructuralDecomposerRecipe(new(LiquidID.Honey, true, 16), new(LiquidID.Lava, true, 16)) },
			{ TileID.ShimmerBlock, new StructuralDecomposerRecipe(new(LiquidID.Shimmer, true, 16), new(new List<int>() { LiquidID.Honey, LiquidID.Lava, LiquidID.Water }, true, 16)) },
			{ TileID.Stone, new StructuralDecomposerRecipe(new(TileID.Silt, false), new(TileID.Silt, false)) },
			{ TileID.RainCloud, new StructuralDecomposerRecipe(new(TileID.Cloud, false), new(LiquidID.Water, true, 255)) }
		};
	};

	public class StructuralDecomposerTE : ModTileEntity
	{
		public static int REQUIRED_CHARGE = 180;
		public int progress = 0;
		public bool flipped = false;
		public int tile = -1;
		public int status = 0; // 0 = empty, 1 = processing, 2 = output blocked
		public int index;

		public override bool IsTileValidForEntity(int x, int y) {
			return Main.tile[x, y].TileType == ModContent.TileType<StructuralDecomposer>();
		}

		public void InsertPower(int amount) {
			Point16 pos = Position;

			switch (status) {
				case 0: // empty
					if (Main.tile[pos.X + 1, pos.Y - 1].HasTile) {
						tile = Main.tile[pos.X + 1, pos.Y - 1].TileType;
						WorldGen.KillTile(pos.X + 1, pos.Y - 1, noItem: true);
						status = 1;
					}
					break;
				case 1:
					int squish = (int)(progress / (float)REQUIRED_CHARGE * 8) * 2;
					int oldSquish = (int)((progress - amount) / (float)REQUIRED_CHARGE * 8) * 2;
					progress += amount;
					if (squish > 6 && oldSquish <= 6) {
						Tile tile = new Tile();
						tile.TileType = (ushort)this.tile;
						for (int i = 0; i < 10; i++)
						WorldGen.KillTile_MakeTileDust(pos.X + 1, pos.Y + 1, tile);
					}
					if (progress >= REQUIRED_CHARGE) {
						status = 2;
						progress = 0;
					}
					break;
				case 2:
					Point majorOut = flipped ? new(pos.X - 1, pos.Y + 1) : new(pos.X + 3, pos.Y + 1);
					Point minorOut = !flipped ? new(pos.X - 1, pos.Y + 1) : new(pos.X + 3, pos.Y + 1);
					Piston.StaticPush(pos.X, pos.Y + 1, 2);
					Piston.StaticPush(pos.X+2, pos.Y + 1, 0);
					if (StructuralDecomposerRecipe.recipes.ContainsKey(tile)) {
						StructuralDecomposerRecipe recipe = StructuralDecomposerRecipe.recipes[tile];
						if (recipe.majorResult.canCreateResult(majorOut, index) && recipe.minorResult.canCreateResult(minorOut, index)) {
							recipe.majorResult.createResult(majorOut, index);
							recipe.minorResult.createResult(minorOut, index);
							tile = -1;
							status = 0;
							index += 1;
						}
					} else {
						status = 0;
					}
					break;
			}
		}

		public void flip() {
			flipped = !flipped;
			for (int x = 0; x < 3; x++) {
				for (int y = 0; y < 2; y++) {
					int tx = Position.X + x;
					int ty = Position.Y + y;
					Main.tile[tx, ty].TileFrameX = (short)((flipped ? 54 : 0) + x * 18);
				}
			}
		}

		public override void SaveData(TagCompound tag) {
			tag.Add("progress", progress);
			tag.Add("tile", tile);
			tag.Add("flipped", flipped);
			base.SaveData(tag);
		}

		public override void LoadData(TagCompound tag) {
			progress = tag.Get<int>("progress");
			tile = tag.Get<int>("tile");
			flipped = tag.Get<bool>("flipped");
			base.LoadData(tag);
		}
	}

	// Where the TE ends and the Tile starts
	public class StructuralDecomposer : EntityTile<StructuralDecomposerTE>, PowerConsumer
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

		public StructuralDecomposerTE GetTileEntity(int i, int j) 
		{
			Tile tile = Framing.GetTileSafely(i, j);
			i -= tile.TileFrameX / 18 % 3;
			j -= tile.TileFrameY / 18 % 2;
			return TileEntity.ByPosition[new Point16(i, j)] as StructuralDecomposerTE;
		}

		public override bool Slope(int i, int j) {
			GetTileEntity(i, j).flip();
			return false;
		}

		public override void PlaceInWorld(int i, int j, Item item) {
			Tile tile = Framing.GetTileSafely(i, j);
			i -= tile.TileFrameX / 18 % 3;
			j -= tile.TileFrameY / 18 % 2;
			ModContent.GetInstance<StructuralDecomposerTE>().Place(i, j);
		}

		public override void KillMultiTile(int i, int j, int frameX, int frameY) {
			StructuralDecomposerTE tileEntity = GetTileEntity(i, j);
			if (tileEntity.tile >= 0) {
				WorldGen.PlaceTile(i+1, j+1, tileEntity.tile);
			}
			ModContent.GetInstance<StructuralDecomposerTE>().Kill(i, j);
		}
		public override void MouseOver(int i, int j) {
			StructuralDecomposerTE tileEntity = GetTileEntity(i, j);
		}

		public override void InsertPower(int i, int j, int amount) {
			GetTileEntity(i, j).InsertPower(amount);
		}

		public override void PostDraw(int i, int j, SpriteBatch spriteBatch) {
			StructuralDecomposerTE tileEntity = GetTileEntity(i, j);
			Point16 subTile = new Point16(i, j) - tileEntity.Position;

			Vector2 TileOffset = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
			Vector2 pos = new Vector2(i, j) * 16 - Main.screenPosition + TileOffset;

			int squish = tileEntity.status == 2 ? 16 : (int)(tileEntity.progress / (float)StructuralDecomposerTE.REQUIRED_CHARGE * 8) * 2;
			if (tileEntity.tile >= 0 && subTile == new Point16(1, 1) && tileEntity.status == 1 && squish <= 6) {
				Texture2D texture = TextureAssets.Tile[tileEntity.tile].Value;
				spriteBatch.Draw(texture, pos + new Vector2(0, squish - 2), new Rectangle(162, 54 + squish, 16, 16 - squish), Lighting.GetColor(i, j));
			}
			if (StructuralDecomposerRecipe.recipes.ContainsKey(tileEntity.tile)) {
				StructuralDecomposerRecipe recipe = StructuralDecomposerRecipe.recipes[tileEntity.tile];
				StructuralDecomposerResult leftResult = !tileEntity.flipped ? recipe.minorResult : recipe.majorResult;
				if (subTile == new Point16(0, 1)) {
					if (!leftResult.isFluid) {
						Texture2D texture = TextureAssets.Tile[leftResult.getResult(tileEntity.index)].Value;
						Rectangle sourceRect = Main.tileFrameImportant[leftResult.getResult(tileEntity.index)] ? new Rectangle(0, 0, 16, 16) : new Rectangle(162, 54, 16, 16);
						spriteBatch.Draw(texture, pos + new Vector2(-squish, 0), sourceRect, Lighting.GetColor(i, j));
					} else {
						Color color = leftResult.getResult(tileEntity.index) switch {
							0 => new Color(0.1f, 0.2f, 1, 0.75f),
							1 => new Color(1, 0.25f, 0, 1),
							2 => new Color(1, 0.75f, 0, 0.9f),
							3 => new Color(0.9f, 0.7f, 1, 0.8f),
							_ => Color.Black
						};
						Rectangle sourceRect = new Rectangle(0, 16 - squish, 16, 16);
						spriteBatch.Draw(ModContent.Request<Texture2D>("Techarria/Content/Tiles/Machines/StructuralDecomposer_Liquid").Value, pos + new Vector2(0, 16-squish), sourceRect, Lighting.GetColor(i, j).MultiplyRGBA(color));
					}
				}

				StructuralDecomposerResult rightResult = tileEntity.flipped ? recipe.minorResult : recipe.majorResult;
				if (subTile == new Point16(2, 1)) {
					if (!rightResult.isFluid) {
						Texture2D texture = TextureAssets.Tile[rightResult.getResult(tileEntity.index)].Value;
						Rectangle sourceRect = Main.tileFrameImportant[rightResult.getResult(tileEntity.index)] ? new Rectangle(0, 0, 16, 16) : new Rectangle(162, 54, 16, 16);
						spriteBatch.Draw(texture, pos + new Vector2(squish, 0), sourceRect, Lighting.GetColor(i, j));
					} else {
						Color color = rightResult.getResult(tileEntity.index) switch {
							0 => new Color(0.1f, 0.2f, 1, 0.75f),
							1 => new Color(1, 0.25f, 0, 1),
							2 => new Color(1, 0.75f, 0, 0.9f),
							3 => new Color(0.9f, 0.7f, 1, 0.8f),
							_ => Color.Black
						};
						Rectangle sourceRect = new Rectangle(0, 16-squish, 16, 16);
						spriteBatch.Draw(ModContent.Request<Texture2D>("Techarria/Content/Tiles/Machines/StructuralDecomposer_Liquid").Value, pos + new Vector2(0, 16-squish), sourceRect, Lighting.GetColor(i, j).MultiplyRGBA(color));
					}
				}
			}
			spriteBatch.Draw(ModContent.Request<Texture2D>("Techarria/Content/Tiles/Machines/StructuralDecomposer_Overlay").Value, pos, new Rectangle(subTile.X*18, subTile.Y*18 + 18 * squish, 16, 16), Lighting.GetColor(i, j));
		}
	}
}
