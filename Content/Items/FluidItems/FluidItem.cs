using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Techarria.Content.Items.FluidItems
{
    public abstract class FluidItem : ModItem
    {
        public override void UpdateInventory(Player player)
        {
            //Item.TurnToAir();
        }

        public override void HoldItem(Player player)
        {
            //Item.TurnToAir();
        }

        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = 99999;
        }
    }
    // fluid items
    public class Lava : FluidItem
    {
    }
    public class Water : FluidItem
    {
    }
    public class Shimmer : FluidItem
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(3, 60));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;
        }
    }
    public class Honey : FluidItem
    {
    }
}
