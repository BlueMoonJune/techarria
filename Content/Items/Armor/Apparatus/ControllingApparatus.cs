using Terraria.GameContent.Creative;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;
using Techarria.Content.Projectiles.Minions;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

namespace Techarria.Content.Items.Armor.Apparatus
{
    [AutoloadEquip(EquipType.Head)]
    public class ControllingApparatus : PowerArmor
    {

        public override void SetStaticDefaults()
        {
            //Tooltip.SetDefault("'1000° Night's Edge Challenge'");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            string color = "[c/7F7F7F:";
            if (charge > 0)
            {
                color = "[c/BFDFFF:";
            }
            tooltips.Add(new TooltipLine(Mod, "ChargeBonuses", color + "9% increased summon damage]\n" + color + "5% increased whip speed]\n" + color + "Increases your max number of minions by 2]\n" + color + "Slowly consumes charge while in combat]"));
            Player player = Main.player[Main.myPlayer];
            if (IsArmorSet(player.armor[0], player.armor[1], player.armor[2]))
            {
                tooltips.Add(new TooltipLine(Mod, "SetBonus", "Set Bonus: " + color + "Summons a drone minion to attack enemies. The drone drains charge when attacking]"));
            }
            base.ModifyTooltips(tooltips);
        }

        public override void SetDefaults()
        {
            damageClass = 4;
            maxcharge = ApparatusMaxCharge;
            Item.width = 20; // Width of the item
            Item.height = 26; // Height of the item
            Item.value = Item.sellPrice(gold: 1); // How many coins the item is worth
            Item.rare = ItemRarityID.Green; // The rarity of the item
            Item.defense = 4; // The amount of defense the item will give when equipped
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
            if (!(head.ModItem is ControllingApparatus h) || h.charge <= 0)
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

		public override void UpdateArmorSet(Player player) {
			player.AddBuff(ModContent.BuffType<DroneApparatusBuff>(), 2);
			if (player.ownedProjectileCounts[ModContent.ProjectileType<DroneApparatus>()] <= 0) {
				Projectile proj = Projectile.NewProjectileDirect(new EntitySource_Buff(player, Type, 0), player.Center, Vector2.Zero, ModContent.ProjectileType<DroneApparatus>(), 0, 0);
				proj.owner = player.whoAmI;
			}
		}

		public override void UpdateEquip(Player player) {

			if (charge > 0)
            {
                player.GetDamage(DamageClass.Summon) += 0.09f;
                player.GetAttackSpeed(DamageClass.SummonMeleeSpeed) += 0.05f;
				player.maxMinions += 2;
            }
        }
        public override void AddRecipes()
        {
            Recipe recipe = Recipe.Create(ModContent.ItemType<Items.Armor.Apparatus.ControllingApparatus>());
            recipe.AddTile(TileID.HeavyWorkBench);
            recipe.AddIngredient<Materials.SpikeSteelSheet>(12);
            recipe.AddIngredient(ItemID.Wire, 5);
            recipe.Register();
        }
    }
}
