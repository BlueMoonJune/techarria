using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Techarria.Content.Dusts;
using Terraria.DataStructures;

namespace Techarria.Content.Tiles.Machines.Logic
{
	/// <summary>
	/// A tile that pushes blocks when powered. Has a push limit of 32.
	/// </summary>
	public class Piston : ModTile
	{
		public static List<Point> scanned = new();

		public int myType;
		public static int blockCount = 0;

		public override void SetStaticDefaults() {
			myType = ModContent.TileType<Piston>();

			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
			Main.tileFrameImportant[Type] = true;

			AddMapEntry(Color.DarkSlateGray, CreateMapEntryName());

			DustType = DustID.Stone;
			ItemDrop = ModContent.ItemType<Items.Placeables.Machines.Logic.Piston>();

			HitSound = SoundID.Tink;
		}

		public override bool Slope(int i, int j) {
			Tile tile = Framing.GetTileSafely(i, j);
			tile.TileFrameX = (short)((tile.TileFrameX + 16) % 64);
			return false;
		}

		public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak) {
			return false;
		}

		public static void CloneTile(int i, int j, int x, int y) {
			Tile sourceTile = Main.tile[i, j];
			Tile destTile = Main.tile[x, y];
			var destTileReset = new Tile();
			destTileReset.CopyFrom(destTile);
			destTile.CopyFrom(sourceTile);
			sourceTile.ClearEverything();

			sourceTile.WallColor = destTile.WallColor;
			sourceTile.WallFrameNumber = destTile.WallFrameNumber;
			sourceTile.WallFrameX = destTile.WallFrameX;
			sourceTile.WallFrameY = destTile.WallFrameY;
			sourceTile.WallType = destTile.WallType;

			destTile.WallColor = destTileReset.WallColor;
			destTile.WallFrameNumber = destTileReset.WallFrameNumber;
			destTile.WallFrameX = destTileReset.WallFrameX;
			destTile.WallFrameY = destTileReset.WallFrameY;
			destTile.WallType = destTileReset.WallType;

			if (TileEntity.ByPosition.TryGetValue(new Point16(i, j), out TileEntity te)) {
				te.Position = new Point16(x, y);
				TileEntity.ByPosition.Remove(new Point16(i, j));
				TileEntity.ByPosition.Add(new Point16(x, y), te);
			}

			if (Chest.FindChest(i, j) >= 0) {
				Chest chest = Main.chest[Chest.FindChest(i, j)];
				chest.x = x;
				chest.y = y;
			}

			Techarria.BlockDusts = true;
			sourceTile.ClearTile();
			Techarria.BlockDusts = false;


		}

		public static bool isImmovable(int i, int j) {
			Tile tile = Main.tile[i, j];
			if ((tile.TileType == ModContent.TileType<Piston>() || tile.TileType == ModContent.TileType<Piston>()) && tile.TileFrameY > 0) {
				return true;
			}

			if (tile.WallType == WallID.LihzahrdBrick || tile.WallType == WallID.LihzahrdBrickUnsafe) {
				return true;
			}

			return tile.TileType == TileID.Obsidian || tile.TileType == TileID.LihzahrdBrick;
		}

		public static bool isSticky(int i, int j) {
			int type = Main.tile[i, j].TileType;
			return type == TileID.SlimeBlock || type == TileID.FrozenSlimeBlock || type == TileID.PinkSlimeBlock || type == TileID.HoneyBlock;
		}

		public static List<Point> SortFrontToBack(List<Point> points, Direction dir, out Rectangle boundingBox) {
			var sorted = new List<Point>();

			int maxX = 0;
			int minX = Main.maxTilesX;
			int maxY = 0;
			int minY = Main.maxTilesY;
			foreach (Point point in points) {
				if (point.X > maxX)
					maxX = point.X;
				if (point.X < minX)
					minX = point.X;
				if (point.Y > maxY)
					maxY = point.Y;
				if (point.Y < minY)
					minY = point.Y;
			}

			boundingBox = new Rectangle(minX * 16, minY * 16, (maxX - minX) * 16, (maxY - minY) * 16);

			if (dir <= 1) {
				for (int x = maxX; x >= minX; x--) {
					for (int y = maxY; y >= minY; y--) {
						if (points.Contains(new Point(x, y))) {
							sorted.Add(new Point(x, y));
						}
					}
				}
			}
			else {
				for (int x = minX; x <= maxX; x++) {
					for (int y = minY; y <= maxY; y++) {
						if (points.Contains(new Point(x, y))) {
							sorted.Add(new Point(x, y));
						}
					}
				}
			}

			return sorted;
		}
		
		public static List<Point> ScanAdhesive(int channel, Point p, Direction dir) {
			var result = new List<Point>();
			if (p.X < 0 || p.X > Main.maxTilesX || p.Y < 0 || p.Y > Main.maxTilesY) {
				return result;
			}
			if (scanned.Contains(p))
				return result;
			if (!Main.tile[p].HasTile)
				return result;

			foreach (Direction d in Direction.directions())
            {
				Point t = p + d;
				if (Main.tile[t].Get<Glue>().GetChannel(channel)) {
					Dust.NewDust(new Vector2(t.X * 16 + 4, t.Y * 16 + 4), 0, 0, ModContent.DustType<TransferDust>());
					List<Point> scanResult = Scan(p, dir, true);
					List<Point> adhesiveScanResult = ScanAdhesive(channel, t, dir);
					foreach (var point in adhesiveScanResult) {
						result.Add(point);
					}
					foreach (var point in scanResult) {
						result.Add(point);
					}
				}
            }

			return result;
		}

