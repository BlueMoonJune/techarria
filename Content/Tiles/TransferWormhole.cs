using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Techarria.Content.Dusts;
using System;
using System.Collections;

namespace Techarria.Content.Tiles
{
    /// <summary>
    /// Ductless item transfer tile
    /// </summary>
    internal class TransferWormhole : TransferDuct
    {
        /// <summary>Stores the primary dust object for the ID. Used for cooldowns</summary>
        public Dust[] dusts = new Dust[2048];
        /// <summary>Stores the last wormhole this ID has transfered to</summary>
        public int[] lastTarget = new int[2048];

        /// <summary>
        /// returns the link item for the wormhole at this position
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns>The item type</returns>
        public int getItem(int i, int j)
        {
            return Techarria.wormholeLinkItems[Techarria.wormholeIDs[i, j]];
        }

        /// <summary>
        /// Creates particles for item transfer
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinates</param>
        /// <param name="dir">Ignored in this function</param>
        public override void CreateParticles(int x, int y, int dir)
        {
            for (int i = 0; i < 16; i++)
            {
                Dust dust;
                dust = Dust.NewDustDirect(new Vector2(x, y) * 16 + new Vector2(4) + new Vector2(65, 1).RotatedBy(i / 8f * Math.PI), 0, 0, ModContent.DustType<AntiWormhole>());
                dust.velocity = new Vector2(-1, 0).RotatedBy(i / 8f * Math.PI) * 2;
                dust.frame.Y = 0;
                dust = Dust.NewDustDirect(new Vector2(x, y) * 16 + new Vector2(4) + new Vector2(56.875f, 2).RotatedBy((i + 0.25) / 8f * Math.PI), 0, 0, ModContent.DustType<AntiWormhole>());
                dust.velocity = new Vector2(-1, 0).RotatedBy((i + 0.25) / 8f * Math.PI) * 1.75f;
                dust.frame.Y = 8;
                dust = Dust.NewDustDirect(new Vector2(x, y) * 16 + new Vector2(4) + new Vector2(48.75f, 3).RotatedBy((i + 0.5) / 8f * Math.PI), 0, 0, ModContent.DustType<AntiWormhole>());
                dust.velocity = new Vector2(-1, 0).RotatedBy((i + 0.5) / 8f * Math.PI) * 1.5f;
                dust.frame.Y = 16;
                dust = Dust.NewDustDirect(new Vector2(x, y) * 16 + new Vector2(4) + new Vector2(40.625f, 4).RotatedBy((i + 0.75) / 8f * Math.PI), 0, 0, ModContent.DustType<AntiWormhole>());
                dust.velocity = new Vector2(-1, 0).RotatedBy((i + 0.75) / 8f * Math.PI) * 1.25f;
                dust.frame.Y = 24;

            }
            dusts[Techarria.wormholeIDs[x, y]] = Dust.NewDustDirect(new Vector2(x, y) * 16 + new Vector2(4), 0, 0, ModContent.DustType<BlackHole>());
        }

        /// <summary>
        /// Creates particles for when the wormhole can not find a destination
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void CreateFailureParticles(int x, int y)
        {
            for (int i = 0; i < 16; i++)
            {
                Dust dust;
                dust = Dust.NewDustDirect(new Vector2(x, y) * 16 + new Vector2(4) + new Vector2(65, 1).RotatedBy(i / 8f * Math.PI), 0, 0, ModContent.DustType<AntiWormhole>());
                dust.velocity = new Vector2(-1, 0).RotatedBy(i / 8f * Math.PI) * 2;
                dust.frame.Y = new Random().Next(4) * 8;
                dust = Dust.NewDustDirect(new Vector2(x, y) * 16 + new Vector2(4) + new Vector2(0, 1).RotatedBy(i / 8f * Math.PI), 0, 0, ModContent.DustType<Wormhole>());
                dust.velocity = new Vector2(1, 0).RotatedBy(i / 8f * Math.PI) * 2;
                dust.frame.Y = new Random().Next(4) * 8;

            }

        }

