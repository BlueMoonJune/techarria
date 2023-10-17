using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent.Creative;

namespace Techarria.Content.Items.Placeables.Transfer
{
	/// <summary>
	/// Item form of TransferDuct
	/// </summary>
	public class TransferDuct : ModItem
    {
        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("Can insert or extract from an adjcacent container when activated\n" +
                "When there is no adjacent containers and the duct is activated it will toggle its ability to transfer items\n" +
               "When there is no destination for transfered items the duct will do nothing\n" +
               "2 ducts painted differently do not connect\n" +
                $"[i:{ModContent.ItemType<RecipeItems.Activations>()}] accepts Activations"); */

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 5;
        }

        public override void SetDefaults()
        {
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

            Item.createTile = ModContent.TileType<Tiles.Transfer.TransferDuct>();
        }
        public override void AddRecipes()
        {
            Recipe recipe = Recipe.Create(ModContent.ItemType<Transfer.TransferDuct>(), 5);
            recipe.AddTile(TileID.SteampunkBoiler);
            recipe.AddRecipeGroup(RecipeGroupID.IronBar, 5);
            recipe.AddIngredient(ItemID.SoulofFlight);
            recipe.AddIngredient(ItemID.Wire);
            recipe.Register();
        }
    }
}
