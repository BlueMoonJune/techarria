using Terraria.GameContent.Creative;
using Terraria.ModLoader;

namespace Techarria.Content.Items.Placeables.Machines
{
	public class PlayerInterface : ModItem
	{
		public override void SetStaticDefaults() {
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults() {
			Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Machines.PlayerInterface>());
			Item.value = 150;
			Item.maxStack = 99;
			Item.width = 38;
			Item.height = 24;
		}
	}
}