		public static List<Point> Scan(Point p, Direction dir, bool adhesive = false) {
			var result = new List<Point>();

			if (p.X < 0 || p.X > Main.maxTilesX || p.Y < 0 || p.Y > Main.maxTilesY) {
				return result;
			}

			Glue glue = Main.tile[p].Get<Glue>();
			if (glue.types != 0 && !adhesive) {
				for (int i = 0; i < 4; i++) {
					if (glue.GetChannel(i)) {
						List<Point> adhesiveScanResult = ScanAdhesive(i, p, dir);
						foreach (var point in adhesiveScanResult) {
							result.Add(point);
						}
					}
				}
			}

			if (scanned.Contains(p))
				return result;

			scanned.Add(p);

			if (!Main.tile[p].HasTile || isImmovable(p.X, p.Y)) {
				return result;
			}


			Point TL = HelperMethods.GetTopLeftTileInMultitile(p.X, p.Y, out int width, out int height);

			if (width != 1 || height != 1)
				for (int x = 0; x < width; x++) {
					for (int y = 0; y < height; y++) {
						if (!scanned.Contains(new Point(TL.X + x, TL.Y + y))) {
							List<Point> ScanResult = Scan(new Point(TL.X + x, TL.Y + y), dir);
							foreach (var point in ScanResult) {
								result.Add(point);
							}
						}
					}
				}
			result.Add(p);

			if (isSticky(p.X, p.Y))
				foreach (Direction d in Direction.directions()) {
					Point t = p + d;
					List<Point> stickyScanResult = Scan(t, dir);
					foreach (var point in stickyScanResult) {
						result.Add(point);
					}
				}

			List<Point> pushScanResult = Scan(p + dir, dir);
			foreach (var point in pushScanResult) {
				result.Add(point);
			}

			return result;
		}

		public bool CanPushTiles(List<Point> pairs, Direction dir) {
			if (pairs.Count > 64)
				return false;

			foreach (var point in pairs) {
				Point t = point + dir;
				if (t.X < 0 || t.X > Main.maxTilesX || t.Y < 0 || t.Y > Main.maxTilesY)
					return false;
				if (Main.tile[t].HasTile) {
					bool inList = false;
					foreach (Point p in pairs) {
						if (p.X == t.X && p.Y == t.Y) {
							inList = true;
						}
					}
					if (!inList) {
						Dust.NewDust(new Vector2(t.X * 16, t.Y * 16), 0, 0, ModContent.DustType<Indicator>());
						return false;
					}
				}
			}
			return true;
		}

		public bool PushTiles(List<Point> pairs, Direction dir) {

			if (!CanPushTiles(pairs, dir)) {
				return false;
			}

			List<Point> sorted = SortFrontToBack(pairs, dir, out Rectangle boundingBox);
			foreach (Point point in sorted) {
				Point t = point + dir;
				CloneTile(point.X, point.Y, t.X, t.Y);
			}

			foreach (Point point in sorted) {
				WorldGen.TileFrame(point.X, point.Y);
			}

			boundingBox.Inflate(1, 1);
			boundingBox.Width += 16;
			boundingBox.Height += 16;
			foreach (Player player in Main.player) {

				if (!player.getRect().Intersects(boundingBox)) {
					continue;
				}
				foreach (Point point in sorted) {
					Point p = point + dir;
					if (new Rectangle(p.X * 16 - 1, p.Y * 16 - 1, 18, 17).Intersects(player.getRect())) {
						player.position += (Vector2)dir * 16;
						break;
					}
				}
			}

			foreach (Item item in Main.item) {

				if (!item.getRect().Intersects(boundingBox)) {
					continue;
				}
				foreach (Point point in sorted) {
					Point p = point + dir;
					if (new Rectangle(point.X * 16 - 1, point.Y * 16 - 1, 18, 17).Intersects(item.getRect())) {
						item.position += (Vector2)dir * 16;
						break;
					}
				}
			}

			return true;
		}

		public virtual void Extend(Point p, Direction dir) {

			int x = p.X + dir.point.X;
			int y = p.Y + dir.point.Y;

			List<Point> scanResult = Scan(new Point(x, y), dir);
			if (CanPushTiles(scanResult, dir))
				foreach (var point in scanResult) {
					Dust.NewDust(new Vector2(point.X, point.Y) * 16, 0, 0, ModContent.DustType<TransferDust>());
				}
			PushTiles(scanResult, dir);
		}

		public static void StaticPush(int i, int j, Direction dir) {
			ModContent.GetInstance<Piston>().Push(i, j, dir);
		}

		public void Push(int i, int j, Direction dir) {
			scanned.Clear();
			scanned.Add(new Point(i, j));
			Extend(new Point(i, j), dir);
		}

		public override void HitWire(int i, int j) {
			Tile tile = Framing.GetTileSafely(i, j);

			Direction dir = tile.TileFrameX / 16;
			if (tile.TileFrameY != 0) {
				return;
			}

			Push(i, j, dir);
		}
	}
}
