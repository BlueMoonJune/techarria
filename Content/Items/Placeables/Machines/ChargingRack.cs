using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Techarria.Content.Items.Placeables.Machines
{
	public class ChargingRack : ModItem
	{
		public override void SetStaticDefaults() {
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;

			/* Tooltip.SetDefault("Right click to place Capacitors on the top of the rack\n" +
                "Diverts power from the bottom right and left of the rack to placed Capacitors\n" +
                "When the bottom center of the rack is activated, all Capacitors will release all their power to connected wires\n" +
				$"[i:{ ModContent.ItemType<RecipeItems.Power>()}] stores Power\n" +
				$"[i:{ModContent.ItemType<RecipeItems.Activations>()}] accepts activations"); */
		}

		public override void SetDefaults() {
			Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Machines.ChargingRack>());
			Item.value = 150;
			Item.maxStack = 99;
			Item.width = 26;
			Item.height = 18;
			Item.mech = true;
		}

		public override void AddRecipes()
		{
			Recipe recipe = Recipe.Create(ModContent.ItemType<Machines.ChargingRack>());
			recipe.AddTile(TileID.HeavyWorkBench);
			recipe.AddIngredient<Materials.SpikeSteelSheet>(5);
			recipe.AddIngredient(ItemID.Wood, 10);
			recipe.AddIngredient(ItemID.Wire, 15);
			recipe.Register();
		}
	}
}