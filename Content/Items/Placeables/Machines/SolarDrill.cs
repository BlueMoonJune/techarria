using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent.Creative;

namespace Techarria.Content.Items.Placeables.Machines
{
	/// <summary>
	/// Item form of BlockBreaker
	/// </summary>
	public class SolarDrill : ModItem
	{
		public override void SetStaticDefaults() {
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 5;

			/* Tooltip.SetDefault("Breaks the tile in front of it when activated\n" +
                $"[i:{ModContent.ItemType<RecipeItems.Activations>()}] accepts Activations"); */
		}

		public override void SetDefaults() {
			Item.width = 16;
			Item.height = 16;
			Item.maxStack = 999;
			Item.consumable = true;
			Item.value = Item.buyPrice(0, 0, 1, 0);
			Item.mech = true;

			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTurn = true;
			Item.useAnimation = 15;
			Item.useTime = 10;
			Item.autoReuse = true;

			Item.createTile = ModContent.TileType<Tiles.Machines.Logic.SolarDrill>();
		}
		public override void AddRecipes() {
			var recipe = Recipe.Create(ModContent.ItemType<Content.Items.Placeables.Machines.SolarDrill>());
			recipe.AddTile(TileID.WorkBenches);
			recipe.AddRecipeGroup(RecipeGroupID.IronBar, 5);
			recipe.AddIngredient(ItemID.Wire, 5);
			recipe.AddIngredient(ItemID.StoneBlock, 25);
			recipe.Register();
		}
	}
}
