using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Techarria.Content.Dusts;
using Techarria.Content.Items.Armor;
using Techarria.Content.Items.Armor.Apparatus;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Techarria.Common.Global
{
    public class GlobalRangedProjectile : GlobalProjectile
    {
        public override bool AppliesToEntity(Projectile entity, bool lateInstantiation)
        {
            return entity.DamageType.CountsAsClass(DamageClass.Ranged);
        }

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if (source is EntitySource_ItemUse_WithAmmo s && s.Entity is Player p)
            {
                PowerArmorPlayer player = p.GetModPlayer<PowerArmorPlayer>();
                if (player.visorCooldown <= 0 && p.armor[0].ModItem is VisorApparatus visor && visor.IsArmorSet(p.armor[0], p.armor[1], p.armor[2]))
                {
                    player.visorCooldown = 75;
                    for (int i = 0; i < 30; i++)
                        Dust.NewDust(p.TopLeft, p.width, p.height, DustID.Vortex);
                    Vector2 mouse = Main.MouseWorld;
                    Vector2 end = mouse - p.Center;
                    end.Normalize();
                    end *= 1024;
                    end += p.Center;
                    LineSegment line = new LineSegment(p.Center, end);
                    for (int i = 0; i < 128; i++)
                    {
                        Vector2 point = (line.End - line.Start) * (i / 128f) + line.Start;
                        Dust.NewDust(point, 0, 0, ModContent.DustType<TransferDust>());
                    }
                    foreach (NPC npc in Main.npc)
                    {
                        if (!npc.friendly && Techarria.Intersects(npc.getRect(), line))
                        {
                            npc.StrikeNPC((int)(p.GetDamage(DamageClass.Ranged).ApplyTo(7)), 0, 0);
                            npc.AddBuff(BuffID.Electrified, 600);
                            if (new Random().Next(0) == 0)
                                Projectile.NewProjectile(source, npc.Center, Vector2.Zero, ModContent.ProjectileType<Content.Projectiles.Lightning>(), 2, 100000);
                            for (int i = 0; i < 30; i++)
                                Dust.NewDust(npc.Center, 0, 0, DustID.Vortex);

                            for (int i = 0; i < 3; i++)
                            {
                                Item item = p.armor[i];
                                if (item.ModItem is PowerArmor armor && 1 != armor.Deplete(1))
                                {
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
