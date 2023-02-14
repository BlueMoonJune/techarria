using Terraria.GameContent.Creative;
using Terraria.ModLoader;

namespace Techarria.Content.Items.Placeables.Machines
{
	public class PlayerInterface : ModItem
	{
		public override void SetStaticDefaults() {
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;

			Tooltip.SetDefault("Accesses the player standing in front of it\n" +
                "Can act as either input or output for ducts\n" +
                "Will charge player items when powered");
		}

		public override void SetDefaults() {
			Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Machines.PlayerInterface>());
			Item.value = 150;
			Item.maxStack = 99;
			Item.width = 48;
			Item.height = 48;
		}
	}
}