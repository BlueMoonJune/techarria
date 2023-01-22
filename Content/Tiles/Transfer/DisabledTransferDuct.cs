using Terraria;
using Terraria.ModLoader;

namespace Techarria.Content.Tiles.Transfer
{
	/// <summary>
	/// The disabled variant of TransferDuct
	/// </summary>
	public class DisabledTransferDuct : TransferDuct
	{
		public override void SetStaticDefaults() {
			base.SetStaticDefaults();
			Techarria.tileIsTransferDuct[Type] = false;
		}

		public override void HitWire(int i, int j) {
			Main.tile[i, j].TileType = (ushort)ModContent.TileType<TransferDuct>();
		}
	}
}
