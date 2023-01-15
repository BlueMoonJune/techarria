using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Microsoft.Xna.Framework;
using Techarria.Content.Items.RecipeItems;

namespace Techarria.Content.Items.Materials.Molten
{
    internal class MoltenSpikeSteel : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 22;
            Item.maxStack = 999;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup(RecipeGroupID.IronBar)
                .AddIngredient(ItemID.Spike, 5)
                .AddIngredient(ModContent.ItemType<Volts>(), 600)
                .Register();

            CreateRecipe()
                .AddIngredient(ItemID.IronOre, 6)
                .AddIngredient(ItemID.Spike, 10)
                .AddIngredient(ModContent.ItemType<Temperature>(), 1500)
                .Register();

            CreateRecipe()
                .AddIngredient(ItemID.LeadOre, 6)
                .AddIngredient(ItemID.Spike, 10)
                .AddIngredient(ModContent.ItemType<Temperature>(), 1500)
                .Register();
        }
    }
}
