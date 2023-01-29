using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Techarria.Transfer;

namespace Techarria.Content.Tiles.Machines.Logic
{
	/// <summary>
	/// A tile that damages blocks when powered. Default pickaxe power is 40
	/// </summary>
	public class BlockBreaker : ModTile
	{
		public static int power = 100;
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

			int tx = i + xOff;
			int ty = j + yOff;

			Point pos = ChestInterface.FindTopLeft(tx, ty);
			if (pos != Point.Zero) {
				Chest chest = Main.chest[Chest.FindChest(pos.X, pos.Y)];

				for (int x = 0; x < chest.item.Length; x++) {
					Item item = chest.item[x];
					Item.NewItem(new EntitySource_TileBreak(chest.x, chest.y), new Rectangle(chest.x * 16, chest.y * 16, 32, 32), item);
					chest.item[x].TurnToAir();
				}
				Main.LocalPlayer.PickTile(tx, ty, power);
				return;
			}
			pos = ChestInterface.FindTopLeft(tx, ty - 1);
			if (pos != Point.Zero) {
				Chest chest = Main.chest[Chest.FindChest(pos.X, pos.Y)];

				for (int x = 0; x < chest.item.Length; x++) {
					Item item = chest.item[x];
					Item.NewItem(new EntitySource_TileBreak(chest.x, chest.y), new Rectangle(chest.x * 16, chest.y * 16, 32, 32), item);
					chest.item[x].TurnToAir();
				}
				Main.LocalPlayer.PickTile(tx, ty - 1, power);
				return;
			}
			Main.LocalPlayer.PickTile(tx, ty, power);
		}
	}
}
