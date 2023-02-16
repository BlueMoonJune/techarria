using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Techarria.Content.Items.Placeables.Machines
{
	public class PlayerInterface : ModItem
	{
		public override void SetStaticDefaults() {
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;

			Tooltip.SetDefault("Accesses the player standing in front of it\n" +
                "Can act as either input or output for ducts\n" +
				"Will charge player items when powered\n" +
				$"[i:{ModContent.ItemType<RecipeItems.Power>()}] diverts Power");
		}

		public override void SetDefaults() {
			Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Machines.PlayerInterface>());
			Item.value = 150;
			Item.maxStack = 99;
			Item.width = 48;
			Item.height = 48;
		}
		public override void AddRecipes()
		{
			Recipe recipe = Recipe.Create(ModContent.ItemType<Machines.PlayerInterface>());
			recipe.AddTile(TileID.Anvils);
			recipe.AddRecipeGroup(RecipeGroupID.IronBar, 5);
			recipe.AddIngredient<Materials.SpikeSteelSheet>(10);
			recipe.AddIngredient(ItemID.Extractinator);
			recipe.Register();
		}
	}
}