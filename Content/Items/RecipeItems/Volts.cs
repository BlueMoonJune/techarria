using Terraria.ModLoader;

namespace Techarria.Content.Items.RecipeItems
{
    internal class Volts : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Volts");
            Tooltip.SetDefault("Single pulse charge needed for this recipe");
        }

        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 32;
            Item.maxStack = 99999;
        }
    }
}
