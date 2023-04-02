using Terraria.ModLoader;
using Terraria;
using Terraria.ID;

namespace Techarria.Content.Items.Placeables.Timers
{
    public abstract class TimerItem : ModItem
    {
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
        }
    }
	public class FiveSecondTimer : TimerItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("5 Second Timer");
            /* Tooltip.SetDefault("A timer which doesn't need supporting tiles\n" +
                "Doesn't need to be re-activated on world startup\n" +
                $"[i:{ModContent.ItemType<RecipeItems.Activations>()}] causes Activations"); */
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.createTile = ModContent.TileType<Tiles.Machines.Logic.Timers.FiveSecondTimer>();
        }

        public override void AddRecipes()
        {
            Recipe recipe5 = Recipe.Create(ModContent.ItemType<Timers.FiveSecondTimer>());
            recipe5.AddTile(TileID.HeavyWorkBench);
            recipe5.AddRecipeGroup(RecipeGroupID.IronBar, 3);
            recipe5.AddIngredient(ItemID.Timer5Second);
            recipe5.Register();
        }
    }

    public class ThreeSecondTimer : TimerItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("3 Second Timer");
            /* Tooltip.SetDefault("A timer which doesn't need supporting tiles\n" +
                "Doesn't need to be re-activated on world startup\n" +
                $"[i:{ModContent.ItemType<RecipeItems.Activations>()}] causes Activations"); */
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.createTile = ModContent.TileType<Tiles.Machines.Logic.Timers.ThreeSecondTimer>();
        }
        public override void AddRecipes()
        {
            Recipe recipe3 = Recipe.Create(ModContent.ItemType<Timers.ThreeSecondTimer>());
            recipe3.AddTile(TileID.HeavyWorkBench);
            recipe3.AddRecipeGroup(RecipeGroupID.IronBar, 3);
            recipe3.AddIngredient(ItemID.Timer3Second);
            recipe3.Register();
        }
    }

    public class OneSecondTimer : TimerItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("1 Second Timer");
            /* Tooltip.SetDefault("A timer which doesn't need supporting tiles\n" +
                "Doesn't need to be re-activated on world startup\n" +
                $"[i:{ModContent.ItemType<RecipeItems.Activations>()}] causes Activations"); */
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.createTile = ModContent.TileType<Tiles.Machines.Logic.Timers.OneSecondTimer>();
        }
        public override void AddRecipes()
        {
            Recipe recipe1 = Recipe.Create(ModContent.ItemType<Timers.OneSecondTimer>());
            recipe1.AddTile(TileID.HeavyWorkBench);
            recipe1.AddRecipeGroup(RecipeGroupID.IronBar, 3);
            recipe1.AddIngredient(ItemID.Timer1Second);
            recipe1.Register();
        }
    }

    public class HalfSecondTimer : TimerItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("1/2 Second Timer");
            /* Tooltip.SetDefault("A timer which doesn't need supporting tiles\n" +
                "Doesn't need to be re-activated on world startup\n" +
                $"[i:{ModContent.ItemType<RecipeItems.Activations>()}] causes Activations"); */
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.createTile = ModContent.TileType<Tiles.Machines.Logic.Timers.HalfSecondTimer>();
        }
        public override void AddRecipes()
        {
            Recipe recipeHalf = Recipe.Create(ModContent.ItemType<Timers.HalfSecondTimer>());
            recipeHalf.AddTile(TileID.HeavyWorkBench);
            recipeHalf.AddRecipeGroup(RecipeGroupID.IronBar, 3);
            recipeHalf.AddIngredient(ItemID.TimerOneHalfSecond);
            recipeHalf.Register();
        }
    }

    public class QuarterSecondTimer : TimerItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("1/4 Second Timer");
            /* Tooltip.SetDefault("A timer which doesn't need supporting tiles\n" +
                "Doesn't need to be re-activated on world startup\n" +
                $"[i:{ModContent.ItemType<RecipeItems.Activations>()}] causes Activations"); */
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.createTile = ModContent.TileType<Tiles.Machines.Logic.Timers.QuarterSecondTimer>();
        }
        public override void AddRecipes()
        {
            Recipe recipeQuarter = Recipe.Create(ModContent.ItemType<Timers.QuarterSecondTimer>());
            recipeQuarter.AddTile(TileID.HeavyWorkBench);
            recipeQuarter.AddRecipeGroup(RecipeGroupID.IronBar, 3);
            recipeQuarter.AddIngredient(ItemID.TimerOneFourthSecond);
            recipeQuarter.Register();
        }
    }
}
