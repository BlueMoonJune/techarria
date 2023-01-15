using Terraria.GameContent.Creative;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;

namespace Techarria.Content.Items.Armor.Apparatus
{
    [AutoloadEquip(EquipType.Head)]
    public class TechnomancyApparatus : PowerArmor
    {

        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("'1000° Night's Edge Challenge'");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            string color = "[c/7F7F7F:";
            if (charge > 0)
            {
                color = "[c/BFDFFF:";
            }
            tooltips.Add(new TooltipLine(Mod, "ChargeBonuses", color + "6% increased magic damage]\n" + color + "+40 mana]\n" + color + "5% increased critical strike chance]\n" + color + "Slowly consumes charge while in combat]"));
            Player player = Main.player[Main.myPlayer];
            if (IsArmorSet(player.armor[0], player.armor[1], player.armor[2]))
            {
                tooltips.Add(new TooltipLine(Mod, "SetBonus", "Set bonus:" + color + "Has a chance to create a burst of shrapnel when a magic projectile hits an enemy, using some charge]"));
            }
            base.ModifyTooltips(tooltips);
        }

        public override void SetDefaults()
        {
            damageClass = 3;
            maxcharge = ApparatusMaxCharge;
            Item.width = 22; // Width of the item
            Item.height = 20; // Height of the item
            Item.value = Item.sellPrice(gold: 1); // How many coins the item is worth
            Item.rare = ItemRarityID.Green; // The rarity of the item
            Item.defense = 5; // The amount of defense the item will give when equipped
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
                player.GetDamage(DamageClass.Magic) += 0.06f;
                player.statManaMax2 += 40;
                player.manaCost -= 0.05f;
            }

        }
    }
}
