using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace Techarria.Content.Items.Placeables.Machines
{
    public class CrystalGrowthChamber : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Cryo Chamber");
            /* Tooltip.SetDefault("insert an enemy banner and provide power to generate enemy loot\n" +
				$"[i:{ModContent.ItemType<RecipeItems.Power>()}] accepts Power"); */

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Machines.CrystalGrowthChamber>());
            Item.value = 150;
            Item.maxStack = 99;
            Item.width = 28;
            Item.height = 20;
            Item.mech = true;
        }
        public override void AddRecipes()
        {
            Recipe recipe = Recipe.Create(ModContent.ItemType<Placeables.Machines.CrystalGrowthChamber>());
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.AddIngredient(ItemID.IceMachine);
            recipe.AddIngredient(ItemID.FrostCore);
            recipe.Register();
        }
    }
}
