using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Techarria.Content.Items.Placeables.Machines
{
	public class Electromagnet : ModItem
	{
		public override void SetStaticDefaults() {
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
			/* Tooltip.SetDefault("Pulls items in when powered\n" +
				$"[i:{ModContent.ItemType<RecipeItems.Power>()}] accepts Power"); */
		}

		public override void SetDefaults() {
			Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Machines.Electromagnet>());
			Item.value = 150;
			Item.maxStack = 99;
			Item.width = 48;
			Item.height = 48;
			Item.mech = true;
		}
		public override void AddRecipes()
		{
			Recipe recipe = Recipe.Create(ModContent.ItemType<Machines.BlastFurnace>());
			recipe.AddTile(TileID.HeavyWorkBench);
			recipe.AddIngredient(ItemID.Hellforge);
			recipe.AddRecipeGroup(RecipeGroupID.IronBar, 3);
			recipe.AddIngredient(ItemID.RedBrick, 10);
			recipe.Register();
		}
	}
}