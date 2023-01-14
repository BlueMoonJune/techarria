using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Techarria.Content.Items;
using Terraria.DataStructures;
using System;

namespace Techarria
{

	public class Techarria : Mod
    {
        public static float GenerationMultiplier = 1;
        public static float CapacityMultiplier = 1;
        public static float UsageMultiplier = 1;

        public static bool BlockDusts = false;

        public static bool Intersects(Rectangle rect, LineSegment line)
        {
            Rectangle aabb = new Rectangle();
            aabb.X = (int)Math.Min(line.Start.X, (int)line.End.X);
            aabb.Y = (int)Math.Min(line.Start.Y, (int)line.End.Y);
            aabb.Width = (int)Math.Max(line.Start.X, (int)line.End.X) - aabb.X;
            aabb.Height = (int)Math.Max(line.Start.Y, (int)line.End.Y) - aabb.Y;
            if (!aabb.Intersects(rect))
            {
                return false;
            }

            float m = (line.Start.Y - line.End.Y) / (line.Start.X - line.End.X);

            Vector2 slideTR = new Vector2(rect.Left, rect.Top - rect.Width * m);
            Vector2 slideBR = new Vector2(rect.Left, slideTR.Y + rect.Width);

            float topBound = Math.Max(rect.Top, rect.Bottom);
            topBound = Math.Max(topBound, slideTR.Y);
            topBound = Math.Max(topBound, slideBR.Y);
            float bottomBound = Math.Min(rect.Top, rect.Bottom);
            bottomBound = Math.Min(bottomBound, slideTR.Y);
            bottomBound = Math.Min(bottomBound, slideBR.Y);

            float b = m * (rect.Left - line.Start.X) + line.Start.Y;

            return b <= topBound && b >= bottomBound;
        }

        public static void print(object obj)
        {
            System.Console.WriteLine(obj);
            Main.NewText(obj);
        }

        /// <summary>contains the filter item types. 'filterItems[filterIDs[x,y]]' gives you the for the filter at those coords</summary>
        public static int[] filterItems = new int[8192];
        /// <summary>contains the positions filters of each ID are located at</summary>
        public static Point[] filterPositions = new Point[8192];
        /// <summary>contains the filter ID for that position</summary>
        public static int[,] filterIDs = new int[Main.maxTilesX, Main.maxTilesY];

        /// <summary>contains the wormhole link item types. 'filterItems[filterIDs[x,y]]' gives you the for the filter at those coords</summary>
        public static int[] wormholeLinkItems = new int[2048];
        /// <summary>contains the positions wormholes of each ID are located at</summary>
        public static Point[] wormholePositions = new Point[2048];
        /// <summary>contains the wormhole ID for that position</summary>
        public static int[,] wormholeIDs = new int[Main.maxTilesX, Main.maxTilesY];

        ///
        public static Item[] itemPlacerItems = new Item[8192];
        ///
        public static Point[] itemPlacerPositions = new Point[8192];
        ///
        public static int[,] itemPlacerIDs = new int[Main.maxTilesX, Main.maxTilesY];

        /// <summary>Whether or not this type is an item transfer tile</summary>
        public static bool[] tileIsTransferDuct = new bool[TileLoader.TileCount];
        /// <summary>Whether or not transfer ducts should connect to this type of tile</summary>
		public static bool[] tileConnectToPipe = new bool[TileLoader.TileCount];

        /// <summary>
        /// Updates the texture of the transfer duct at the specified coords
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void TransferDuctFrame(int x, int y)
        {
            if (tileIsTransferDuct[Main.tile[x, y].TileType])
            {
                ModTile tileInstance = TileLoader.GetTile(Main.tile[x, y].TileType);
                bool idc = false;
                tileInstance.TileFrame(x, y, ref idc, ref idc);
            }
        }


        public override void Load()
        {
            On.Terraria.WorldGen.paintTile += WorldGen_paintTile;
            On.Terraria.Dust.NewDust += DustDetour;
        }

        private int DustDetour(On.Terraria.Dust.orig_NewDust orig, Vector2 Position, int Width, int Height, int Type, float SpeedX, float SpeedY, int Alpha, Color newColor, float Scale)
        {
            if (BlockDusts)
            {
                return Main.maxDust;
            }
            return orig(Position, Width, Height, Type, SpeedX, SpeedY, Alpha, newColor, Scale);
        }

        public override void Unload()
        {
            On.Terraria.WorldGen.paintTile -= WorldGen_paintTile;
        }

        private bool WorldGen_paintTile(On.Terraria.WorldGen.orig_paintTile orig, int x, int y, byte color, bool broadCast)
        {
            bool result = orig.Invoke(x, y, color, broadCast);
            TransferDuctFrame(x, y);
            TransferDuctFrame(x+1, y);
            TransferDuctFrame(x, y+1);
            TransferDuctFrame(x-1, y);
            TransferDuctFrame(x, y-1);

            return result;
        }
    }
}