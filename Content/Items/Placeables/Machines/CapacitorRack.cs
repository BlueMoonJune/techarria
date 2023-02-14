using Terraria.GameContent.Creative;
using Terraria.ModLoader;

namespace Techarria.Content.Items.Placeables.Machines
{
	public class CapacitorRack : ModItem
	{
		public override void SetStaticDefaults() {
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;

			Tooltip.SetDefault("Right click to place Capacitors on the top of the rack\n" +
                "Diverts power from the bottom right and left of the rack to placed Capacitors\n" +
                "When the bottom center of the rack is activated, all Capacitors will release all their power to connected wires");
		}

		public override void SetDefaults() {
			Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Machines.CapacitorRack>());
			Item.value = 150;
			Item.maxStack = 99;
			Item.width = 48;
			Item.height = 32;
		}
	}
}