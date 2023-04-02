using Microsoft.Xna.Framework;
using System;
using Techarria.Content.Dusts;
using Techarria.Content.Items.Armor;
using Techarria.Content.Items.Armor.Apparatus;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Techarria.Common.Global
{
	public class GlobalMagicProjectile : GlobalProjectile
	{
		public override bool AppliesToEntity(Projectile entity, bool lateInstantiation) {
			return entity.DamageType.CountsAsClass(DamageClass.Magic);
		}

		public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone) {
		Player owner = Main.player[projectile.owner];

			PowerArmorPlayer player = owner.GetModPlayer<PowerArmorPlayer>();
			if (owner.armor[0].ModItem is TechnomancyApparatus helm) {
				Main.NewText("1");
				if (helm.IsArmorSet(owner.armor[0], owner.armor[1], owner.armor[2])) {
					Main.NewText("2");
					if (Main.rand.NextBool(3, 5)) {
						Main.NewText("3");
						for (int i = 0; i < 8; i++) {
							Vector2 pos = target.Center;
							Vector2 vel = new Vector2(10, 0).RotatedBy(i * MathHelper.PiOver4);
							Projectile proj = Projectile.NewProjectileDirect(new EntitySource_OnHit(owner, target), pos, vel, ProjectileID.SpikyBall, 1, 0, projectile.owner);
							proj.penetrate = 2;
						}
					}
				}
			}
		}
	}
}