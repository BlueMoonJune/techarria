using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.ObjectData;
using Techarria.Content.Dusts;
using Terraria.Audio;

namespace Techarria.Content.Tiles
{
    public class FoundContainer
    {
        public int x;
        public int y;
        public int dir;
        public Tile tile;
        public bool isNull = false;

        public FoundContainer setNull(bool setTo)
        {
            isNull = setTo;
            return this;
        }

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

    internal class TransferDuct : ModTile
    {

        int[,] lastDir = new int[Main.maxTilesX, Main.maxTilesY];

        public int dirToX(int dir)
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
        public int dirToY(int dir)
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

        public Vector2 dirToVec(int dir)
        {
            return new Vector2(dirToX(dir), dirToY(dir));
        }

        public int posToDir(Vector2 vec)
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

            Techarria.tileIsTransferDuct[Type] = true;
            Techarria.tileConnectToPipe[Type] = true;

            AddMapEntry(Color.Blue, CreateMapEntryName());

            DustType = DustID.Stone;
            ItemDrop = ModContent.ItemType<Items.Placeables.TransferDuct>();

            HitSound = SoundID.Tink;
        }

        public bool ShouldConnect(int i, int j, int sourceX, int sourceY)
        {
            return (
                Techarria.tileConnectToPipe[Main.tile[i, j].TileType] 
                && 
                MatchingPaint(sourceX, sourceY, i, j)
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

        public FoundContainer FindContainer(int i, int j)
        {
            int chest = Chest.FindChest(i, j);
            if (chest >= 0)
            {
                FoundContainer container = new FoundContainer();
                container.x = i;
                container.y = j;
                container.tile = Main.tile[i, j];
                return container;
            }
            else if (Main.tileContainer[Main.tile[i, j].TileType])
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

        public bool IsMatchingTile<T>(int x, int y) where T : ModTile
        {   
            return Main.tile[x, y].TileType == ModContent.TileType<T>();
        }
        
        public void CreateParticles(int x, int y, int dir)
        {
            Dust dust = Dust.NewDustDirect(new Vector2(x, y) * 16 + new Vector2(4), 0, 0, ModContent.DustType<Transfer>());
            dust.velocity = new Vector2(dirToX(dir), dirToY(dir));
        }

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


            return new FoundContainer().setNull(true);
        }


        public override void HitWire(int i, int j)
        {
            //Main.NewText("TransferDuct.HitWire(" + i + ", " + j + ")");
            FoundContainer container = FindAdjacentContainer(i, j);
            if (!container.isNull && !container.isEmpty())
            {
                Dust suction = Dust.NewDustDirect(new Vector2(i + dirToX(container.dir), j + dirToY(container.dir)) * 16 + new Vector2(4), 0, 0, ModContent.DustType<Suction>());
                suction.velocity = new Vector2(-dirToX(container.dir), -dirToY(container.dir));
                //Main.NewText("Found Source!");
                Chest source = Main.chest[Chest.FindChest(container.x, container.y)];
                foreach (Item item in source.item)
                {
                    if (!item.IsAir)
                    {
                        //Main.NewText("Found an item: " + item.Name);

                        FoundContainer target = EvaluatePath(i, j, item, (container.dir + 2) % 4, 0);
                        if (!target.isNull && target.canDeposit(item))
                        {
                            //CreateParticles(i + dirToX(container.dir), j + dirToY(container.dir), (container.dir + 2) % 4);
                            //Main.NewText("Destination found at X:" + target.x + " Y:" + target.y);
                            SoundEngine.PlaySound(new SoundStyle("Techarria/Content/Sounds/Transfer"), new Vector2(i, j) * 16);
                            Wiring.SkipWire(i, j);
                            Chest destination = Main.chest[Chest.FindChest(target.x, target.y)];
                            foreach (Item destSlot in destination.item)
                            {
                                if (destSlot.type == item.type && destSlot.stack < destSlot.maxStack) {
                                    //Main.NewText("Destination has existing stack of " + item.Name + ", sending there");
                                    destSlot.stack++;
                                    item.stack--;
                                    if (item.stack <= 0)
                                    {
                                        item.TurnToAir();
                                    }
                                    //Main.NewText("Item Transfered");
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