        /// <summary>
        /// Creates particles for when the wormhole receives an item
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void CreateTargetParticles(int x, int y)
        {
            for (int i = 0; i < 16; i++)
            {
                Dust dust;
                dust = Dust.NewDustDirect(new Vector2(x, y) * 16 + new Vector2(4) + new Vector2(0, 1).RotatedBy(i / 8f * Math.PI), 0, 0, ModContent.DustType<Wormhole>());
                dust.velocity = new Vector2(1, 0).RotatedBy(i / 8f * Math.PI) * 2;
                dust.frame.Y = 0;
                dust = Dust.NewDustDirect(new Vector2(x, y) * 16 + new Vector2(4) + new Vector2(0, 2).RotatedBy((i - 0.25) / 8f * Math.PI), 0, 0, ModContent.DustType<Wormhole>());
                dust.velocity = new Vector2(1, 0).RotatedBy((i + 0.25) / 8f * Math.PI) * 1.75f;
                dust.frame.Y = 8;
                dust = Dust.NewDustDirect(new Vector2(x, y) * 16 + new Vector2(4) + new Vector2(0, 3).RotatedBy((i - 0.5) / 8f * Math.PI), 0, 0, ModContent.DustType<Wormhole>());
                dust.velocity = new Vector2(1, 0).RotatedBy((i + 0.5) / 8f * Math.PI) * 1.5f;
                dust.frame.Y = 16;
                dust = Dust.NewDustDirect(new Vector2(x, y) * 16 + new Vector2(4) + new Vector2(0, 4).RotatedBy((i - 0.75) / 8f * Math.PI), 0, 0, ModContent.DustType<Wormhole>());
                dust.velocity = new Vector2(1, 0).RotatedBy((i + 0.75) / 8f * Math.PI) * 1.25f;
                dust.frame.Y = 24;

            }
            Dust.NewDustDirect(new Vector2(x, y) * 16 + new Vector2(4), 0, 0, ModContent.DustType<WhiteHole>());

        }

        /// <summary>
        /// Finds suitable target wormholes
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>An array of 'Point's for suitable targets</returns>
        public ArrayList findTarget(int x, int y)
        {
            ArrayList targets = new ArrayList();
            if (Techarria.wormholeIDs[x, y] >= 0 && getItem(x, y) != 0)
            {
                for (int i = 0; i < 2048; i++)
                {
                    int j = (i + lastTarget[Techarria.wormholeIDs[x, y]] + 1) % 2048;
                    if (Techarria.wormholeLinkItems[j] == getItem(x, y) && j != Techarria.wormholeIDs[x, y])
                    {
                        targets.Add(Techarria.wormholePositions[j]);
                    }
                }
            }
            return targets;
        }

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ItemDrop = ModContent.ItemType<Items.Placeables.TransferWormhole>();
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            return true;
        }

        public override FoundContainer EvaluatePath(int x, int y, Item item, int origin, int depth)
        {
            if (dusts[Techarria.wormholeIDs[x, y]] != null && dusts[Techarria.wormholeIDs[x, y]].active)
            {
                return new FoundContainer().setNull(true);
            }

            foreach (Point receiver in findTarget(x, y))
            {
                if (receiver.X == -1 && receiver.Y == -1)
                {
                    return new FoundContainer().setNull(true);
                }

                FoundContainer container = FindAdjacentContainer(receiver.X, receiver.Y);
                if (!container.isNull && container.dir == origin)
                {
                    CreateTargetParticles(receiver.X, receiver.Y);
                    CreateParticles(x, y, container.dir);
                    return container;
                }

                Main.NewText(receiver.X + ", " + receiver.Y);
                int i = dirToX(origin) + receiver.X;
                int j = dirToY(origin) + receiver.Y;
                if (Techarria.tileIsTransferDuct[Main.tile[i, j].TileType])
                {
                    FoundContainer target = ((TransferDuct)TileLoader.GetTile(Main.tile[i, j].TileType)).EvaluatePath(i, j, item, origin, depth + 1);
                    if (!target.isNull)
                    {
                        lastTarget[Techarria.wormholeIDs[x, y]] = Techarria.wormholeIDs[receiver.X, receiver.Y];
                        CreateTargetParticles(receiver.X, receiver.Y);
                        CreateParticles(x, y, origin);
                        return target;
                    }
                }
            }

            CreateFailureParticles(x, y);

            return new FoundContainer().setNull(true);
        }

        public override void PlaceInWorld(int i, int j, Item item)
        {
            base.PlaceInWorld(i, j, item);
            for (int x = 0; x < 2048; x++)
            {
                if (Techarria.wormholePositions[x] == Point.Zero)
                {
                    Techarria.wormholePositions[x] = new Point(i, j);
                    Techarria.wormholeIDs[i, j] = x;
                    return;
                }
            }
        }

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            base.KillTile(i, j, ref fail, ref effectOnly, ref noItem);
            if (Techarria.wormholeIDs[i, j] >= 0)
            {
                Techarria.wormholePositions[Techarria.wormholeIDs[i, j]] = Point.Zero;
                Techarria.wormholeIDs[i, j] = -1;
            }
        }

        public override void HitWire(int i, int j)
        {
        }

        public override bool RightClick(int i, int j)
        {
            Main.mouseRightRelease = false;
            Techarria.wormholeLinkItems[Techarria.wormholeIDs[i, j]] = Main.player[Main.myPlayer].HeldItem.type;
            return true;
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            if (Techarria.wormholeLinkItems[Techarria.wormholeIDs[i, j]] != 0)
            {
                player.cursorItemIconEnabled = true;
                player.cursorItemIconID = Techarria.wormholeLinkItems[Techarria.wormholeIDs[i, j]];
            }

            Item item = new Item();
            item.TurnToAir();
        }
    }
}
