
using Terraria;
using Terraria.ModLoader;

namespace Techarria.Content.Tiles
{
    public class StickyPiston : Piston
    {


        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            myType = ModContent.TileType<StickyPiston>();
            ItemDrop = ModContent.ItemType<Items.Placeables.StickyPiston>();
        }

        public override void Retract(int i, int j, int dir)
        {
            Tile tile = Framing.GetTileSafely(i, j);

            int x = i + dirToX(dir);
            int y = j + dirToY(dir);

            Techarria.BlockDusts = true;
            WorldGen.KillTile(x, y, false, false, true);
            Techarria.BlockDusts = false;
            tile.TileFrameY = 0;
            PushTile(x - i + x, y - j + y, (dir + 2) % 4, dir);
        }
    }
}
