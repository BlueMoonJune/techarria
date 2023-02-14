using Techarria.Transfer;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Techarria.Content.Tiles.Transfer
{
	/// <summary>
	/// A tile that can only send items in the direction it is facing
	/// </summary>
	public class DirectionalDuct : TransferDuct
	{
		public override void SetStaticDefaults() {
			base.SetStaticDefaults();

			TileID.Sets.CanBeSloped[Type] = true;
			ItemDrop = ModContent.ItemType<Items.Placeables.Transfer.DirectionalDuct>();
		}

		public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak) {
			return true;
		}

		public override bool Slope(int i, int j) {
			Tile tile = Framing.GetTileSafely(i, j);
			tile.TileFrameX = (short)((tile.TileFrameX + 16) % 64);
			return false;
		}


		public override ContainerInterface EvaluatePath(int x, int y, Item item, int origin, int depth) {
			origin = (origin + 2) % 4;
			if (depth >= 256) {
				Main.LocalPlayer.PickTile(x, y, 40000);
			}
			ContainerInterface container = FindAdjacentContainer(x, y);

			int dir = Main.tile[x, y].TileFrameX % 64 / 16;
			if (container != null && container.dir == dir) {
				CreateParticles(x, y, container.dir);
				return container;
			}

			int i = x + dirToX(dir);
			int j = y + dirToY(dir);
			if (Techarria.tileIsTransferDuct[Main.tile[i, j].TileType] && dir != origin) {
				ContainerInterface target = ((TransferDuct)TileLoader.GetTile(Main.tile[i, j].TileType)).EvaluatePath(x + dirToX(dir), y + dirToY(dir), item, dir, depth + 1);
				if (target != null) {
					CreateParticles(x, y, dir);
					return target;
				}
			}


			return null;
		}

		public override void HitWire(int i, int j) {
			Tile tile = Framing.GetTileSafely(i, j);
			tile.TileFrameX += 32;
			tile.TileFrameX %= 64;
		}
	}
}
