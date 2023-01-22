using Terraria.GameContent.Creative;
using Terraria.ModLoader;

namespace Techarria.Content.Items.Placeables.Machines
{
	public class CastingTable : ModItem
	{
		public override void SetStaticDefaults() {
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults() {
			Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Machines.CastingTable>());
			Item.value = 150;
			Item.maxStack = 99;
			Item.width = 32;
			Item.height = 16;
		}
	}
}