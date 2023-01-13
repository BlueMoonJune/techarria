using Terraria.GameContent.Creative;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;

namespace Techarria.Content.Items.Armor.Apparatus
{
    [AutoloadEquip(EquipType.Head)]
    internal class RadiatorApparatus : PowerArmor
    {

        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("'1000° night's edge challenge'");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            string color = "[c/7F7F7F:";
            if (charge > 0)
            {
                color = "[c/BFDFFF:";
            }
            tooltips.Add(new TooltipLine(Mod, "ChargeBonuses", color + "12% increased melee damage]\n" + color + "4% increased melee speed]\n" + color + "5% increased critical strike chance]\n" + color + "Slowly consumes charge while in combat]"));
            Player player = Main.player[Main.myPlayer];
            if (IsArmorSet(player.armor[0], player.armor[1], player.armor[2]))
            {
                tooltips.Add(new TooltipLine(Mod, "SetBonus", "Set Bonus:\n" + color + "Uses some charge to superheat melee weapons, setting hit enemies on fire]"));
            }
            base.ModifyTooltips(tooltips);
        }

        public override void SetDefaults()
        {
            damageClass = 1;
            maxcharge = ApparatusMaxCharge;
            Item.width = 24; // Width of the item
            Item.height = 22; // Height of the item
            Item.value = Item.sellPrice(gold: 1); // How many coins the item is worth
            Item.rare = ItemRarityID.Green; // The rarity of the item
            Item.defense = 9; // The amount of defense the item will give when equipped
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.UseSound = SoundID.Item1;
        }

        public override int Charge(int amount)
        {
            return base.Charge(amount);
        }

        public override int Deplete(int amount)
        {
            return base.Deplete(amount);

        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            if (!(head.ModItem is RadiatorApparatus h) || h.charge <= 0)
            {
                return false;
            }
            if (!(body.ModItem is BreastplateApparatus b) || b.charge <= 0)
            {
                return false;
            }
            if (!(legs.ModItem is LeggingsApparatus l) || l.charge <= 0)
            {
                return false;
            }
            return true;
        }

        public override void UpdateEquip(Player player)
        {
            if (charge > 0)
            {
                player.GetDamage(DamageClass.Melee) += 0.12f;
                player.GetAttackSpeed(DamageClass.Melee) += 0.04f;
                player.GetCritChance(DamageClass.Melee) += 0.05f;
            }
        }
    }
}
