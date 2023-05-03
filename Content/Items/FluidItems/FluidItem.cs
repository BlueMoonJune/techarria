using Microsoft.Xna.Framework;
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
		public int liquidType = -1;

		public static Dictionary<int, int> liquidToFluidItem = new Dictionary<int, int>();
        public override void UpdateInventory(Player player)
        {
			if (liquidType >= 0) {
				Point tileP = new((int)player.Bottom.X / 16, (int)player.Center.Y / 16);
				byte amount = (byte)Math.Min(255 - Main.tile[tileP].LiquidAmount, Item.stack);
				Item.stack -= amount;
				WorldGen.PlaceLiquid(tileP.X, tileP.Y, (byte)liquidType, amount);
				if (Item.stack <= 0) {
					Item.TurnToAir();
				}
			}
        }

        public override void HoldItem(Player player)
        {
			if (liquidType >= 0) {
				Point tileP = new((int)player.Center.X / 16, (int)player.Center.Y / 16);
				byte amount = (byte)Math.Min(255 - Main.tile[tileP].LiquidAmount, Item.stack);
				Item.stack -= amount;
				WorldGen.PlaceLiquid(tileP.X, tileP.Y, (byte)liquidType, amount);
				if (Item.stack <= 0) {
					Item.TurnToAir();
				}
			}
		}

		public override void Update(ref float gravity, ref float maxFallSpeed) {
			if (liquidType >= 0) {
				Point tileP = new((int)Item.Bottom.X / 16, (int)Item.Bottom.Y / 16 - 1);
				byte amount = (byte)Math.Min(Math.Min(255 - Main.tile[tileP].LiquidAmount, 16), Item.stack);
				Item.stack -= amount;
				WorldGen.PlaceLiquid(tileP.X, tileP.Y, (byte)liquidType, amount);
				if (Item.stack <= 0) {
					Item.TurnToAir();
					return;
				}
				if (Main.tile[tileP].LiquidAmount == 255)
					Item.velocity.Y = -1;
			}
		}

		public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = 99999;
			Item.rare = ItemRarityID.Blue;
			liquidToFluidItem.TryAdd(liquidType, Type);
        }
    }
    // fluid items
    public class Lava : FluidItem
    {
		public override void SetDefaults() {
			liquidType = LiquidID.Lava;
			base.SetDefaults();
		}
	}
    public class Water : FluidItem
	{
		public override void SetDefaults() {
			liquidType = LiquidID.Water;
			base.SetDefaults();
		}
	}
    public class Shimmer : FluidItem
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(3, 60));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;
		}
		public override void SetDefaults() {
			liquidType = LiquidID.Shimmer;
			base.SetDefaults();
		}
	}
    public class Honey : FluidItem
	{
		public override void SetDefaults() {
			liquidType = LiquidID.Honey;
			base.SetDefaults();
		}
	}
}
