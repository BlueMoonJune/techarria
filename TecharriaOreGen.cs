using System.Collections.Generic;
using Techarria.Content.Tiles.Natural;
using Terraria;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
// using ctrl c + ctrl v;
namespace Techarria
{
	public class OreSystem : ModSystem
	{
		public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
		{
			// Because world generation is like layering several images ontop of each other, we need to do some steps between the original world generation steps.

			// The first step is an Ore. Most vanilla ores are generated in a step called "Shinies", so for maximum compatibility, we will also do this.
			// First, we find out which step "Shinies" is.
			int ShiniesIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Shinies"));

			if (ShiniesIndex != -1)
			{
				// Next, we insert our pass directly after the original "Shinies" pass.
				// TecharriaOrePass is a class seen below
				tasks.Insert(ShiniesIndex + 1, new TecharriaOrePass("Techarria Ores", 237.4298f));
			}
		}
	}
	public class TecharriaOrePass : GenPass
	{
		public TecharriaOrePass(string name, float loadWeight) : base(name, loadWeight)
		{
		}

		protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
		{
			// progress.Message is the message shown to the user while the following code is running.
			// Try to make your message clear. You can be a little bit clever, but make sure it is descriptive enough for troubleshooting purposes.
			progress.Message = "Generating Techarria Ores";

			// Ores are quite simple, we simply use a for loop and the WorldGen.TileRunner to place splotches of the specified Tile in the world.
			// that big number is commonality of veins, lower = less, bigger = more. 0.00006 is basically copper spawn rate

			// coal ore gen
			for (int k = 0; k < (int)(Main.maxTilesX * Main.maxTilesY * 0.0000015); k++)
			{
				// The inside of this for loop corresponds to one single splotch of our Ore.
				// First, we randomly choose any coordinate in the world by choosing a random x and y value.
				int x = WorldGen.genRand.Next(0, Main.maxTilesX);

				// WorldGen.worldSurfaceLow is actually the highest surface tile. In practice you might want to use GenVars.rockLayer or other WorldGen values.
				int y = WorldGen.genRand.Next((int)GenVars.rockLayer, Main.maxTilesY);

				// first Rand is strength (how much per vein) and second is steps (makes it snakey)
				WorldGen.TileRunner(x, y, WorldGen.genRand.Next(26, 32), WorldGen.genRand.Next(215, 285), ModContent.TileType<CoalOre>());

				// Alternately, we could check the tile already present in the coordinate we are interested.
				// Wrapping WorldGen.TileRunner in the following condition would make the ore only generate in Snow.
				// Tile tile = Framing.GetTileSafely(x, y);
				// if (tile.HasTile && tile.TileType == TileID.SnowBlock) {
				// 	WorldGen.TileRunner(.....);
				// }
			}
			// coal ore gen but smaller
			for (int k = 0; k < (int)(Main.maxTilesX * Main.maxTilesY * 0.000007); k++)
			{
				// The inside of this for loop corresponds to one single splotch of our Ore.
				// First, we randomly choose any coordinate in the world by choosing a random x and y value.
				int x = WorldGen.genRand.Next(0, Main.maxTilesX);

				// WorldGen.worldSurfaceLow is actually the highest surface tile. In practice you might want to use GenVars.rockLayer or other WorldGen values.
				int y = WorldGen.genRand.Next((int)GenVars.rockLayer, Main.maxTilesY);

				// first Rand is strength (how much per vein) and second is steps (makes it snakey)
				WorldGen.TileRunner(x, y, WorldGen.genRand.Next(3, 6), WorldGen.genRand.Next(4, 8), ModContent.TileType<CoalOre>());
			}

			// limestone spawn
			for (int k = 0; k < (int)(Main.maxTilesX * Main.maxTilesY * 0.00004); k++)
			{
				// The inside of this for loop corresponds to one single splotch of our Ore.
				// First, we randomly choose any coordinate in the world by choosing a random x and y value.
				int x = WorldGen.genRand.Next(0, Main.maxTilesX);

				// WorldGen.worldSurfaceLow is actually the highest surface tile. In practice you might want to use GenVars.rockLayer or other WorldGen values.
				int y = WorldGen.genRand.Next((int)GenVars.rockLayer, Main.maxTilesY);

				// first Rand is strength (how much per vein) and second is steps (makes it snakey)
				WorldGen.TileRunner(x, y, WorldGen.genRand.Next(4, 6), WorldGen.genRand.Next(78, 112), ModContent.TileType<Limestone>());
			}
		}
	}
}
