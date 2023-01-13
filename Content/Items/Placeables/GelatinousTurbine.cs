using Terraria.ID;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;

namespace Techarria.Content.Items.Placeables
{
	public class GelatinousTurbine : ModItem
	{
		public override void SetStaticDefaults()
		{
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.GelatinousTurbine>());
			Item.value = 150;
			Item.maxStack = 99;
			Item.width = 48;
			Item.height = 32;
		}
	}
}