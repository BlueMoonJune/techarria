using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Techarria.Content.Projectiles
{
	public class Lightning : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 38;
            Projectile.height = 28;

            AIType = ProjectileID.InfernoHostileBlast;

            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.tileCollide = false; // Can the projectile collide with tiles?
            Projectile.timeLeft = 180; // Each update timeLeft is decreased by 1. Once timeLeft hits 0, the Projectile will naturally despawn. (60 ticks = 1 second)


            Projectile.penetrate = -1;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Electrified, 60);
        }
    }
}
