using Terraria.ID;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;
using Terraria;

namespace Techarria.Content.Items.Placeables
{
    public class JourneyCrate : ModItem
    {
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Journey Crate");

			/* Tooltip.SetDefault("Obtained from researching a Storage Crate\n"
				+ "Can be infinetely inserted into or extracted from so long as you have enough to research the item\n"
				+ "Any inserted items will be cleared of any reforges or similar"); */

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.JourneyCrate>());
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
	}
}
