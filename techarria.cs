using Terraria.ModLoader;

namespace Techarria
{
	public class Techarria : Mod
	{
		public static bool[] tileIsTransferDuct = new bool[TileLoader.TileCount];
		public static bool[] tileConnectToPipe = new bool[TileLoader.TileCount];
	}
}