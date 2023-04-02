using Terraria.ModLoader;
using Terraria;
using Terraria.ID;

namespace Techarria.Content.Items.Placeables.Machines.Logic
{
    public class Button : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Button");
            // Tooltip.SetDefault($"[i:{ModContent.ItemType<RecipeItems.Activations>()}] causes Activations");
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Machines.Logic.Button>());
            Item.value = 100;
            Item.maxStack = 99;

            Item.useTurn = true;
            Item.autoReuse = true;
            Item.consumable = true;
            Item.value = Item.buyPrice(silver: 30);
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 10;
            Item.useAnimation = 15;
            Item.mech = true;
        }
    }
}
