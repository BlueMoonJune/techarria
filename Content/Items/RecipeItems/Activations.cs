using Terraria.ModLoader;

namespace Techarria.Content.Items.RecipeItems
{
    public class Activations : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Activations");
            Tooltip.SetDefault("Logic signal activations needed for this recipe");
        }

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.maxStack = 99999;
        }
    }
}
