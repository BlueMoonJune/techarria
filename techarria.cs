using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Techarria.Content.Items;

namespace Techarria
{

	public class Techarria : Mod
    {
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