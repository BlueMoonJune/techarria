using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace Techarria.Content.Items.Placeables
{
    public class GreenhouseGlass : ModItem
	{
		public override void SetStaticDefaults() {

			// journey mode
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;

		}
		public override void SetDefaults() {
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTurn = true;
			Item.useAnimation = 15;
			Item.useTime = 10;
			Item.autoReuse = true;
			Item.maxStack = 999;
			Item.consumable = true;
			Item.createTile = ModContent.TileType<Tiles.Misc.GreenhouseGlass>();
			Item.width = 16;
			Item.height = 16;
        }

        public override void AddRecipes()
        {
            Recipe recipe = Recipe.Create(ModContent.ItemType<GreenhouseGlass>(), 20);
            recipe.AddTile(TileID.GlassKiln);
            recipe.AddIngredient(ItemID.Glass, 20);
            recipe.AddRecipeGroup(RecipeGroupID.IronBar);
            recipe.Register();

            recipe = Recipe.Create(ModContent.ItemType<GreenhouseGlass>(), 20);
            recipe.AddTile(TileID.GlassKiln);
            recipe.AddIngredient<GreenhouseAccentGlass>();
            recipe.Register();
        }
    }

	public class GreenhouseAccentGlass : GreenhouseGlass
    {
		public override void SetDefaults()
		{
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTurn = true;
			Item.useAnimation = 15;
			Item.useTime = 10;
			Item.autoReuse = true;
			Item.maxStack = 999;
			Item.consumable = true;
			Item.createTile = ModContent.TileType<Tiles.Misc.GreenhouseAccentGlass>();
			Item.width = 16;
			Item.height = 16;
        }

        public override void AddRecipes()
        {
            Recipe recipe = Recipe.Create(ModContent.ItemType<GreenhouseAccentGlass>(), 20);
            recipe.AddTile(TileID.GlassKiln);
            recipe.AddIngredient<GreenhouseGlass>();
            recipe.Register();
        }

    }
}
