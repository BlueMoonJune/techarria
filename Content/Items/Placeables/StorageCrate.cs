using Terraria.ID;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;
using Terraria;

namespace Techarria.Content.Items.Placeables
{
    public class StorageCrate : ModItem
    {
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Storage Crate");

			/* Tooltip.SetDefault("Stores up to 999999 matching items\n"
				+ "Any inserted items will be cleared of any reforges or similar\n" +
				"'Stores items with 99% accuracy'"); */

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Misc.StorageCrate>());
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
			Recipe recipe = Recipe.Create(ModContent.ItemType<Placeables.StorageCrate>());
			recipe.AddTile(TileID.WorkBenches);
			recipe.AddIngredient(ItemID.IronCrate);
			recipe.AddRecipeGroup(nameof(ItemID.Chest));
			recipe.Register();

			Recipe recipeHard = Recipe.Create(ModContent.ItemType<Placeables.StorageCrate>());
			recipeHard.AddTile(TileID.WorkBenches);
			recipeHard.AddIngredient(ItemID.IronCrateHard);
			recipeHard.AddRecipeGroup(nameof(ItemID.Chest));
			recipeHard.Register();

			Recipe recipeAlt = Recipe.Create(ModContent.ItemType<Placeables.StorageCrate>());
			recipeAlt.AddTile(TileID.WorkBenches);
			recipeAlt.AddRecipeGroup(nameof(ItemID.IronBar), 10);
			recipeAlt.AddIngredient(ItemID.SteampunkChest);
			recipeAlt.Register();


		}

		// journey mode stuff
		public override void OnResearched(bool fullyResearched)
		{
			if (fullyResearched)
			{
				CreativeUI.ResearchItem(ModContent.ItemType<JourneyCrate>());
				Main.NewText("Your research discovered a bonus item!:" + $"\n[i:{ ModContent.ItemType<JourneyCrate>()}]");
			}
		}
	}
}
