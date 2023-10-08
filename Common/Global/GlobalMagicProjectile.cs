using Microsoft.Xna.Framework;
using System;
using Techarria.Content.Dusts;
using Techarria.Content.Items.Armor;
using Techarria.Content.Items.Armor.Apparatus;
using Techarria.Content.NPCs;
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
				if (helm.IsArmorSet(owner.armor[0], owner.armor[1], owner.armor[2])) {
					if (Main.rand.NextBool(3, 5)) {
						for (int i = 0; i < 8; i++) {
							Vector2 pos = target.Center;
							Vector2 vel = new Vector2(5, 0).RotatedBy(Main.rand.NextFloat() * MathHelper.TwoPi);
							Projectile proj = Projectile.NewProjectileDirect(new EntitySource_OnHit(owner, target), pos, vel, ModContent.ProjectileType<SpikedDungeonSlimeProjectile>(), projectile.damage/10, 0, projectile.owner);
							proj.friendly = true;
							proj.hostile = false;
							proj.penetrate = -1;
						}
					}
				}
			}
		}
	}
}