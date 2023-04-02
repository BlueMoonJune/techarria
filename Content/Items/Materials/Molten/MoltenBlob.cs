using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Techarria.Content.Items.Materials.Molten
{
	public abstract class MoltenBlob : ModItem
    {
        public float temp;

        public override void HoldItem(Player player)
        {
            player.AddBuff(BuffID.Burning, 60);
            player.AddBuff(BuffID.OnFire, 60);
        }

        public override void UpdateInventory(Player player)
        {
            player.AddBuff(BuffID.Burning, 60);
            player.AddBuff(BuffID.OnFire, 60);
        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            if (Item.velocity.Y == 0)
            {
                WorldGen.PlaceLiquid((int)Item.Center.X / 16, (int)Item.Center.Y / 16, (byte)LiquidID.Lava, 255);
                Item.TurnToAir();
            }
        }
    }
}
