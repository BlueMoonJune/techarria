using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace Techarria.Content.Items.Placeables.Natural
{
	class CoalOre : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Industrial Coal Stone Block");

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 200;
			ItemID.Sets.SortingPriorityMaterials[Item.type] = 58;
		}

		public override void SetDefaults()
		{
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTurn = true;
			Item.useAnimation = 15;
			Item.useTime = 10;
			Item.autoReuse = true;
			Item.maxStack = 999;
			Item.consumable = true;
			Item.createTile = ModContent.TileType<Tiles.Natural.CoalOre>();
			Item.width = 22;
			Item.height = 20;
			Item.value = Item.buyPrice(silver: 1, copper: 25);

		}
		public override void AddRecipes()
		{
			Recipe recipe = Recipe.Create(ModContent.ItemType<Placeables.Natural.CoalOre>());
			recipe.AddTile(TileID.HeavyWorkBench);
			recipe.AddCondition(Recipe.Condition.InGraveyardBiome);
			recipe.AddIngredient(ItemID.StoneBlock);
			recipe.AddIngredient<Materials.Sprocket>();
			recipe.Register();
		}
	}
}
