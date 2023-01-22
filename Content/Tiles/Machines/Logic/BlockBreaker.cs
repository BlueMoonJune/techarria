using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;

namespace Techarria.Content.Tiles.Machines.Logic
{
	/// <summary>
	/// A tile that damages blocks when powered. Default pickaxe power is 40
	/// </summary>
	public class BlockBreaker : ModTile
	{
		public static int power = 4000;
		public override void SetStaticDefaults() {
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
			Main.tileFrameImportant[Type] = true;

			AddMapEntry(Color.DarkSlateGray, CreateMapEntryName());

			DustType = DustID.Stone;
			ItemDrop = ModContent.ItemType<Items.Placeables.Machines.Logic.BlockBreaker>();

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

		public override void HitWire(int i, int j) {
			Tile tile = Framing.GetTileSafely(i, j);
			var dir = new Direction(tile.TileFrameX / 16);
			int xOff = dir.point.X;
			int yOff = dir.point.Y;

			tile.TileFrameY += 16;
			tile.TileFrameY %= 32;

			Main.LocalPlayer.PickTile(i + xOff, j + yOff, 400);
		}
	}
}
