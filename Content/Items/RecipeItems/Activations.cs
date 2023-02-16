using Terraria;
using Terraria.ModLoader;

namespace Techarria.Content.Items.RecipeItems
{
    public class Activations : RecipeItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Activations");
            Tooltip.SetDefault("Logic signal activations needed for this recipe");
        }
    }
}
