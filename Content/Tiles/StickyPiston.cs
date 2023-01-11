
using Terraria;
using Terraria.ModLoader;

namespace Techarria.Content.Tiles
{
    internal class StickyPiston : Piston
    {

        public override void SetStaticDefaults()
        {
            myType = ModContent.TileType<StickyPiston>();
            base.SetStaticDefaults();
            ItemDrop = ModContent.ItemType<Items.Placeables.Piston>();
        }

        public override void Retract(int i, int j, int dir)
        {
            Tile tile = Framing.GetTileSafely(i, j);

            int x = i + dirToX(dir);
            int y = j + dirToY(dir);

            WorldGen.KillTile(x, y, false, false, true);
            tile.TileFrameY = 0;
            PushTile(x - i + x, y - j + y, (dir + 2) % 4, dir);
        }
    }
}
