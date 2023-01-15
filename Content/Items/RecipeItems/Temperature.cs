using Terraria.ModLoader;

namespace Techarria.Content.Items.RecipeItems
{
    internal class Temperature : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Degrees");
            Tooltip.SetDefault("The temperature needed for this recipe");
        }

        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 32;
            Item.maxStack = 99999;
        }
    }
}
