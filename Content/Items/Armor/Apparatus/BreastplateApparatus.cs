using Terraria.GameContent.Creative;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;

namespace Techarria.Content.Items.Armor.Apparatus
{
    [AutoloadEquip(EquipType.Body)]
    internal class BreastplateApparatus : PowerArmor
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
            tooltips.Add(new TooltipLine(Mod, "ChargeBonuses", color + "Absorbs some damage, using some charge when hit]\n" + color + "Reduces damage taken by 5%]"));
            Player player = Main.player[Main.myPlayer];
            if (player.armor[0] == null && player.armor[0].ModItem.IsArmorSet(player.armor[0], player.armor[1], player.armor[2]))
            {
                if (player.armor[0].ModItem is RadiatorApparatus)
                    tooltips.Add(new TooltipLine(Mod, "SetBonus", "Set bonus:\n" + color + "Uses some charge to superheat melee weapons, setting hit enemies on fire]"));

            }
            base.ModifyTooltips(tooltips);
        }

        public override void SetDefaults()
        {
            maxcharge = ApparatusMaxCharge;
            Item.width = 26; // Width of the item
            Item.height = 22; // Height of the item
            Item.value = Item.sellPrice(gold: 1); // How many coins the item is worth
            Item.rare = ItemRarityID.Green; // The rarity of the item
            Item.defense = 4; // The amount of defense the item will give when equipped
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.UseSound = SoundID.Item1;
        }
    }
}
