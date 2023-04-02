using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Techarria.Content.Items.Placeables.Machines
{
	public class GreenhouseController : ModItem
	{
		public override void SetStaticDefaults() {
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
			/* Tooltip.SetDefault("Used for simple alloy crafting\n" +
                "Provide power anywhere to heat up the Blast Furnace\n" +
                "Input items at the top\n" +
                "Items can be extracted using transfer ducts at the bottom\n" +
				$"[i:{ModContent.ItemType<RecipeItems.Temperature>()}] prefers higher Temperature\n" +
				$"[i:{ModContent.ItemType<RecipeItems.Power>()}] accepts Power"); */
		}

		public override void SetDefaults() {
			Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Machines.GreenhouseController>());
			Item.value = 150;
			Item.maxStack = 99;
			Item.width = 28;
			Item.height = 20;
			Item.mech = true;
		}
		public override void AddRecipes()
		{
			Recipe recipe = Recipe.Create(ModContent.ItemType<Machines.GreenhouseController>());
			recipe.AddTile(TileID.HeavyWorkBench);
			recipe.AddIngredient(ItemID.Hellforge);
			recipe.AddRecipeGroup(RecipeGroupID.IronBar, 3);
			recipe.AddIngredient(ItemID.RedBrick, 10);
			recipe.Register();
		}
	}
}