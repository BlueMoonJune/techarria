using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace Techarria.Content.Items.Materials
{
    public class CoalCoke : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Coal Coke");

            // journey mode
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 50;
        }
        
        public override void SetDefaults()
        {
            Item.width = 16; // The item texture's width
            Item.height = 16; // The item texture's height

            Item.maxStack = 999; // The item's max stack value
            // The value of the item in copper coins. Item.buyPrice & Item.sellPrice are helper methods that returns costs in copper coins based on
            // platinum/gold/silver/copper arguments provided to it.
            Item.value = Item.buyPrice(silver: 3, copper: 87);
        }
        

        public override void AddRecipes()
        {
            Recipe recipe = Recipe.Create(ModContent.ItemType<Items.Materials.CoalCoke>());
            recipe.AddTile(TileID.Furnaces);
            recipe.AddIngredient<Materials.IndustrialCoal>(3);
            recipe.Register();
        }
    }
}
