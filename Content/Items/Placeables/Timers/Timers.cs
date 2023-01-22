using Terraria.ModLoader;
using Terraria;
using Terraria.ID;

namespace Techarria.Content.Items.Placeables.Timers
{
	internal class FiveSecondTimer : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("5 Second Timer");
        }

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.maxStack = 999;
            Item.consumable = true;
            Item.value = Item.buyPrice(0, 0, 1, 0);
            Item.mech = true;

            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.autoReuse = true;

            Item.createTile = ModContent.TileType<Tiles.Machines.Logic.Timers.FiveSecondTimer>();
        }
    }

    internal class ThreeSecondTimer : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("3 Second Timer");
        }

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.maxStack = 999;
            Item.consumable = true;
            Item.value = Item.buyPrice(0, 0, 1, 0);
            Item.mech = true;

            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.autoReuse = true;

            Item.createTile = ModContent.TileType<Tiles.Machines.Logic.Timers.ThreeSecondTimer>();
        }
    }

    internal class OneSecondTimer : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("1 Second Timer");
        }

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.maxStack = 999;
            Item.consumable = true;
            Item.value = Item.buyPrice(0, 0, 1, 0);
            Item.mech = true;

            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.autoReuse = true;

            Item.createTile = ModContent.TileType<Tiles.Machines.Logic.Timers.OneSecondTimer>();
        }
    }

    internal class HalfSecondTimer : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("1/2 Second Timer");
        }

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.maxStack = 999;
            Item.consumable = true;
            Item.value = Item.buyPrice(0, 0, 1, 0);
            Item.mech = true;

            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.autoReuse = true;

            Item.createTile = ModContent.TileType<Tiles.Machines.Logic.Timers.HalfSecondTimer>();
        }
    }

    internal class QuarterSecondTimer : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("1/4 Second Timer");
        }

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.maxStack = 999;
            Item.consumable = true;
            Item.value = Item.buyPrice(0, 0, 1, 0);
            Item.mech = true;

            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.autoReuse = true;

            Item.createTile = ModContent.TileType<Tiles.Machines.Logic.Timers.QuarterSecondTimer>();
        }
    }
}
