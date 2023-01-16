using System;
using Terraria;
using Terraria.ModLoader;

namespace Techarria.Content.Projectiles
{
    public class EMPBlast : ModProjectile
    {
        public static int range = 16 * 12;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 400;
            Projectile.height = 400;
            Projectile.scale = 0;

            Projectile.aiStyle = 0;

            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.tileCollide = false; // Can the projectile collide with tiles?
            Projectile.timeLeft = 40; // Each update timeLeft is decreased by 1. Once timeLeft hits 0, the Projectile will naturally despawn. (60 ticks = 1 second)
            Projectile.light = 1f;

            Projectile.penetrate = -1;

            DrawOffsetX = -200;
            DrawOriginOffsetX = 0;
            DrawOriginOffsetY = -200;
        }

        public override void AI()
        {
            Projectile.scale = (Projectile.scale - 1) * .90f + 1;

            Projectile.alpha = (int)(Math.Pow(Projectile.scale, 2) * 255);

            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;

                Projectile.frame = ++Projectile.frame % Main.projFrames[Projectile.type];
            }
        }
    }
}
