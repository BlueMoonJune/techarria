using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace Techarria.VanillaChanges
{
    public class VanillaRecipes : ModSystem
    {
        public override void AddRecipes()
        {
            Recipe wireRecipe = Recipe.Create(ItemID.Wire, 25);
            wireRecipe.AddTile(TileID.HeavyWorkBench);
            wireRecipe.AddRecipeGroup(nameof(ItemID.CopperBar), 5);
            wireRecipe.Register();
        }
    }
}
