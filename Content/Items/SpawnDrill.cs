using Techarria.Drills;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Techarria
{
    public class SpawnDrill : GlobalProjectile
    {
        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            Player player = Main.player[projectile.owner];

            if (projectile.type == ContentSamples.ProjectilesByType[player.HeldItem.shoot].type && projectile.aiStyle == 99)
            {
                Main.NewText("yes");
                Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center.X, projectile.Center.Y - 1f, Main.rand.NextBool() ? 1 : -1,
                    Main.rand.NextBool() ? 1 : -1, ModContent.ProjectileType<IronDrill>(), (int)(projectile.damage * 1f), 0, projectile.owner, 0, projectile.whoAmI);
            }
        }
    }
}
