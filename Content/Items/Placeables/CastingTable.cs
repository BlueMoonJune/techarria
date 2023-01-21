using Terraria.ID;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;

namespace Techarria.Content.Items.Placeables
{
	public class CastingTable : ModItem
	{
		public override void SetStaticDefaults()
		{
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.CastingTable>());
			Item.value = 150;
			Item.maxStack = 99;
			Item.width = 32;
			Item.height = 16;
		}
	}
}