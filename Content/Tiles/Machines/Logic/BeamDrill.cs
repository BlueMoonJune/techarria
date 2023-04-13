using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Techarria.Content.Dusts;
using Techarria.Transfer;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Techarria.Content.Tiles.Machines.Logic
{
	public class BeamDrillTE : ModTileEntity {
		public override bool IsTileValidForEntity(int x, int y) {
			return ModContent.GetModTile(Main.tile[x, y].TileType) is BeamDrill;
		}

		public float targetRange = 0;
		public float range = 0;
		public BeamDrill modtile = null;
		public int timeSinceLastUpdate = 0;
		public Direction dir = new(0);
		public float angle = 0;

		public override void Update() {
			Tile tile = Main.tile[Position.X, Position.Y];
			modtile = ModContent.GetModTile(tile.TileType) as BeamDrill;
			if (modtile == null) {
				Kill(Position.X, Position.Y);
			}
			dir = new(tile.TileFrameX / 16);
			if (timeSinceLastUpdate > 1) {
				targetRange = 0;
			}
			if (range + 0.05f < targetRange) {
				range += 0.05f;
			} else if (range - 0.1f > targetRange) {
				range -= 0.1f;
			} else {
				range = targetRange;
			}
			for (int x = 1; x <= range; x++) {
				Point t = new Point(Position.X + dir.point.X * x, Position.Y + dir.point.Y * x);
				if (Main.tile[t].HasTile) {
					int tx = t.X;
					int ty = t.Y;
					int power = modtile.power;
					Point pos = ChestInterface.FindTopLeft(tx, ty);
					if (pos != Point.Zero) {
						Chest chest = Main.chest[Chest.FindChest(pos.X, pos.Y)];

						foreach (Item item in chest.item) {
							Item.NewItem(new EntitySource_TileBreak(chest.x, chest.y), new Rectangle(chest.x * 16, chest.y * 16, 32, 32), item);
							item.TurnToAir();
						}
						Main.LocalPlayer.PickTile(tx, ty, power);
						return;
					}
					pos = ChestInterface.FindTopLeft(tx, ty - 1);
					if (pos != Point.Zero) {
						Chest chest = Main.chest[Chest.FindChest(pos.X, pos.Y)];

						foreach (Item item in chest.item) {
							Item.NewItem(new EntitySource_TileBreak(chest.x, chest.y), new Rectangle(chest.x * 16, chest.y * 16, 32, 32), item);
							item.TurnToAir();
						}
						Main.LocalPlayer.PickTile(tx, ty - 1, power);
						return;
					}
					Main.LocalPlayer.PickTile(tx, ty, power);
					break;
				}
			}
			timeSinceLastUpdate++;
		}

		public void SetRange(float range) {
			targetRange = range;
			timeSinceLastUpdate = 0;
		}
	}

	public abstract class BeamDrill : PowerConsumer
	{
		public int power = 0; //pickaxe power
		public int range = 0; //range in tiles
		public int cost = 1; //cost for full range

		public static double velCoef = Math.Log(0.94);

		public Effect effect;
		public static Dictionary<ModTile, Effect> effects = new();

		public override void SetStaticDefaults() {
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
			Main.tileFrameImportant[Type] = true;
			if (effect != null && !effects.ContainsKey(this)) {
				effects.Add(this, effect);
			}
		}
		public override bool Slope(int i, int j) {
			Tile tile = Framing.GetTileSafely(i, j);
			tile.TileFrameX = (short)((tile.TileFrameX + 16) % 64);
			GetTileEntity(i, j).range = 0;
			return false;
		}

		public virtual void GenParticles(int x, int y, float distance) {
			Tile tile = Main.tile[x, y];
			Direction dir = Direction.directions()[tile.TileFrameX / 16];
			Dust dust = Dust.NewDustDirect(new Vector2(x * 16 + 4, y * 16 + 4), 0, 0, ModContent.DustType<TransferDust>());
			dust.velocity = new Vector2(dir.point.X, dir.point.Y) * -(float)(distance * velCoef);
		}

		public static BeamDrillTE GetTileEntity(int i, int j) {
			return TileEntity.ByPosition[new Point16(i, j)] as BeamDrillTE;
		}

		public override void PlaceInWorld(int i, int j, Item item) {
			ModContent.GetInstance<BeamDrillTE>().Place(i, j);
		}

		public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem) {
			if (effectOnly || noItem || fail) { return; }
			ModContent.GetInstance<ItemPlacerTE>().Kill(i, j);
		}

		public override void InsertPower(int i, int j, int amount) {
			float distance = Math.Min(range * amount / (float)cost, range);
			BeamDrillTE tileEntity = GetTileEntity(i, j);
			if (distance >= tileEntity.range) {
				tileEntity.SetRange(distance);
			}

		}
	}
}
