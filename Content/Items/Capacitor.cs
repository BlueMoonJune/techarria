using Terraria.ModLoader;

namespace Techarria.Content.Items
{
    internal class Capacitor : ChargableItem
    {
        public override void SetStaticDefaults()
        {
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
    }
}
