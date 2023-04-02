using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using System;
using Terraria.DataStructures;
using Techarria.Content.Tiles;
using Techarria.Content.Tiles.Machines;
using System.Collections.Generic;
using Terraria.ModLoader.IO;
using Techarria.Structures;

namespace Techarria
{
	internal class WorldDataManager : ModSystem
	{
		public override void SaveWorldData(TagCompound tag) {
			List<Vector2> points = new();
			List<byte> glues = new();
			for (int j = 0; j < Main.maxTilesY; j++) {
				for (int i = 0; i < Main.maxTilesX; i++) {
					Point p = new(i, j);
					Glue glue = Main.tile[p].Get<Glue>();
					if (glue.types != 0) {
						points.Add(new(p.X, p.Y));
						glues.Add((byte)glue.types);
					}
				}
			}
			if (glues.Count > 0) {
				tag.Add("gluePoints", points);
				tag.Add("glueValues", glues);
			}
		}

		public override void LoadWorldData(TagCompound tag) {
			Greenhouse.GreenhousePoints.Clear();
			Greenhouse.greenhouses.Clear();
			if (tag.ContainsKey("gluePoints") && tag.ContainsKey("glueValues")) {
				List<Vector2> points = tag.Get<List<Vector2>>("gluePoints");
				List<byte> glues = tag.Get<List<byte>>("glueValues");
				for (int i = 0; i < points.Count; i++) {
					Vector2 p = points[i];
					Main.tile[(int)p.X, (int)p.Y].Get<Glue>().types = (GlueTypes)glues[i];
				}
			}
		}
	}
}
