using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Techarria.Content.Projectiles.Minions
{
	public class DroneApparatusBuff : ModBuff
	{

		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Laser drone");
			// Description.SetDefault("Little lady with little lasers");

			Main.buffNoSave[Type] = true; // This buff won't save when you exit the world
			Main.buffNoTimeDisplay[Type] = true; // The time remaining won't display on this buff
		}

		public override void Update(Player player, ref int buffIndex)
		{
			// If the minions exist reset the buff time, otherwise remove the buff from the player
			if (player.ownedProjectileCounts[ModContent.ProjectileType<DroneApparatus>()] > 0)
			{
			}
			else
			{
				player.DelBuff(buffIndex);
				buffIndex--;
			}
		}
	}
	public class DroneApparatus : ModProjectile
	{
		public int shootTimer = 0;
		public override void SetStaticDefaults() {
			// DisplayName.SetDefault("Drone Apparatus");
			Main.projFrames[Projectile.type] = 2;

			// right-click targetting
			ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;

			// so it follows you I think
			Main.projPet[Projectile.type] = false;
			// no cultist cheese
			ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
		}

		public override void SetDefaults() {
			Projectile.width = 38;
			Projectile.height = 31;

			// goes through tiles however she likes
			Projectile.tileCollide = false;

			// These below are needed for a minion weapon (and mostly copy+pasted from example mod)
			Projectile.friendly = false; // Only controls if it deals damage to enemies on contact
			Projectile.minion = true; // Declares this as a minion
			Projectile.DamageType = DamageClass.Summon; // Declares the damage type
			Projectile.minionSlots = 0f; // Amount of slots this minion occupies from the total minion slots available to the player
			Projectile.penetrate = -1; // Needed so the minion doesn't despawn on collision with enemies or tiles
		}
		// makes it not destroy your beuatiful lawn
		public override bool? CanCutTiles() {
			return false;
		}

		public override void AI() {
			Player owner = Main.player[Projectile.owner];

			if (owner.Distance(Projectile.Center) > 2880) {
				Projectile.Center = owner.Center;
			}

			if (!owner.HasBuff<DroneApparatusBuff>()) {
				Projectile.Kill();
				return;
			}

			Projectile.timeLeft = 2;

			SearchForTargets(owner, out bool foundTarget, out float distance, out Vector2 center);

			Vector2 moveTarget;
			if (foundTarget) 
			{
				Vector2 offset = Projectile.Center - center;
				offset.Normalize();
				offset *= 128;
				moveTarget = offset + center;
			}
			else 
			{
				moveTarget = owner.Center + new Vector2(0, -64);
			}

			Vector2 target = foundTarget ? center : owner.Center;

			if ((moveTarget - Projectile.Center).X > 8) {
				Projectile.direction = 1;
				Projectile.spriteDirection = 1;
			}
			if ((moveTarget - Projectile.Center).X < -8) {
				Projectile.direction = -1;
				Projectile.spriteDirection = -1;
			}

			Projectile.velocity += ((moveTarget - Projectile.Center) - Projectile.velocity / 0.05f) * 0.005f;
			if (Projectile.velocity.Length() > 10) {
				Projectile.velocity.Normalize();
				Projectile.velocity *= 10;
			}

			if (shootTimer > 0)
				shootTimer--;

			if (shootTimer == 0 && foundTarget) {
				Projectile proj = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), Projectile.Center, (target - Projectile.Center), ProjectileID.LaserMachinegunLaser, 10, 1, Projectile.owner);
				proj.DamageType = DamageClass.Summon;
				shootTimer = 15;
			}

			Visuals();
		}

		private void Visuals() {
			Vector2 accel = Projectile.velocity - Projectile.oldVelocity;
			accel += new Vector2(-Main.windSpeedCurrent * Main.windPhysicsStrength, -Player.defaultGravity) + Projectile.velocity * 0.01f;
			float angle = MathF.Atan2(accel.X, -accel.Y);
			Projectile.rotation = (Projectile.rotation - angle) * 0.9f + angle;


			if (++Projectile.frameCounter >= 2) {
				Projectile.frameCounter = 0;

				Projectile.frame = ++Projectile.frame % Main.projFrames[Projectile.type];
			}

			Lighting.AddLight((int)Projectile.Center.X / 16, (int)Projectile.Center.Y / 16, 0, 0.75f, 1);
		}

		private void SearchForTargets(Player owner, out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter) {
			// Starting search distance
			distanceFromTarget = 700f;
			targetCenter = Projectile.position;
			foundTarget = false;

			// This code is required if your minion weapon has the targeting feature
			if (owner.HasMinionAttackTargetNPC) {
				NPC npc = Main.npc[owner.MinionAttackTargetNPC];
				float between = Vector2.Distance(npc.Center, Projectile.Center);

				// Reasonable distance away so it doesn't target across multiple screens
				if (between < 2000f) {
					distanceFromTarget = between;
					targetCenter = npc.Center;
					foundTarget = true;
				}
			}

			if (!foundTarget) {
				// This code is required either way, used for finding a target
				for (int i = 0; i < Main.maxNPCs; i++) {
					NPC npc = Main.npc[i];

					if (npc.CanBeChasedBy()) {
						float between = Vector2.Distance(npc.Center, Projectile.Center);
						bool closest = Vector2.Distance(Projectile.Center, targetCenter) > between;
						bool inRange = between < distanceFromTarget;
						bool lineOfSight = Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, npc.position, npc.width, npc.height);
						// Additional check for this specific minion behavior, otherwise it will stop attacking once it dashed through an enemy while flying though tiles afterwards
						// The number depends on various parameters seen in the movement code below. Test different ones out until it works alright
						bool closeThroughWall = between < 100f;

						if (((closest && inRange) || !foundTarget) && (lineOfSight || closeThroughWall)) {
							distanceFromTarget = between;
							targetCenter = npc.Center;
							foundTarget = true;
						}
					}
				}
			}

		}
	}
}
