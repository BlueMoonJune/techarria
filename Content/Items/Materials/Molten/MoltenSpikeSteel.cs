using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Techarria.Content.Items.RecipeItems;
using Techarria.Content.Tiles.Machines;

namespace Techarria.Content.Items.Materials.Molten
{
	public class MoltenSpikeSteel : MoltenBlob
    {

        public override void SetDefaults()
        {
            temp = 1500;
            Item.width = 24;
            Item.height = 18;
            Item.maxStack = 999;
        }

        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("'I would recommend putting this down'");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 25;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup(RecipeGroupID.IronBar)
                .AddIngredient(ItemID.Spike, 5)
                .AddIngredient(ModContent.ItemType<Volts>(), 600)
                .Register();

            CreateRecipe()
                .AddRecipeGroup(nameof(ItemID.IronOre), 6)
                .AddIngredient(ItemID.Spike, 10)
                .AddIngredient(ModContent.ItemType<Temperature>(), 1500)
                .AddTile<BlastFurnace>()
                .Register();
        }
    }
}
