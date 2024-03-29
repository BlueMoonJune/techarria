﻿using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Techarria.Content.Items;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.NPC;

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

        public override void AI() {
			Projectile.rotation += 0.05f;
			if (Main.tile[(Projectile.Center / 16).ToPoint()].WallType == WallID.LihzahrdBrickUnsafe && !NPC.downedGolemBoss) {
				Projectile.velocity.Y += Player.defaultGravity;
				return;
			}
			Projectile.velocity *= 0.95f;


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

        public override void OnKill(int timeLeft) {
			if (Main.tile[(Projectile.Center / 16).ToPoint()].WallType == WallID.LihzahrdBrickUnsafe && !NPC.downedGolemBoss) {
				Projectile.velocity.Y += Player.defaultGravity;
				Projectile.NewProjectileDirect(new EntitySource_Misc("Visual"), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<EMPBlast>(), 0, 0);
				return;
			}
			Projectile.NewProjectile(new EntitySource_Misc("Visual"), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<EMPBlast>(), 0, 0);

			Rectangle explosionBoundingBox = new Rectangle((int)(Projectile.Center.X - range), (int)(Projectile.Center.Y - range), 512, 512);
            List<Point> tiles = Collision.GetTilesIn(explosionBoundingBox.TopLeft(), explosionBoundingBox.BottomRight());
            foreach (Point point in tiles)
            {
                Point center = new Point(point.X * 16 + 8, point.Y * 16 + 8);
                if ((new Vector2(center.X, center.Y) - Projectile.Center).Length() <= range) {

					Tile tile = Main.tile[point];
					if (tile.WallType == WallID.LihzahrdBrickUnsafe) {
						Dust.NewDust(new Vector2(point.X, point.Y) * 16, 16, 16, DustID.SomethingRed);
						continue;
					}
                    Dust.NewDust(new Vector2(point.X, point.Y) * 16, 16, 16, DustID.Electric);
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
            foreach (Player player in Main.player)
            {
                if ((player.Center - Projectile.Center).Length() > range)
                {
                    continue;
                }

                int damage = 0;
                if (player.armor[2].ModItem is ChargableItem)
                {
                    player.AddBuff(ModContent.BuffType<Buffs.MotorMalfunction>(), 600);
                    damage += 70;
                }
                if (player.armor[1].ModItem is ChargableItem)
                {
                    player.AddBuff(ModContent.BuffType<Buffs.CircuitOverload>(), 600);
                    damage += 70;
                }
                if (player.armor[0].ModItem is ChargableItem)
                {
                    player.AddBuff(ModContent.BuffType<Buffs.DisplayOffline>(), 600);
                    damage += 70;
                }
                if (Main.player[Projectile.owner] == player)
                    player.Hurt(PlayerDeathReason.ByCustomReason(player.name + " didn't realise EMPs break electronics"), damage, 0);
                else
                    player.Hurt(PlayerDeathReason.ByProjectile(Projectile.owner, Projectile.identity), damage, 0);
            }
            List<NPC> destroyerSegments = new List<NPC>();
            foreach (NPC npc in Main.npc)
            {
                if ((npc.Center - Projectile.Center).Length() > range)
                {
                    continue;
                }

				HitInfo hitInfo = new HitInfo();
				hitInfo.Damage = 1000;
				npc.StrikeNPC(new HitInfo());
				if (npc.type == NPCID.SkeletronPrime || npc.type == NPCID.PrimeCannon || npc.type == NPCID.PrimeLaser || npc.type == NPCID.PrimeSaw || npc.type == NPCID.PrimeVice)
                    npc.StrikeNPC(hitInfo);

                if (npc.type == NPCID.Spazmatism || npc.type == NPCID.Retinazer)
                    npc.StrikeNPC(hitInfo);

                if (npc.type == NPCID.Probe)
                    npc.StrikeNPC(hitInfo);

                if (npc.type == NPCID.TheDestroyer || npc.type == NPCID.TheDestroyerBody || npc.type == NPCID.TheDestroyerTail)
                    destroyerSegments.Add(npc);

            }
            foreach (NPC npc in destroyerSegments) {
				HitInfo hitInfo = new HitInfo();
				hitInfo.Damage = 2000 / destroyerSegments.Count;
				npc.StrikeNPC(new HitInfo());
				npc.StrikeNPC(hitInfo);
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
