using Terraria.ID;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;
using Terraria;

namespace Techarria.Content.Items.Placeables
{
    public class InfinityCrate : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Infinity Crate");

			Tooltip.SetDefault("'Check my recipe ;)'\n"
				+ "Can be infinetely inserted into or extracted from\n"
				+ "Any inserted items will be cleared of any reforges or similar");
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.InfinityCrate>());
			Item.value = 100;
			Item.maxStack = 99;
			Item.width = 32;
			Item.height = 32;

			Item.useTurn = true;
			Item.autoReuse = true;
			Item.consumable = true;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = 10;
			Item.useAnimation = 15;
		}
		public override void AddRecipes()
		{
			Recipe recipe = Recipe.Create(ModContent.ItemType<Placeables.InfinityCrate>());
			recipe.AddTile(TileID.Heart);
			recipe.AddTile(TileID.DemonAltar);
			recipe.AddCondition(Recipe.Condition.InSkyHeight);
			recipe.AddIngredient(ItemID.GoldWaterStrider, 89911);
			recipe.AddIngredient(ItemID.Fake_newchest1);
			recipe.Register();
		}
	}
}
