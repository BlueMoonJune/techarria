using Terraria.GameContent.Creative;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;
using Techarria.Content.Projectiles.Minions;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;

namespace Techarria.Content.Items.Armor.Apparatus
{
    [AutoloadEquip(EquipType.Legs)]
    internal class LeggingsApparatus : PowerArmor
    {

        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            string color = "[c/7F7F7F:";
            if (charge > 0)
            {
                color = "[c/BFDFFF:";
            }
            tooltips.Add(new TooltipLine(Mod, "ChargeBonuses", color + "Allows the wearer to double jump, consuming energy on use]"));
            Player player = Main.player[Main.myPlayer];
            if (player.armor[0] == null && player.armor[0].ModItem.IsArmorSet(player.armor[0], player.armor[1], player.armor[2]))
            {
                if (player.armor[0].ModItem is RadiatorApparatus)
                    tooltips.Add(new TooltipLine(Mod, "SetBonus", "Set bonus:" + color + "Uses some charge to superheat melee weapons, setting hit enemies on fire]"));
                if (player.armor[0].ModItem is VisorApparatus)
                    tooltips.Add(new TooltipLine(Mod, "SetBonus", "Set Bonus:" + color + "Has a chance to create a burst of lightning when using ranged weapons. Uses some charge]"));
                if (player.armor[0].ModItem is RadiatorApparatus)
                    tooltips.Add(new TooltipLine(Mod, "SetBonus", "Set bonus:" + color + "Has a chance to create a burst of shrapnel when a magic projectile hits an enemy, using some charge]"));
                if (player.armor[0].ModItem is VisorApparatus)
                    tooltips.Add(new TooltipLine(Mod, "SetBonus", "Set Bonus:" + color + "Summons a drone minion to attack enemies. The drone drains charge when attacking]"));

            }
            base.ModifyTooltips(tooltips);
        }

        public override void SetDefaults()
        {
            maxcharge = ApparatusMaxCharge;
            Item.width = 22; // Width of the item
            Item.height = 18; // Height of the item
            Item.value = Item.sellPrice(gold: 1); // How many coins the item is worth
            Item.rare = ItemRarityID.Green; // The rarity of the item
            Item.defense = 3; // The amount of defense the item will give when equipped
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.UseSound = SoundID.Item1;
            Item.buffType = ModContent.BuffType<DroneApparatusBuff>();
            // No buffTime because otherwise the item tooltip would say something like "1 minute duration"
            Item.shoot = ModContent.ProjectileType<DroneApparatus>(); // This item creates the minion projectile
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // This is needed so the buff that keeps your minion alive and allows you to despawn it properly applies
            player.AddBuff(Item.buffType, 2);

            // Minions have to be spawned manually, then have originalDamage assigned to the damage of the summon item
            var projectile = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, Main.myPlayer);
            projectile.originalDamage = Item.damage;

            // Since we spawned the projectile manually already, we do not need the game to spawn it for ourselves anymore, so return false
            return false;
        }

        public override int Charge(int amount)
        {
            return base.Charge(amount);
        }

        public override int Deplete(int amount)
        {
            return base.Deplete(amount);

        }

    }
}
