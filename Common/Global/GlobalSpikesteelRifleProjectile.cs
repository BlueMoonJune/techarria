using Microsoft.Xna.Framework;
using System;
using Techarria.Content.Dusts;
using Techarria.Content.Items.Armor;
using Techarria.Content.Items.Armor.Apparatus;
using Techarria.Content.Items.Weapons;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.NPC;

namespace Techarria.Common.Global
{
    public class GlobalSpikesteelRifleProjectile : GlobalProjectile
    {
        public bool isRifle = false;

        public override bool InstancePerEntity => true;

        public override bool AppliesToEntity(Projectile entity, bool lateInstantiation)
        {
            return entity.DamageType.CountsAsClass(DamageClass.Ranged);
        }

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if (source is EntitySource_ItemUse_WithAmmo s && s.Item.type == ModContent.ItemType<SpikesteelRifle>())
            {
                isRifle = true;
            }
        }

        public override void OnHitNPC(Projectile projectile, NPC target, HitInfo hit, int damageDone)
        {
            if (isRifle)
            {
                target.AddBuff(BuffID.Bleeding, 300);
            }
        }
    }
}
