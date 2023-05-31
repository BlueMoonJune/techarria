using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent.Creative;

namespace Techarria.Content.Items.Placeables.Machines.Logic
{
	public class ItemDropper : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 5;

            /* Tooltip.SetDefault("Right click to insert an item\n" +
                "Activate to place items\n" +
                $"[i:{ModContent.ItemType<RecipeItems.Activations>()}] accepts Activations"); */
        }

        public override void SetDefaults()
        {
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

            Item.createTile = ModContent.TileType<Tiles.Machines.Logic.ItemDropper>();
        }

        public override void AddRecipes()
        {
            Recipe recipe = Recipe.Create(ModContent.ItemType<Logic.ItemDropper>());
            recipe.AddTile(TileID.WorkBenches);
            recipe.AddIngredient(ItemID.Wire, 10);
            recipe.AddIngredient(ItemID.StoneBlock, 25);
            recipe.Register();
        }
    }
}
