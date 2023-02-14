using Terraria.GameContent.Creative;
using Terraria.ModLoader;

namespace Techarria.Content.Items.Placeables.Machines
{
	public class CastingTable : ModItem
	{
		public override void SetStaticDefaults() {
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;

			Tooltip.SetDefault("Requires a mold for special crafting\n" +
				"Accepts transfer duct input from any side, a solid lid may be handy to avoid spillage\n" +
                "Once the molten metal is cooled to the required temperature, the resulting item may be extracted from the top\n" +
                "Can not be extracted from with solid tiles covering the top");
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