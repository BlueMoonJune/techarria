using Terraria.GameContent.Creative;
using Terraria.ModLoader;

namespace Techarria.Content.Items.Placeables.Machines
{
	public class GelatinousTurbine : ModItem
	{
		public override void SetStaticDefaults() {
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;

			Tooltip.SetDefault("Burns gel to produce power\n" +
				$"[i:{ ModContent.ItemType<RecipeItems.Power>()}]" + "60 power per second");
		}

		public override void SetDefaults() {
			Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Machines.GelatinousTurbine>());
			Item.value = 150;
			Item.maxStack = 99;
			Item.width = 28;
			Item.height = 20;
		}
	}
}