
using Microsoft.Xna.Framework;
using System.Collections.Generic;
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

        public override void Extend(Point p, Direction dir)
        {
            int x = p.X + dir.point.X;
            int y = p.Y + dir.point.Y;

            if (Main.tile[x, y].HasTile)
            {
                List<Point> scanResult = Scan(new Point(x, y), dir);
                PushTiles(scanResult, dir);
            }
            else
            {
                List<Point> scanResult = Scan(new Point(x + dir.point.X, y + dir.point.Y), dir.Rotated(2));
                PushTiles(scanResult, dir.Rotated(2));
            }
        }
    }
}
