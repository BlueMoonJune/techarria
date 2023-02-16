using Terraria;
using Terraria.ModLoader;

namespace Techarria.Content.Items.RecipeItems
{
    public abstract class RecipeItem : ModItem
    {
        public override void UpdateInventory(Player player)
        {
            Item.TurnToAir();
        }

        public override void HoldItem(Player player)
        {
            Item.TurnToAir();
        }

        public override void SetDefaults()
        {
            Item.maxStack = 99999;
        }
    }
}