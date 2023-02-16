using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Techarria.Content.Tiles.Machines;

namespace Techarria.Content.Items
{
	public class Capacitor : ChargableItem
    {
        public override void SetStaticDefaults()
        {
			CapacitorRack.capacitorTextures[Type] = "Capacitor";
            Tooltip.SetDefault("Stores charge and can instantaneously release it");
        }

        public override void SetDefaults()
        {
            maxcharge = 100;
            Item.maxStack = 1;
            Item.width = 16;
            Item.height = 16;
            base.SetDefaults();
        }
        public override void AddRecipes()
        {
            Recipe recipe = Recipe.Create(ModContent.ItemType<Capacitor>());
            recipe.AddTile(TileID.HeavyWorkBench);
            recipe.AddIngredient<Materials.SpikeSteelSheet>(3);
            recipe.AddIngredient(ItemID.Wire, 5);
            recipe.Register();
        }
    }
}
