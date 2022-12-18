using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;

namespace Techarria.Content.Tiles
{
    /// <summary>
    /// The disabled variant of TransferDuct
    /// </summary>
    internal class DisabledTransferDuct : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileBlockLight[Type] = true;
            Main.tileFrameImportant[Type] = true;

            Techarria.tileConnectToPipe[Type] = true;

            AddMapEntry(Color.Blue, CreateMapEntryName());

            DustType = DustID.Stone;
            ItemDrop = ModContent.ItemType<Items.Placeables.TransferDuct>();

            HitSound = SoundID.Tink;
        }

        public override void HitWire(int i, int j)
        {
            Main.tile[i, j].TileType = (ushort) ModContent.TileType<TransferDuct>();
        }
    }
}
