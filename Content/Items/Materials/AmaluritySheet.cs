using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;
using Techarria.Content.Items.Materials.Molten;
using Techarria.Content.Tiles.Machines;

namespace Techarria.Content.Items.Materials
{
	public class AmaluritySheet : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Spikesteel Sheet");
            // Tooltip.SetDefault("Casted from molten Spikesteel for use in crafting");

            // journey mode
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 50;
        }
        public override void SetDefaults() {
			Item.width = 20; // The item texture's width
			Item.height = 20; // The item texture's height

			Item.maxStack = 999; // The item's max stack value
            // The value of the item in copper coins. Item.buyPrice & Item.sellPrice are helper methods that returns costs in copper coins based on
            // platinum/gold/silver/copper arguments provided to it.
            Item.value = Item.buyPrice(silver: 10);
		}

        public override void AddRecipes()
        {
            new CastingTableRecipe(ModContent.ItemType<AmalurityBlob>(), Type, ModContent.ItemType<SheetMold>(), 60);
        }
    }
}
