using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Techarria.Content.Tiles.Machines;

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

            Recipe logicGateLampOffRecipe = Recipe.Create(ItemID.LogicGateLamp_Off);
            logicGateLampOffRecipe.AddTile(TileID.Anvils);
            logicGateLampOffRecipe.AddIngredient(ItemID.Torch);
            logicGateLampOffRecipe.AddIngredient(ItemID.Glass);
            logicGateLampOffRecipe.AddIngredient(ItemID.Wire);
            logicGateLampOffRecipe.Register();

            Recipe logicGateLampOnRecipe = Recipe.Create(ItemID.LogicGateLamp_On);
            logicGateLampOnRecipe.AddTile(TileID.Anvils);
            logicGateLampOnRecipe.AddIngredient(ItemID.Torch);
            logicGateLampOnRecipe.AddIngredient(ItemID.Glass);
            logicGateLampOnRecipe.AddIngredient(ItemID.Wire);
            logicGateLampOnRecipe.Register();

            Recipe logicGateLampOffToOnRecipe = Recipe.Create(ItemID.LogicGateLamp_On);
            logicGateLampOffToOnRecipe.AddIngredient(ItemID.LogicGateLamp_Off);
            logicGateLampOffToOnRecipe.Register();

            Recipe logicGateLampOnToOffRecipe = Recipe.Create(ItemID.LogicGateLamp_Off);
            logicGateLampOnToOffRecipe.AddIngredient(ItemID.LogicGateLamp_On);
            logicGateLampOnToOffRecipe.Register();

            Recipe logicGateANDRecipe = Recipe.Create(ItemID.LogicGate_AND);
            logicGateANDRecipe.AddTile(TileID.Anvils);
            logicGateANDRecipe.AddIngredient(ItemID.WireBulb);
            logicGateANDRecipe.AddIngredient(ItemID.Wire, 5);
            logicGateANDRecipe.Register();

        }
    }
}
