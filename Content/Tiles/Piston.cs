using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.Localization;
using System.Collections.Generic;
using System.Linq;
using Terraria.DataStructures;

namespace Techarria.Content.Tiles
{
    /// <summary>
    /// A tile that pushes blocks when powered. Has a push limit of 32.
    /// </summary>
    public class Piston : ModTile
    {
        public static List<Point> scanned = new List<Point>();

        public int myType;
        public static int blockCount = 0;

        public override void SetStaticDefaults()
        {
            myType = ModContent.TileType<Piston>();

            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileFrameImportant[Type] = true;

            AddMapEntry(Color.DarkSlateGray, CreateMapEntryName());

            DustType = DustID.Stone;
            ItemDrop = ModContent.ItemType<Items.Placeables.Piston>();

            HitSound = SoundID.Tink;
        }

        public override bool Slope(int i, int j)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            tile.TileFrameX = (short)((tile.TileFrameX + 16) % 64);
            return false;
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            return false;
        }

        public static void CloneTile(int i, int j, int x, int y)
        {
            Tile sourceTile = Main.tile[i, j];
            Tile destTile = Main.tile[x, y];
            Tile destTileReset = new Tile();
            destTileReset.CopyFrom(destTile);
            destTile.CopyFrom(sourceTile);
            sourceTile.ClearEverything();

            sourceTile.WallColor = destTile.WallColor;
            sourceTile.WallFrameNumber = destTile.WallFrameNumber;
            sourceTile.WallFrameX = destTile.WallFrameX;
            sourceTile.WallFrameY = destTile.WallFrameY;
            sourceTile.WallType = destTile.WallType;

            destTile.WallColor = destTileReset.WallColor;
            destTile.WallFrameNumber = destTileReset.WallFrameNumber;
            destTile.WallFrameX = destTileReset.WallFrameX;
            destTile.WallFrameY = destTileReset.WallFrameY;
            destTile.WallType = destTileReset.WallType;

            if (TileEntity.ByPosition.TryGetValue(new Point16(i, j), out TileEntity te))
            {
                te.Position = new Point16(x, y);
                TileEntity.ByPosition.Remove(new Point16(i, j));
                TileEntity.ByPosition.Add(new Point16(x, y), te);
            }

            if (Chest.FindChest(i, j) >= 0)
            {
                Chest chest = Main.chest[Chest.FindChest(i, j)];
                chest.x = x;
                chest.y = y;
            }

            Techarria.BlockDusts = true;
            sourceTile.ClearTile();
            Techarria.BlockDusts = false;


        }

        public static bool isImmovable(int i, int j)
        {
            Tile tile = Main.tile[i, j];
            if ((tile.TileType == ModContent.TileType<Piston>() || tile.TileType == ModContent.TileType<Piston>()) && tile.TileFrameY > 0)
            {
                return true;
            }

            if (tile.WallType == WallID.LihzahrdBrick || tile.WallType == WallID.LihzahrdBrickUnsafe)
            {
                return true;
            }

            return tile.TileType == TileID.Obsidian || tile.TileType == TileID.LihzahrdBrick;
        }

        public static bool isSticky(int i, int j)
        {
            int type = Main.tile[i, j].TileType;
            return (type == TileID.SlimeBlock || type == TileID.FrozenSlimeBlock || type == TileID.PinkSlimeBlock || type == TileID.HoneyBlock);
        }

        public static List<Point> SortFrontToBack(List<Point> points, Direction dir)
        {
            List<Point> sorted = new List<Point>();

            int maxX = 0;
            int minX = Main.maxTilesX;
            int maxY = 0;
            int minY = Main.maxTilesY;
            foreach (Point point in points)
            {
                if (point.X > maxX) maxX = point.X;
                if (point.X < minX) minX = point.X;
                if (point.Y > maxY) maxY = point.Y;
                if (point.Y < minY) minY = point.Y;
            }

            if (dir <= 1)
            {
                for (int x = maxX; x >= minX; x--)
                {
                    for (int y = maxY; y >= minY; y--)
                    {
                        if (points.Contains(new Point(x, y)))
                        {
                            sorted.Add(new Point(x, y));
                        }
                    }
                }
            }
            else
            {
                for (int x = minX; x <= maxX; x++)
                {
                    for (int y = minY; y <= maxY; y++)
                    {
                        if (points.Contains(new Point(x, y)))
                        {
                            sorted.Add(new Point(x, y));
                        }
                    }
                }
            }

            return sorted;
        }

        public static List<Point> Scan(Point p, Direction dir)
        {
            List<Point> result = new List<Point>();

            if (scanned.Contains(p)) return result;

            scanned.Add(p);

            if (!Main.tile[p].HasTile || isImmovable(p.X, p.Y))
            {
                return result;
            }

            Point TL = HelperMethods.GetTopLeftTileInMultitile(p.X, p.Y, out int width, out int height);

            if (width != 1 || height != 1 )
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        if (!scanned.Contains(new Point(TL.X + x, TL.Y + y)))
                        {
                            List<Point> ScanResult = Scan(new Point(TL.X + x, TL.Y + y), dir);
                            foreach (var point in ScanResult)
                            {
                                result.Add(point);
                            }
                        }
                    }
                }
            result.Add(p);

            if (isSticky(p.X, p.Y))
                foreach (Direction d in Direction.directions())
                {
                    Point t = p + d;
                    List<Point> stickyScanResult = Scan(t, dir);
                    foreach (var point in stickyScanResult)
                    {
                        result.Add(point);
                    }
                }

            List<Point> pushScanResult = Scan(p + dir, dir);
            foreach (var point in pushScanResult)
            {
                result.Add(point);
            }

            return result;
        }

        public bool CanPushTiles(List<Point> pairs, Direction dir)
        {
            if (pairs.Count > 64) return false;

            foreach (var point in pairs)
            {
                Point t = point + dir;
                if (Main.tile[t].HasTile)
                {
                    bool inList = false;
                    foreach (Point p in pairs)
                    {
                        if (p.X == t.X && p.Y == t.Y)
                        {
                            inList = true;
                        }
                    }
                    if (!inList)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public bool PushTiles(List<Point> pairs, Direction dir)
        {

            if (!CanPushTiles(pairs, dir))
            {
                return false;
            }

            List<Point> sorted = SortFrontToBack(pairs, dir);
            foreach (Point point in sorted)
            {
                Point t = point + dir;
                CloneTile(point.X, point.Y, t.X, t.Y);
            }

            foreach (Point point in sorted)
            {
                WorldGen.TileFrame(point.X, point.Y);
            }

            return true;
        }

        public virtual void Extend(Point p, Direction dir)
        {

            int x = p.X + dir.point.X;
            int y = p.Y + dir.point.Y;

            List<Point> scanResult = Scan(new Point(x, y), dir);
            if (CanPushTiles(scanResult, dir))
            SortFrontToBack(scanResult, dir);
            PushTiles(scanResult, dir);
        }

        public override void HitWire(int i, int j)
        {
            scanned.Clear();
            scanned.Add(new Point(i, j));
            Tile tile = Framing.GetTileSafely(i, j);
            if (tile.TileFrameY != 0)
            {
                return;
            }

            Direction dir = tile.TileFrameX / 16;

            Extend(new Point(i, j), dir);
        }
    }
}
