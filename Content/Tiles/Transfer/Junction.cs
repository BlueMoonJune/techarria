using Techarria.Transfer;
using Terraria;
using Terraria.ModLoader;

namespace Techarria.Content.Tiles.Transfer
{
	/// <summary>
	/// Sends items straight through itself
	/// </summary>
	public class Junction : TransferDuct
	{
		public override void SetStaticDefaults() {
			base.SetStaticDefaults();
			ItemDrop = ModContent.ItemType<Items.Placeables.Transfer.Junction>();
		}

		public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak) {
			return true;
		}

		public override ContainerInterface EvaluatePath(int x, int y, Item item, int origin, int depth) {
			if (depth >= 256) {
				Main.LocalPlayer.PickTile(x, y, 40000);
			}
			ContainerInterface container = FindAdjacentContainer(x, y);
			if (container != null && container.dir == origin) {
				CreateParticles(x, y, container.dir);
				return container;
			}

			int i = x + dirToX(origin);
			int j = y + dirToY(origin);
			if (Techarria.tileIsTransferDuct[Main.tile[i, j].TileType]) {
				ContainerInterface target = ((TransferDuct)TileLoader.GetTile(Main.tile[i, j].TileType)).EvaluatePath(x + dirToX(origin), y + dirToY(origin), item, origin, depth + 1);
				if (target != null) {
					CreateParticles(x, y, origin);
					return target;
				}
			}


			return null;
		}

		public override void HitWire(int x, int y) {
		}
	}
}
