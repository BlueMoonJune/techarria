using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent.Creative;

namespace Techarria.Content.Items.Placeables.Machines
{
	public class CableConnector : ModItem
	{
		public override void SetStaticDefaults() {
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 5;

			Tooltip.SetDefault("Provides a way to freely connect wires long distance\n" +
                "Right click with wires to extend the max length\n" +
                "Right click with a free hand to connect two Cable Connectors\n" +
                "'Also try Immersive Engineering!'");
		}

		public override void SetDefaults() {
			Item.width = 16;
			Item.height = 16;
			Item.maxStack = 999;
			Item.consumable = true;
			Item.value = Item.buyPrice(0, 0, 1, 0);
			Item.mech = true;

			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTurn = true;
			Item.useAnimation = 15;
			Item.useTime = 10;
			Item.autoReuse = true;

			Item.createTile = ModContent.TileType<Tiles.Machines.CableConnector>();
		}
	}
}
