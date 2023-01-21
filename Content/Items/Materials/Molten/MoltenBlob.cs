using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Techarria.Content.Items.Materials.Molten
{
    public abstract class MoltenBlob : ModItem
    {
        public float temp;

        public override void UpdateInventory(Player player)
        {
            if (player.HasItem(ModContent.ItemType<MoltenSpikeSteel>()))
            {
                player.AddBuff(BuffID.Burning, 60);
                player.AddBuff(BuffID.OnFire, 60);
            }
        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            if (Item.velocity.Y == 0)
            {
                WorldGen.PlaceLiquid((int)Item.Center.X / 16, (int)Item.Center.Y / 16, LiquidID.Lava, 255);
                Item.TurnToAir();
            }
        }
    }
}
