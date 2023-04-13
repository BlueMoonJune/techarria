using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Techarria.Content.Items.RecipeItems;
using Techarria.Content.Tiles.Machines;

namespace Techarria.Content.Items.Materials.Molten
{
	public class BlobOfLava : MoltenBlob
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
            // Tooltip.SetDefault("'I would recommend putting this down'");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 25;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.StoneBlock, 1)
                .AddIngredient(ModContent.ItemType<Temperature>(), 1000)
				.AddTile<BlastFurnace>()
				.Register();
        }
    }
}
