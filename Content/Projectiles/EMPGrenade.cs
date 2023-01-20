﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Techarria.Content.Items;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Techarria.Content.Projectiles
{
    public class EMPGrenade : ModProjectile
    {
        public static int range = 16 * 12;

        public override void SetDefaults()
        {
            Projectile.width = 28;
            Projectile.height = 28;

            Projectile.aiStyle = 0;

            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.timeLeft = 60; // Each update timeLeft is decreased by 1. Once timeLeft hits 0, the Projectile will naturally despawn. (60 ticks = 1 second)
            Projectile.light = 1f;
            Projectile.tileCollide = true;

            Projectile.penetrate = -1;
        }

        public override void AI()
        {
            Projectile.velocity *= 0.95f;
            Projectile.rotation += 0.05f;

            Vector2 offset = new Vector2(range, 0);
            offset = offset.RotatedBy(Projectile.rotation);
            Dust.NewDust(Projectile.Center + offset, 0, 0, DustID.Electric);
            offset = new Vector2(-range, 0);
            offset = offset.RotatedBy(Projectile.rotation);
            Dust.NewDust(Projectile.Center + offset, 0, 0, DustID.Electric);
            offset = new Vector2(0, range);
            offset = offset.RotatedBy(Projectile.rotation);
            Dust.NewDust(Projectile.Center + offset, 0, 0, DustID.Electric);
            offset = new Vector2(0, -range);
            offset = offset.RotatedBy(Projectile.rotation);
            Dust.NewDust(Projectile.Center + offset, 0, 0, DustID.Electric);
        }

        public override void Kill(int timeLeft)
        {
            Projectile.NewProjectile(new EntitySource_Misc("Visual"), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<EMPBlast>(), 0, 0);

            Rectangle explosionBoundingBox = new Rectangle((int)(Projectile.Center.X - range), (int)(Projectile.Center.Y - range), 512, 512);
            List<Point> tiles = Collision.GetTilesIn(explosionBoundingBox.TopLeft(), explosionBoundingBox.BottomRight());
            foreach (Point point in tiles)
            {
                Point center = new Point(point.X * 16 + 8, point.Y * 16 + 8);
                if ((new Vector2(center.X, center.Y) - Projectile.Center).Length() <= range)
                {
                    Dust.NewDust(new Vector2(point.X, point.Y) * 16, 16, 16, DustID.Electric);
                    Tile tile = Main.tile[point];
                    for (int c = 0; c < 4; c++)
                    {
                        if (Power.GetWire(tile, c)) {
                            Item.NewItem(new EntitySource_TileBreak(point.X, point.Y), point.X * 16, point.Y * 16, 16, 16, ItemID.Wire);
                        }
                    }
                    tile.RedWire = false;
                    tile.BlueWire = false;
                    tile.GreenWire = false;
                    tile.YellowWire = false;
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Vector2 deltaV = Projectile.velocity - oldVelocity;
            Projectile.velocity += deltaV;
            return false;
        }
    }
}