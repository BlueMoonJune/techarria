using Terraria.ID;
using Terraria.ModLoader;
using Terraria;

namespace Techarria.Content.Items.Placeables.Machines
{
    public class StirlingEngine : ModItem
    {
        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("Produces small amounts of power when the candle is lit\n" +
                "The candle periodically burns out\n" +
                "Activations relight the candle\n" +
                $"[i:{ModContent.ItemType<RecipeItems.Power>()}] generates " + Tiles.Machines.StirlingEngineTE.STIRLING_GENERATION * 60 + " Power per second\n" +
                $"[i:{ModContent.ItemType<RecipeItems.Activations>()}] accepts activations"); */
        }
        public override void SetDefaults()
        {
            Item.maxStack = 999;
            Item.consumable = true;
            Item.value = Item.buyPrice(0, 0, 1, 0);
            Item.mech = true;

            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.autoReuse = true;

            Item.createTile = ModContent.TileType<Tiles.Machines.StirlingEngine>();
        }
        public override void AddRecipes()
        {
            Recipe recipe = Recipe.Create(ModContent.ItemType<StirlingEngine>());
            recipe.AddTile(TileID.HeavyWorkBench);
            recipe.AddRecipeGroup(nameof(ItemID.Candle));
            recipe.AddRecipeGroup(RecipeGroupID.IronBar, 14);
            recipe.AddIngredient(ItemID.WaterBucket);
            recipe.Register();
        }
    }
}
