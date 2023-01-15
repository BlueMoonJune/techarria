using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Net;
using Terraria.GameContent.NetModules;
using Terraria.GameContent.Creative;


namespace Techarria.Content.Items.Materials
{
    internal class SpikeSteelSheet : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spikesteel Sheet");
            Tooltip.SetDefault("Casted from molten Spikesteel for use in crafting");

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
    }
}
