using Microsoft.Xna.Framework;
using Techarria.Content.Items.Armor.Apparatus;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Techarria.Common.Global
{
	public class GlobalMeleeWeapon : GlobalItem
    {
        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return entity.CountsAsClass(DamageClass.Melee);
        }

        public override void MeleeEffects(Item item, Player player, Rectangle hitbox)
        {
            base.MeleeEffects(item, player, hitbox);
            if (!(player.armor[0].ModItem is RadiatorApparatus helm && helm.IsArmorSet(player.armor[0], player.armor[1], player.armor[2])))
                return;
            if (Main.rand.NextBool(3))
            {
                Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, DustID.FlameBurst);
            }
        }
    }
}
