using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;

namespace Techarria.Content.Tiles
{
    internal class DisabledItemPipe : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileBlockLight[Type] = true;
            Main.tileFrameImportant[Type] = true;

            Techarria.tileConnectToPipe[Type] = true;

            AddMapEntry(Color.Blue, CreateMapEntryName());

            DustType = DustID.Stone;
            ItemDrop = ModContent.ItemType<Items.Placeables.ItemPipe>();

            HitSound = SoundID.Tink;
        }

        public override void HitWire(int i, int j)
        {
            Main.tile[i, j].TileType = (ushort) ModContent.TileType<ItemPipe>();
        }
    }
}
