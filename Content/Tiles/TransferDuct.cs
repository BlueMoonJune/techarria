using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.ObjectData;
using Techarria.Content.Dusts;
using Terraria.Audio;

namespace Techarria.Content.Tiles
{
    ///<summary>
    ///A class containing some information relating to containers used by item transport
    ///</summary>
    public class FoundContainer
    {
        public int x;
        public int y;
        /// <summary>Direction from the source. 0 = right, 1 = down, 2 = left, 3 = up</summary>
        public int dir;
        /// <summary>The tile that is the top left of the container</summary>
        public Tile tile;
        /// <summary>Set to true when no container is found</summary>
        public bool isNull = false;

        /// <summary>Sets 'isNull' to 'setTo' and returns the container</summary>
        public FoundContainer setNull(bool setTo)
        {
            isNull = setTo;
            return this;
        }

        /// <summary>Scans the contents of the container, checking if it is empty</summary>
        public bool isEmpty()
        {
            if (isNull)
            {
                return false;
            }

            Chest chest = Main.chest[Chest.FindChest(x, y)];
            foreach (Item item in chest.item)
            {
                if (!item.IsAir)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>Scans the contents of the container, checking if 'item' can be inserted</summary>
        public bool canDeposit(Item item)
        {
            if (isNull)
            {
                return false;
            }

            Chest chest = Main.chest[Chest.FindChest(x, y)];
            foreach (Item slot in chest.item)
            {
                if (slot.IsAir || slot.type == item.type && item.stack < item.maxStack)
                {
                    return true;
                }
            }

            return false;


        }
    }

    /// <summary>Basic item transportation tile. Other item transfer tiles should extend this</summary>
    internal class TransferDuct : ModTile
    {

        /// <summary>That last direction that an item was sent in. Used for round-robin</summary>
        int[,] lastDir = new int[Main.maxTilesX, Main.maxTilesY];

        /// <summary>takes a direction and returns the X component of the coresponding vector</summary>
        /// <param name="dir">The input direction. 0 = right, 1 = down, 2 = left, 3 = up</param>
        /// <returns>The X component of the vector coresponding to 'dir'</returns>
        public static int dirToX(int dir)
        {
            switch(dir % 4)
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
            int x = (int) vec.X;
            int y = (int) vec.Y;
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

        /// <summary>Checks if the 2 tiles have matching paints. If one of the tiles has no paint, returns true</summary>
        /// <param name="x">X coordinate of tile 1</param>
        /// <param name="y">Y coordinate of tile 1</param>
        /// <param name="i">X coordinate of tile 2</param>
        /// <param name="j">Y coordinate of tile 2</param>
        /// <returns>Whether the 2 tiles have matching paints. If one of the tiles has no paint, will be true</returns>
        public bool MatchingPaint(int x, int y, int i, int j)
        {
            return
                Main.tile[x, y].TileColor == 0
                ||
                Main.tile[i, j].TileColor == 0
                ||
                Main.tile[i, j].TileColor == Main.tile[x, y].TileColor;
        }

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = false;

            Techarria.tileIsTransferDuct[Type] = true;
            Techarria.tileConnectToPipe[Type] = true;

            AddMapEntry(Color.Blue, CreateMapEntryName());

            DustType = DustID.Stone;
            ItemDrop = ModContent.ItemType<Items.Placeables.TransferDuct>();

            HitSound = SoundID.Tink;
        }

        /// <summary>
        /// Returns whether the source tile should connect to the target tile
        /// </summary>
        /// <param name="i">X coordinate of the target tile</param>
        /// <param name="j">Y coordinate of the target tile</param>
        /// <param name="x">X coordinate of the source tile</param>
        /// <param name="y">Y coordinate of the source tile</param>
        /// <returns></returns>
        public bool ShouldConnect(int i, int j, int x, int y)
        {
            return (
                Techarria.tileConnectToPipe[Main.tile[i, j].TileType] 
                && 
                MatchingPaint(x, y, i, j)
                || 
                !FindContainer(i, j).isNull
            );

        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            tile.TileFrameX = 0;
            if (ShouldConnect(i + 1, j, i, j)) 
            {
                tile.TileFrameX += 16;
            }
            if (ShouldConnect(i - 1, j, i, j))
            {
                tile.TileFrameX += 32;
            }

            tile.TileFrameY = 0;
            if (ShouldConnect(i, j + 1, i, j))
            {
                tile.TileFrameY += 16;
            }
            if (ShouldConnect(i, j - 1, i, j))
            {
                tile.TileFrameY += 32;
            }

            return true;
        }

        /// <summary>
        /// Finds a container at the specified coordinates
        /// </summary>
        /// <param name="i">X coordinate</param>
        /// <param name="j">Y coordinate</param>
        /// <returns>The FoundContainer object coresponding to the found container</returns>
        public FoundContainer FindContainer(int i, int j)
        {
            if (i < 0 || j < 0) 
            {
                return new FoundContainer().setNull(true);
            }
            int chest = Chest.FindChest(i, j);
            if (chest >= 0)
            {
                FoundContainer container = new FoundContainer();
                container.x = i;
                container.y = j;
                container.tile = Main.tile[i, j];
                return container;
            }
            if (Main.tileContainer[Main.tile[i, j].TileType])
            {
                var tileData = TileObjectData.GetTileData(Main.tile[i, j]);
                int frameX = Main.tile[i, j].TileFrameX;
                int frameY = Main.tile[i, j].TileFrameY;

                int partFrameX = frameX % tileData.CoordinateFullWidth;
                int partFrameY = frameY % tileData.CoordinateFullHeight;

                int partX = partFrameX / (tileData.CoordinateWidth + tileData.CoordinatePadding);
                int partY = 0;
                int remainingFrame = partFrameY;
                while (remainingFrame > 0)
                {
                    remainingFrame -= tileData.CoordinateHeights[partY] + tileData.CoordinatePadding;
                    partY++;
                }

                FoundContainer container = new FoundContainer();
                container.x = i - partX;
                container.y = j - partY;
                container.tile = Main.tile[i, j];
                if (!container.isNull)
                {
                    return container;
                }
            }
            return new FoundContainer().setNull(true);
        }

        /// <summary>
        /// Finds a container adjacent to the specified coordinates
        /// </summary>
        /// <param name="i">X coordinate</param>
        /// <param name="j">Y coordinate</param>
        /// <returns>The FoundContainer object coresponding to the found container</returns>
        public FoundContainer FindAdjacentContainer(int i, int j)
        {
            FoundContainer container;
            container = FindContainer(i + 1, j);
            if (!container.isNull)
            {
                container.dir = 0;
                return container;
            }
            container = FindContainer(i, j + 1); 
            if (!container.isNull)
            {
                container.dir = 1;
                return container;
            }
            container = FindContainer(i - 1, j);
            if (!container.isNull)
            {
                container.dir = 2;
                return container;
            }
            container = FindContainer(i, j - 1);
            if (!container.isNull)
            {
                container.dir = 3;
                return container;
            }
            return container;
        }

        /// <summary>
        /// Checks to see if the tile at the specified coordinates is of the specified type. Does not check for extending types
        /// </summary>
        /// <typeparam name="T">The type to test for</typeparam>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <returns></returns>
        public bool IsMatchingTile<T>(int x, int y) where T : ModTile
        {   
            return Main.tile[x, y].TileType == ModContent.TileType<T>();
        }
        
        /// <summary>
        /// Creates particles for item transfer
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinates</param>
        /// <param name="dir">Direction of item transfer. -1 causes no motion and is used for transfer failure</param>
        public virtual void CreateParticles(int x, int y, int dir)
        {
            Dust dust = Dust.NewDustDirect(new Vector2(x, y) * 16 + new Vector2(4), 0, 0, ModContent.DustType<Transfer>());
            if (dir >= 0)
            {
                dust.velocity = new Vector2(dirToX(dir), dirToY(dir));
                return;
            }
        }

        /// <summary>
        /// The pathfinding function for item transfer. This is a recursive algorithm that stops once a container is found
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <param name="item">The item that is trying to be transfered</param>
        /// <param name="origin">The direction the item is being sent from</param>
        /// <param name="depth">How 'deep' the recursive algorithm has gone. Should cause breakage after a certain threshold</param>
        /// <returns>The container that was found, either by this call or passed back up from the recursion</returns>
        public virtual FoundContainer EvaluatePath(int x, int y, Item item, int origin, int depth)
        {
            origin = (origin + 2) % 4;
            if (depth >= 256)
            {
                Main.LocalPlayer.PickTile(x, y, 40000);
            }
            FoundContainer container = FindAdjacentContainer(x, y);
            if (!container.isNull && !(container.dir == origin))
            {
                CreateParticles(x, y, container.dir);
                return container;
            }
            for (int c = 0; c < 4; c++)
            {
                int dir = (c + lastDir[x, y] + 1) % 4;
                int i = x + dirToX(dir);
                int j = y + dirToY(dir);
                if (Techarria.tileIsTransferDuct[Main.tile[i, j].TileType] && MatchingPaint(x, y, i, j) && dir != origin)
                {
                    FoundContainer target = ((TransferDuct) TileLoader.GetTile(Main.tile[i, j].TileType)).EvaluatePath(x + dirToX(dir), y + dirToY(dir), item, dir, depth + 1);
                    if (!target.isNull)
                    {
                        lastDir[x, y] = dir;
                        CreateParticles(x, y, dir);
                        return target;
                    }
                }
            }

            CreateParticles(x, y , -1);


            return new FoundContainer().setNull(true);
        }


        public override void HitWire(int i, int j)
        {
            FoundContainer container = FindAdjacentContainer(i, j);
            if (!container.isNull && !container.isEmpty())
            {
                Dust suction = Dust.NewDustDirect(new Vector2(i + dirToX(container.dir), j + dirToY(container.dir)) * 16 + new Vector2(4), 0, 0, ModContent.DustType<Suction>());
                suction.velocity = new Vector2(-dirToX(container.dir), -dirToY(container.dir));
                Chest source = Main.chest[Chest.FindChest(container.x, container.y)];
                foreach (Item item in source.item)
                {
                    if (!item.IsAir)
                    {

                        FoundContainer target = EvaluatePath(i, j, item, (container.dir + 2) % 4, 0);
                        if (!target.isNull && target.canDeposit(item))
                        {
                            SoundEngine.PlaySound(new SoundStyle("Techarria/Content/Sounds/Transfer"), new Vector2(i, j) * 16);
                            Wiring.SkipWire(i, j);
                            Chest destination = Main.chest[Chest.FindChest(target.x, target.y)];
                            foreach (Item destSlot in destination.item)
                            {
                                if (destSlot.type == item.type && destSlot.stack < destSlot.maxStack) {
                                    destSlot.stack++;
                                    item.stack--;
                                    if (item.stack <= 0)
                                    {
                                        item.TurnToAir();
                                    }
                                    return;
                                }
                            }

                            //Main.NewText("No existing stack of " + item.Name + ". Creating new stack");

                            for (int index = 0; index < destination.item.Length; index++ )
                            {
                                if (destination.item[index].IsAir)
                                {
                                    destination.item[index] = item.Clone();
                                    destination.item[index].stack = 1;
                                    item.stack--;
                                    if (item.stack <= 0)
                                    {
                                        item.TurnToAir();
                                    }
                                    //Main.NewText("Item Transfered");
                                    return;
                                }
                            }
                        }
                    }
                }
            } else
            {
                Main.tile[i, j].TileType = (ushort)ModContent.TileType<DisabledTransferDuct>();
            }
        }
    }
}
