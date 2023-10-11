using Terraria.ID;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;
using Terraria;


namespace Techarria.Content.Items.Placeables.Machines
{
    public class SoulCondenser : ModItem
    {
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Cryo Chamber");
			/* Tooltip.SetDefault("insert an enemy banner and provide power to generate enemy loot\n" +
				$"[i:{ModContent.ItemType<RecipeItems.Power>()}] accepts Power"); */

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Machines.SoulCondenser>());
			Item.value = 150;
			Item.maxStack = 99;
			Item.width = 28;
			Item.height = 20;
			Item.mech = true;
		}
		public override void AddRecipes()
		{
			Recipe recipe = Recipe.Create(ModContent.ItemType<Placeables.Machines.SoulCondenser>());
		}
	}
}
