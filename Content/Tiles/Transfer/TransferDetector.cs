using Microsoft.Xna.Framework;
using Techarria.Content.Dusts;
using Techarria.Transfer;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Techarria.Content.Tiles.Transfer
{
	public class TransferDetectorTE : ModTileEntity
	{
		public bool detected = false;

		public override bool IsTileValidForEntity(int x, int y) {
			return Main.tile[x, y].TileType == ModContent.TileType<TransferDetector>();
		}

		public override void Update() {
			if (detected) {
				Dust.NewDustDirect(new Vector2(Position.X, Position.Y) * 16 + new Vector2(4), 0, 0, ModContent.DustType<Indicator>());
				Wiring.TripWire(Position.X, Position.Y, 1, 1);
				detected = false;
			}
		}
	}

	/// <summary>
	/// Outputs a wire pulse when items are transfered through it
	/// </summary>
	public class TransferDetector : TransferDuct
	{
		public override void SetStaticDefaults() {
			base.SetStaticDefaults();
			ItemDrop = ModContent.ItemType<Items.Placeables.Transfer.TransferDetector>();
		}

		public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak) {
			return true;
		}

		public override void PlaceInWorld(int i, int j, Item item) {
			ModContent.GetInstance<TransferDetectorTE>().Place(i, j);
		}

		public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem) {
			if (fail || effectOnly) {
				return;
			}
			ModContent.GetInstance<TransferDetectorTE>().Kill(i, j);
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
					var tileEntity = TileEntity.ByPosition[new Point16(x, y)] as TransferDetectorTE;
					tileEntity.detected = true;
				}
				return target;
			}


			return null;
		}

		public override void HitWire(int i, int j) {
		}
	}
}
