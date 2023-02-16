using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent.Creative;

namespace Techarria.Content.Items.Placeables.Machines.Logic
{
	/// <summary>
	/// Item form of Junction
	/// </summary>
	public class StickyPiston : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 5;

            Tooltip.SetDefault($"[i:{ModContent.ItemType<RecipeItems.Activations>()}] accepts Activations\n" +
                "'From a mod to a game to a mod'");
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

            Item.createTile = ModContent.TileType<Tiles.Machines.Logic.StickyPiston>();
        }
        public override void AddRecipes()
        {
            Recipe recipe = Recipe.Create(ModContent.ItemType<Logic.StickyPiston>());
            recipe.AddTile(TileID.WorkBenches);
            recipe.AddRecipeGroup(RecipeGroupID.IronBar, 5);
            recipe.AddIngredient(ItemID.Wood, 2);
            recipe.AddIngredient(ItemID.Wire, 5);
            recipe.Register();

            Recipe recipe2 = Recipe.Create(ModContent.ItemType<Logic.StickyPiston>());
            recipe2.AddTile(TileID.WorkBenches);
            recipe2.AddIngredient(ItemID.Gel, 5);
            recipe2.AddIngredient<Logic.Piston>();
            recipe2.Register();
        }
    }
}
