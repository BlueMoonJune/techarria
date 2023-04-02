using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Techarria.Content.Items
{
	internal class Multimeter : ModItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Displays transfer of power");
        }

		public override void SetDefaults()
		{
			Item.width = 32;
			Item.height = 32;
			Item.value = 10000;
			Item.rare = ItemRarityID.Blue;
        }

        public override void UpdateInventory(Player player)
        {
            base.UpdateInventory(player);
        }
    }
}
