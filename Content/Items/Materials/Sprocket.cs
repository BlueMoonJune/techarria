using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;
using Techarria.Content.Tiles.Machines;

namespace Techarria.Content.Items.Materials
{
    public class Sprocket : ModItem
    {
        public override void SetStaticDefaults()
        {
            // journey mode
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 200;

            ItemID.Sets.SortingPriorityMaterials[Item.type] = 58;
        }
        public override void SetDefaults()
        {
            Item.width = 22; // The item texture's width
            Item.height = 22; // The item texture's height

            Item.maxStack = 999; // The item's max stack value
            // The value of the item in copper coins. Item.buyPrice & Item.sellPrice are helper methods that returns costs in copper coins based on
            // platinum/gold/silver/copper arguments provided to it.
            Item.value = Item.buyPrice(silver: 1, copper: 25);
        }

		public override void AddRecipes() {
			new AssemblyRecipe(new Item(ModContent.ItemType<Sprocket>()), new Item(ItemID.Cog))
				.AddItem(new Item(ItemID.Chain), 0)
				.AddItem(new Item(ItemID.Chain), 1)
				.AddItem(new Item(ItemID.Chain), 2)
				.AddItem(new Item(ItemID.Chain), 3)
				.AddItem(new Item(ItemID.Chain), 4)
				.AddItem(new Item(ItemID.Chain), 5)
				.AddItem(new Item(ItemID.Chain), 6)
				.AddItem(new Item(ItemID.Chain), 7);
		}
	}
}
