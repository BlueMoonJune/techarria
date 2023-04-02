using Terraria.ID;
using Terraria.ModLoader;
using Terraria;

namespace Techarria.Content.Items.Placeables.Machines
{
    public class PowerBlock : ModItem
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return ModContent.GetInstance<Common.Configs.TecharriaServerConfig>().DevItemsEnabled;
        }

        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("Actications produce power\n" +
                $"[i:{ModContent.ItemType<RecipeItems.Power>()}] generates " + Tiles.Machines.PowerBlock.POWERBLOCK_GENERATION + " Power\n" +
                $"[i:{ModContent.ItemType<RecipeItems.Activations>()}] accepts activations"); */
        }
        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = 999;
            Item.consumable = true;
            Item.value = Item.buyPrice(0, 0, 1, 0);
            Item.mech = true;

            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.autoReuse = true;

            Item.createTile = ModContent.TileType<Tiles.Machines.PowerBlock>();
        }
    }
}
