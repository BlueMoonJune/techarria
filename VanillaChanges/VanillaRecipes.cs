using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Techarria.Content.Tiles.Machines;
using Techarria.Content.Tiles.Furniture.WorkStations;

namespace Techarria.VanillaChanges
{
    public class VanillaRecipes : ModSystem
    {
        public override void AddRecipes()
        {
            // wire
            Recipe.Create(ItemID.Wire, 25)
                .AddTile(TileID.HeavyWorkBench)
                .AddRecipeGroup(nameof(ItemID.CopperBar), 5)
                .Register();

            // logic gates
            // off
            Recipe.Create(ItemID.LogicGateLamp_Off)
                .AddTile(TileID.Anvils)
                .AddIngredient(ItemID.Torch)
                .AddIngredient(ItemID.Glass)
                .AddIngredient(ItemID.Wire)
                .Register();
            // on
            Recipe.Create(ItemID.LogicGateLamp_On)
                .AddTile(TileID.Anvils)
                .AddIngredient(ItemID.Torch)
                .AddIngredient(ItemID.Glass)
                .AddIngredient(ItemID.Wire)
                .Register();
            // on from off
            Recipe.Create(ItemID.LogicGateLamp_On)
                .AddIngredient(ItemID.LogicGateLamp_Off)
                .Register();
            // off from on
            Recipe logicGateLampOnToOffRecipe = Recipe.Create(ItemID.LogicGateLamp_Off)
                .AddIngredient(ItemID.LogicGateLamp_On)
                .Register();
            // and gate
            Recipe.Create(ItemID.LogicGate_AND)
                .AddTile(TileID.Anvils)
                .AddIngredient(ItemID.WireBulb)
                .AddIngredient(ItemID.Wire, 5)
                .Register();

            // extractinator
            Recipe.Create(ItemID.Extractinator)
                .AddTile(TileID.HeavyWorkBench)
                .AddRecipeGroup(RecipeGroupID.IronBar, 15)
                .AddIngredient(ItemID.Diamond)
                .Register();

            // souls ------------ subject to major change ------------
            // night
            Recipe.Create(ItemID.SoulofNight, 50)
                .AddTile<SoulGrinder>()
                .AddCondition(Condition.InEvilBiome)
                .AddIngredient(ItemID.SlimeCrown) 
                .Register();
            // light
            Recipe.Create(ItemID.SoulofLight, 50)
                .AddTile<SoulGrinder>()
                .AddCondition(Condition.InHallow)
                .AddIngredient(ItemID.SlimeCrown)
                .Register();

            // life crystal
            Recipe.Create(ItemID.LifeCrystal)
                .AddTile(TileID.Solidifier)
                .AddIngredient(ItemID.Heart, 5)
                .Register();
        }
    }
}
