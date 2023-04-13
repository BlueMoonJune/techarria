using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Techarria.Content.Dusts;
using Techarria.Content.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Techarria.Structures
{
	public class StructureWalls {
		public List<Point> eastWalls = new();
		public List<Point> westWalls = new();
		public List<Point> floors = new();
		public List<Point> ceilings = new();

		public List<List<Point>> AllWalls => new List<List<Point>>() {eastWalls, floors, westWalls, ceilings};

		public static StructureWalls GenerateWallsFromInterior(List<Point> interiorPoints) {
			StructureWalls walls = new();
			foreach (Direction d in Direction.directions()) {
				List<Point> wall = d.num switch {
					0 => walls.eastWalls,
					1 => walls.floors,
					2 => walls.westWalls,
					_ => walls.ceilings
				};
				foreach (Point p in interiorPoints) {
					Point t = p + d.point;
					if (!interiorPoints.Contains(t)) {
						Tile tile = Main.tile[p];
						if (!Main.tileSolid[tile.TileType] || !tile.HasTile) {
							wall.Add(t);
						}
					}
				}
			}
			return walls;
		}
	}

	public class Greenhouse
	{
		public static float GREENHOUSE_ROOF_PERCENTAGE_REQUIREMENT = 0.5f;

		public bool isValid = true;
		public string invalidation = "";
		public Point[] Interior;
		public StructureWalls walls;

		public bool validRoof = false;

		public static List<Greenhouse> greenhouses = new();

		public static List<Point> ScannedPoints = new();

		public static List<Point> GreenhousePoints = new();

		public static bool IsWallTile(Tile tile) {
			return Main.tileSolid[tile.TileType] && tile.HasTile || TileID.Sets.HousingWalls[tile.TileType];
		}

		public static Greenhouse CreateGreenhouse(int x, int y) {
			Greenhouse greenhouse = new();
			if (GreenhousePoints.Contains(new(x, y))) {
				greenhouse.isValid = false;
				greenhouse.invalidation = "A greenhouse exists here!";
				return greenhouse;
			}
			int res = ScanTile(new Point(x, y));
			foreach (Point point in ScannedPoints) {
				GreenhousePoints.Add(point);
			}
			greenhouse.walls = StructureWalls.GenerateWallsFromInterior(ScannedPoints);
			greenhouse.Interior = ScannedPoints.ToArray();
			ScannedPoints.Clear();
			if (res == 1) {
				greenhouse.isValid = false;
				greenhouse.invalidation = "Greenhouse is missing walls!";
				return greenhouse;
			}
			if (res == 2) {
				greenhouse.isValid = false;
				greenhouse.invalidation = "Greenhouse is too big!";
				return greenhouse;
			}
			foreach (List<Point> wall in greenhouse.walls.AllWalls) {
				foreach (Point point in wall) {
					Dust.NewDust(new Vector2(point.X * 16 + 4, point.Y * 16 + 4), 0, 0, ModContent.DustType<Indicator>());
				}
			}
			return greenhouse;
		}

		public static int ScanTile(Point p) {

			Tile tile = Main.tile[p];
			if (!IsWallTile(tile)) {
				ScannedPoints.Add(p);

				if (tile.WallType == 0) {
					return 1;
				}

				if (ScannedPoints.Count() > 750) {
					return 2;
				}

				foreach (Direction dir in Direction.directions()) {
					Point t = p + dir.point;
					if (!ScannedPoints.Contains(t)) {
						Dust.NewDust(new Vector2(t.X * 16 + 4, t.Y * 16 + 4), 0, 0, ModContent.DustType<Indicator>());
						Main.NewText(Main.tileSolid[tile.TileType]);
						int res = ScanTile(t);
						if (res != 0) return res;
					}
				}
			}
			return 0;
		}

		public static bool CheckGlass(Point p) {
			if (Main.tile[p].TileType != ModContent.TileType<GreenhouseGlass>()) {
				return false;
			}
			for (int i = 1; i < 51; i++) {
				Tile tile = Main.tile[new Point(p.X, p.Y - i)];
				if (tile.HasTile && Main.tileBlockLight[tile.TileType] && !tile.IsActuated) {
					return false;
				}
			}
			return true;
		}

		public void Remove() {
			foreach (Point p in Interior) {
				GreenhousePoints.Remove(p);
			}
		}

		public bool CheckStructure() {
			foreach (List<Point> wall in walls.AllWalls) {
				foreach (Point point in wall) {
					Tile tile = Main.tile[point];
					if (!IsWallTile(tile)) {
						Remove();
						return false;
					}
				}
			}
			foreach (Point p in Interior) {
				if (Main.tile[p].WallType == 0) {
					Remove();
					return false;
				}
			}

			int successes = 0;
			int checks = 0;
			foreach (Point p in walls.ceilings) {
				if (!Main.tileSolidTop[Main.tile[p].TileType]) {
					checks++;
					if (CheckGlass(p))
						successes++;
				}
			}
			validRoof = successes / (float)checks > GREENHOUSE_ROOF_PERCENTAGE_REQUIREMENT;
			return true;

		}
	}
}
