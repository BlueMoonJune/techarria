using Terraria.GameContent.Creative;
using Terraria.ModLoader;

namespace Techarria.Content.Items.Placeables.Machines
{
	public class BlastFurnace : ModItem
	{
		public override void SetStaticDefaults() {
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
			Tooltip.SetDefault("Used for simple alloy crafting\n" +
                "Provide power anywhere to heat up the Blast Furnace\n" +
                "Input items at the top\n" +
                "Items can be extracted using transfer ducts at the bottom");
		}

		public override void SetDefaults() {
			Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Machines.BlastFurnace>());
			Item.value = 150;
			Item.maxStack = 99;
			Item.width = 28;
			Item.height = 20;
		}
	}
}