﻿using Microsoft.Xna.Framework;
using System;
using Techarria.Content.Dusts;
using Techarria.Structures;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Techarria.Content.Tiles.Machines
{
	public class GreenhouseControllerTE : ModTileEntity {

		public static int SCAN_TILE_COST = 12;
		public static int HARVEST_PLANT_COST = 30;

		int updateTimer = 0;
		int scanIndex = 0;
		int scanProgress = 0;
		int harvestProgress = 0;
		public Greenhouse greenhouse;

		public static int[] seeds = new int[7] { 
			ItemID.DaybloomSeeds, 
			ItemID.MoonglowSeeds, 
			ItemID.BlinkrootSeeds, 
			ItemID.DeathweedSeeds,
			ItemID.WaterleafSeeds,
			ItemID.FireblossomSeeds,
			ItemID.ShiverthornSeeds
		};

		public override bool IsTileValidForEntity(int x, int y) {
			return Main.tile[x, y].TileType == ModContent.TileType<GreenhouseController>();
		}

		public override void Update() {
			updateTimer--;
			if (updateTimer <= 0) {
				if (greenhouse != null) {
					if (!greenhouse.CheckStructure()) {
						greenhouse = null;
					}
				}
				if (greenhouse == null) {
					greenhouse = Greenhouse.CreateGreenhouse(Position.X, Position.Y);
				}
				updateTimer = 60;
			}
			if (greenhouse != null) {
				foreach (Point point in greenhouse.Interior) {
					Tile tile = Main.tile[point];
					if (greenhouse.validRoof) {
						if (tile.TileType == TileID.MatureHerbs) {
							tile.TileType = TileID.BloomingHerbs;
						}
					} else {
						if (tile.TileType == TileID.BloomingHerbs) {
							tile.TileType = TileID.MatureHerbs;
						}
					}
				}
			}
		}

		public void InsertCharge(int amount) {
			if (greenhouse == null || !greenhouse.validRoof) return;
			if (scanIndex >= greenhouse.Interior.Length)
				scanIndex = 0;
			Point p = greenhouse.Interior[scanIndex];
			Tile tile = Main.tile[p];
			if (tile.TileType == TileID.BloomingHerbs) {
				harvestProgress += amount;
				Dust.NewDustDirect(new Vector2(p.X * 16 + 4, p.Y * 16 + 12), 0, 0, DustID.Dirt).velocity = new(MathF.Sin(harvestProgress / (float)HARVEST_PLANT_COST * MathHelper.TwoPi * 2) / 2, -harvestProgress / (float)HARVEST_PLANT_COST * 3);
				if (harvestProgress >= HARVEST_PLANT_COST) {
					harvestProgress = 0;
					int type = tile.TileFrameX / 18;
					WorldGen.KillTile(p.X, p.Y);
					Point scanPoint = new Point(p.X * 16 + 8, p.Y * 16 + 8);
					foreach (Item item in Main.item) {
						if (item.getRect().Intersects(new(scanPoint.X, scanPoint.Y, 0, 0)) && item.type == seeds[type]) {
							item.stack--;
							if (item.stack == 0) {
								item.TurnToAir();
							}
							WorldGen.PlaceTile(p.X, p.Y, TileID.ImmatureHerbs, style: type);
							break;
						}
					}
				}
			} else {
				scanProgress += amount;
				Dust.NewDustDirect(new Vector2(p.X * 16 + 4, p.Y * 16 + 4), 0, 0, ModContent.DustType<Indicator>()).scale = scanProgress / (float)SCAN_TILE_COST;
				if (scanProgress >= SCAN_TILE_COST) {
					scanProgress = 0;
					scanIndex++;
				}
			}
		}
	}

	public class GreenhouseController : PowerConsumer {
		public override void SetStaticDefaults() {

			// Spelunker
			Main.tileSpelunker[Type] = true;

			// Properties
			Main.tileLavaDeath[Type] = false;
			Main.tileFrameImportant[Type] = true;
			TileID.Sets.DisableSmartCursor[Type] = true;
			TileID.Sets.IgnoredByNpcStepUp[Type] = true;
			TileID.Sets.PreventsTileRemovalIfOnTopOfIt[Type] = true;

			// placement
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.LavaDeath = false;
			TileObjectData.addTile(Type);
		}

		public virtual GreenhouseControllerTE GetTileEntity(int i, int j) {
			Tile tile = Framing.GetTileSafely(i, j);
			i -= tile.TileFrameX / 18 % 2;
			j -= tile.TileFrameY / 18 % 2;
			TileEntity.ByPosition.TryGetValue(new Point16(i, j), out TileEntity te);
			return te as GreenhouseControllerTE;
		}
		public override void PlaceInWorld(int i, int j, Item item) {
			Tile tile = Framing.GetTileSafely(i, j);
			i -= tile.TileFrameX / 18 % 2;
			j -= tile.TileFrameY / 18 % 2;
			ModContent.GetInstance<GreenhouseControllerTE>().Place(i, j);
		}

		public override void KillMultiTile(int i, int j, int frameX, int frameY) {
			GetTileEntity(i, j).greenhouse.Remove();
			ModContent.GetInstance<GreenhouseControllerTE>().Kill(i, j);
		}

		public override void InsertPower(int i, int j, int amount) {
			GreenhouseControllerTE te = GetTileEntity(i, j);
			te.InsertCharge(amount);

		}
	}
}