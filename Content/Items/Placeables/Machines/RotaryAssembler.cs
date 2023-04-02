using Terraria.GameContent.Creative;
using Terraria.ModLoader;

namespace Techarria.Content.Items.Placeables.Machines
{
	public class RotaryAssembler : ModItem
	{
		public override void SetStaticDefaults() {
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;

			/* Tooltip.SetDefault("Items inserted depending on direction they are inserted from\n" +
				"When activated, rotates 45° in a direction correspondant to the direction the wires flow\n" +
				$"[i:{ModContent.ItemType<RecipeItems.Activations>()}] accepts Activations\n" +
                "'I reccomend you play with it to figure out the directional movement direction'"); */
		}

		public override void SetDefaults() {
			Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Machines.RotaryAssembler>());
			Item.value = 150;
			Item.maxStack = 99;
			Item.width = 48;
			Item.height = 48;
			Item.mech = true;
		}
	}
}