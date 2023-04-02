using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent.Creative;

namespace Techarria.Content.Items.Placeables.Transfer
{
	/// <summary>
	/// Item form of TransferDetector
	/// </summary>
	public class TransferDetector : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 5;

            /* Tooltip.SetDefault("Will activate wires when an item is sent through it\n" +
            $"[i:{ModContent.ItemType<RecipeItems.Activations>()}] accepts and causes Activations\n" +
            "'Can't recursively power.. trust me you don't want that'"); */
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

            Item.createTile = ModContent.TileType<Tiles.Transfer.TransferDetector>();
        }
        public override void AddRecipes()
        {
            Recipe recipe = Recipe.Create(ModContent.ItemType<Transfer.TransferDetector>());
            recipe.AddTile(TileID.Anvils);
            recipe.AddIngredient<Transfer.TransferDuct>();
            recipe.AddIngredient(ItemID.Cog, 5);
            recipe.AddIngredient(ItemID.Wire);
            recipe.Register();
        }
    }
}
