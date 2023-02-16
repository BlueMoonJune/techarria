using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent.Creative;

namespace Techarria.Content.Items.Placeables.Machines.Logic
{
	public class TemperatureProbe : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 5;

            Tooltip.SetDefault("Activates wires when the tile in front of it is at target temperature\n" +
                "Target temperature is set when activated\n" +
                $"[i:{ModContent.ItemType<RecipeItems.Temperature>()}] displays Temperature\n" +
                $"[i:{ ModContent.ItemType<RecipeItems.Activations>()}] accepts and causes Activations");
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

            Item.createTile = ModContent.TileType<Tiles.Machines.Logic.TemperatureProbe>();
        }
        public override void AddRecipes()
        {
            Recipe recipe = Recipe.Create(ModContent.ItemType<Logic.TemperatureProbe>());
            recipe.AddTile(TileID.WorkBenches);
            recipe.AddRecipeGroup(RecipeGroupID.IronBar, 5);
            recipe.AddIngredient(ItemID.Fireblossom);
            recipe.AddIngredient(ItemID.Shiverthorn);
            recipe.Register();
        }
    }
}
