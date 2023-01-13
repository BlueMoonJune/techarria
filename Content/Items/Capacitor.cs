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
            maxcharge = 1000;
            Item.maxStack = 1;
            base.SetDefaults();
        }
    }
}
