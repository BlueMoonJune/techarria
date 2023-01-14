using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.Localization;

namespace Techarria.Content.Tiles
{
    /// <summary>
    /// A tile that pushes blocks when powered. Has a push limit of 32.
    /// </summary>
    internal class Piston : ModTile
    {
        public int myType;
        public static int blockCount = 0;
        public static int dirToX(int dir)
        {
            switch (dir % 4)
            {
                case 0:
                    return 1;
                case 2:
                    return -1;
                default:
                    return 0;
            }
        }

        /// <summary>takes a direction and returns the Y component of the coresponding vector</summary>
        /// <param name="dir">The input direction. 0 = right, 1 = down, 2 = left, 3 = up</param>
        /// <returns>The Y component of the vector coresponding to 'dir'</returns>
        public static int dirToY(int dir)
        {
            switch (dir % 4)
            {
                case 1:
                    return 1;
                case 3:
                    return -1;
                default:
                    return 0;
            }
        }

        /// <summary>takes a direction and returns the coresponding vector</summary>
        /// <param name="dir">The input direction. 0 = right, 1 = down, 2 = left, 3 = up</param>
        /// <returns>The vector coresponding to 'dir'</returns>
        public static Point dirToVec(int dir)
        {
            return new Point(dirToX(dir), dirToY(dir));
        }

        /// <summary>Takes a point and returns the coresponding direction</summary>
        /// <param name="dir">The input point</param>
        /// <returns>The coresponding direction. 0 = right, 1 = down, 2 = left, 3 = up</returns>
        public static int posToDir(Point vec)
        {
            int x = (int)vec.X;
            int y = (int)vec.Y;
            if (x == 1 && y == 0)
            {
                return 0;
            }
            if (x == 0 && y == 1)
            {
                return 1;
            }
            if (x == -1 && y == 0)
            {
                return 2;
            }
            return 3;
        }

        /// <summary>Takes a point and returns the coresponding direction</summary>
        /// <param name="dir">The input point</param>
        /// <returns>The coresponding direction. 0 = right, 1 = down, 2 = left, 3 = up</returns>
        public int posToDir(int x, int y)
        {
            if (x == 1 && y == 0)
            {
                return 0;
            }
            if (x == 0 && y == 1)
            {
                return 1;
            }
            if (x == -1 && y == 0)
            {
                return 2;
            }
            return 3;
        }

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
            Tile destTileReset = (Tile) destTile.Clone();
            WorldGen.PlaceTile(x, y, sourceTile.TileType, true, true);
            destTile.CopyFrom(sourceTile);
            destTile.WallColor = destTileReset.WallColor;
            destTile.WallFrameNumber = destTileReset.WallFrameNumber;
            destTile.WallFrameX = destTileReset.WallFrameX;
            destTile.WallFrameY = destTileReset.WallFrameY;
            destTile.WallType = destTileReset.WallType;
            Techarria.BlockDusts = true;
            WorldGen.KillTile(i, j, false, false, true);
            Techarria.BlockDusts = false;
        }

        public static bool isImmovable(int type)
        {
            return (type == TileID.Obsidian || type == TileID.LihzahrdBrick);
        }

        public static bool isSticky(int type)
        {
            return (type == TileID.SlimeBlock || type == TileID.FrozenSlimeBlock || type == TileID.PinkSlimeBlock || type == TileID.HoneyBlock);
        }

        public static bool PushSticky(int i, int j, int dir, int origin = -1)
        {
            Techarria.print("Pushing Sticky");
            bool success = true;
            Tile myTile = Main.tile[i, j];
            if (!isSticky(myTile.TileType))
            {
                return false;
            }
            for (int k = 0; k < 4; k++)
            {
                int x = i + dirToX(k);
                int y = j + dirToY(k);
                Techarria.print("Moving direction " + k);
                if (k != dir && k != origin && !isImmovable(Main.tile[x, y].TileType) && Main.tile[x, y].HasTile)
                {
                    success = success && PushTile(x, y, dir, k);
                }
            }
            return success;
        }

        public static bool PushTile(int i, int j, int dir, int origin = -1)
        {
            
            if (origin == -1)
            {
                origin = (dir + 2) % 4;
            }
            Tile myTile = Main.tile[i, j];

            if (isSticky(myTile.TileType))
            {
                if (!PushSticky(i, j, dir, origin))
                {
                    return false;
                }
            }

            if (isImmovable(myTile.TileType))
            {
                return false;
            }
            if (!Main.tile[i, j].HasTile)
            {
                return true;
            }
            if (blockCount >= 32)
            {
                return false;
            }

            int x = i + dirToX(dir);
            int y = j + dirToY(dir);
            Tile destTile = Main.tile[x, y];
            if (!destTile.HasTile || PushTile(x, y, dir))
            {
                CloneTile(i, j, x, y);
                myTile.ClearEverything();
                blockCount++;
                return true;
            }
            return false;
        }

        public virtual void Retract(int i, int j, int dir)
        {
            Tile tile = Framing.GetTileSafely(i, j);

            int x = i + dirToX(dir);
            int y = j + dirToY(dir);

            Techarria.BlockDusts = true;
            WorldGen.KillTile(x, y, false, false, true);
            Techarria.BlockDusts = false;
            tile.TileFrameY = 0;
        }

        public override void HitWire(int i, int j)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            if (tile.TileFrameY == 32)
            {
                return;
            }

            int dir = tile.TileFrameX / 16;

            int x = i + dirToX(dir);
            int y = j + dirToY(dir);
            
            if (tile.TileFrameY == 16 && Main.tile[x, y].TileFrameY == 32 && Main.tile[x, y].TileType == myType)
            {
                Retract(i, j, dir);
                return;
            }
            blockCount = 0;
            if (PushTile(x, y, dir))
            {
                WorldGen.PlaceTile(x, y, myType, false, true);
                Main.tile[x, y].TileFrameX = tile.TileFrameX;
                Main.tile[x, y].TileFrameY = 32;
                tile.TileFrameY = 16;
            }
        }

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (!effectOnly)
            {
                Tile tile = Framing.GetTileSafely(i, j);
                int dir = tile.TileFrameX / 16;
                int x = i + dirToX(dir);
                int y = j + dirToY(dir);

                if (tile.TileFrameY == 16 && Main.tile[x, y].TileFrameY == 32 && Main.tile[x, y].TileType == myType)
                {
                    Retract(i, j, dir);
                    return;
                }
            }
    
            base.KillTile(i, j, ref fail, ref effectOnly, ref noItem);
        }
    }
}
