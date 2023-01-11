using Terraria.ID;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;

namespace Techarria.Content.Items.Placeables
{
	public class ExampleTable : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Example Table");
			Tooltip.SetDefault("This is a modded table.");

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.PlayerInterface>());
			Item.value = 150;
			Item.maxStack = 99;
			Item.width = 38;
			Item.height = 24;
		}
	}
}