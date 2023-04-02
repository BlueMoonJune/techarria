using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent.Creative;

namespace Techarria.Content.Items.Placeables.Transfer.Drones
{
	/// <summary>
	/// Item form of DirectionalDuct
	/// </summary>
	public class DroneNode : ModItem
	{
		public override void SetStaticDefaults() {
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 5;

			/* Tooltip.SetDefault("Used to guide Transfer Drones\nCreates activations when a drone arrives\n" +
				$"[i:{ModContent.ItemType<RecipeItems.Activations>()}] causes Activations"); */
		}

		public override void SetDefaults() {
			Item.width = 32;
			Item.height = 32;
			Item.maxStack = 999;
			Item.consumable = true;
			Item.value = Item.buyPrice(0, 0, 1, 0);
			Item.mech = true;

			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTurn = true;
			Item.useAnimation = 15;
			Item.useTime = 10;
			Item.autoReuse = true;

			Item.createTile = ModContent.TileType<Tiles.Transfer.Drone.DroneNode>();
		}
		public override void AddRecipes() {
			var recipe = Recipe.Create(ModContent.ItemType<DirectionalDuct>());
			recipe.AddTile(TileID.Anvils);
			recipe.AddIngredient<TransferDuct>();
			recipe.AddIngredient(ItemID.WoodenArrow);
			recipe.Register();
		}
	}
}
