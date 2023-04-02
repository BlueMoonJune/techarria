using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace Techarria.Content.Items.Materials
{
	public class IndustrialCoal : ModItem
	{
		public override void SetStaticDefaults() {
			// DisplayName.SetDefault("Industrial Coal");
			// Tooltip.SetDefault("'Even less festive than normal'");

			// journey mode
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 200;

			ItemID.Sets.SortingPriorityMaterials[Item.type] = 58;
		}
		public override void SetDefaults() {
			Item.width = 22; // The item texture's width
			Item.height = 22; // The item texture's height

			Item.maxStack = 999; // The item's max stack value
								 // The value of the item in copper coins. Item.buyPrice & Item.sellPrice are helper methods that returns costs in copper coins based on
								 // platinum/gold/silver/copper arguments provided to it.
			Item.value = Item.buyPrice(silver: 1, copper: 25);
		}
	}
}

