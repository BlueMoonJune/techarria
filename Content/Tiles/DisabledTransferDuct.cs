using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;

namespace Techarria.Content.Tiles
{
    /// <summary>
    /// The disabled variant of TransferDuct
    /// </summary>
    internal class DisabledTransferDuct : TransferDuct
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Techarria.tileIsTransferDuct[Type] = false;
        }

        public override void HitWire(int i, int j)
        {
            Main.tile[i, j].TileType = (ushort) ModContent.TileType<TransferDuct>();
        }
    }
}
