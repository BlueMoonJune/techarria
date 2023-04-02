using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent.Creative;

namespace Techarria.Content.Items.Placeables.Machines.Logic
{
	/// <summary>
	/// Item form of Junction
	/// </summary>
	public class Piston : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 5;

            /* Tooltip.SetDefault($"[i:{ModContent.ItemType<RecipeItems.Activations>()}] accepts Activations\n" +
                "'From a mod to a game to a mod'"); */
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

            Item.createTile = ModContent.TileType<Tiles.Machines.Logic.Piston>();
        }

        public override void AddRecipes()
        {
            Recipe recipe = Recipe.Create(ModContent.ItemType<Logic.Piston>());
            recipe.AddTile(TileID.WorkBenches);
            recipe.AddRecipeGroup(RecipeGroupID.IronBar, 5);
            recipe.AddIngredient(ItemID.Wood, 2);
            recipe.AddIngredient(ItemID.Wire, 5);
            recipe.Register();
        }
    }
}
