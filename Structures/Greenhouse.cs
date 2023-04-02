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

	internal class Greenhouse
	{
		public bool isValid = false;
		public string invalidation = "";
		public List<Point> Interior = new();
		public StructureWalls walls;

		public static List<Greenhouse> greenhouses = new();

		public static List<Point> ScannedPoints = new();

		public static List<Point> GreenhousePoints = new();

		public static void CreateGreenhouse(int x, int y) {
			if (GreenhousePoints.Contains(new(x, y))) {
				Main.NewText("A Greenhouse exists here!");
				ScannedPoints.Clear();
				return;
			}
			Greenhouse greenhouse = new();
			if (!ScanTile(new Point(x, y))) {
				Main.NewText("Greenhouse is too big!");
				ScannedPoints.Clear();
				return;
			}
			greenhouse.walls = StructureWalls.GenerateWallsFromInterior(ScannedPoints);
			foreach (List<Point> wall in greenhouse.walls.AllWalls) {
				foreach (Point point in wall) {
					Dust.NewDust(new Vector2(point.X * 16 + 4, point.Y * 16 + 4), 0, 0, ModContent.DustType<Indicator>());
				}
			}
			foreach (Point point in ScannedPoints) {
				GreenhousePoints.Add(point);
				Dust.NewDust(new Vector2(point.X * 16 + 4, point.Y * 16 + 4), 0, 0, ModContent.DustType<DroneNodeDust>());
			}
			ScannedPoints.Clear();
			greenhouses.Add(greenhouse);
		}

		public static bool ScanTile(Point p) {

			Tile tile = Main.tile[p];
			if (!Main.tileSolid[tile.TileType] || !tile.HasTile) {
				ScannedPoints.Add(p);

				if (ScannedPoints.Count() > 750) {
					return false;
				}

				foreach (Direction dir in Direction.directions()) {
					Point t = p + dir.point;
					if (!ScannedPoints.Contains(t)) {
						Dust.NewDust(new Vector2(t.X * 16 + 4, t.Y * 16 + 4), 0, 0, ModContent.DustType<Indicator>());
						Main.NewText(Main.tileSolid[tile.TileType]);
						if (!ScanTile(t)) return false;
						continue;
					}
				}
			}
			return true;
		}

		public static bool CheckGlass(Point p) {
			if (Main.tile[p].TileType != ModContent.TileType<GreenhouseGlass>()) {
				return false;
			}
			for (int i = 1; i < 51; i++) {
				Tile tile = Main.tile[new Point(p.X, p.Y - i)];
				if (Main.tileBlockLight[tile.TileType] && ! tile.IsActuated) {
					return false;
				}
			}
			return true;
		}

		public bool CheckWalls() {
			foreach (List<Point> wall in walls.AllWalls) {
				foreach (Point point in wall) {
					Tile tile = Main.tile[point];
					Dust.NewDust(new Vector2(point.X * 16 + 4, point.Y * 16 + 4), 0, 0, ModContent.DustType<Indicator>());
					if (!Main.tileSolid[tile.TileType] || !tile.HasTile) {
						return false;
					}
				}
			}
			return true;
		}
	}
}
