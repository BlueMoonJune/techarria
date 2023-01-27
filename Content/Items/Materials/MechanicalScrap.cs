using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;
using Techarria.Content.Tiles.Machines;
using Terraria.ID;

namespace Techarria.Content.Items.Materials
{
    public class MechanicalScrap : ModItem
    {
        public override void SetStaticDefaults()
        {
            // journey mode
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 25;
        }
        public override void SetDefaults()
        {
            Item.width = 28; // The item texture's width
            Item.height = 28; // The item texture's height

            Item.maxStack = 999; // The item's max stack value
            // The value of the item in copper coins. Item.buyPrice & Item.sellPrice are helper methods that returns costs in copper coins based on
            // platinum/gold/silver/copper arguments provided to it.
            Item.value = Item.buyPrice(silver: 1, copper: 25);
        }

		public override void AddRecipes() {
			new AssemblyRecipe(new Item(ItemID.MechanicalWheelPiece), new Item(ItemID.Ruby))
				.AddItem(new Item(ModContent.ItemType<MechanicalScrap>()), 0)
				.AddItem(new Item(ModContent.ItemType<Sprocket>()), 1)
				.AddItem(new Item(ModContent.ItemType<MechanicalScrap>()), 2)
				.AddItem(new Item(ModContent.ItemType<Sprocket>()), 3)
				.AddItem(new Item(ModContent.ItemType<MechanicalScrap>()), 4)
				.AddItem(new Item(ModContent.ItemType<Sprocket>()), 5)
				.AddItem(new Item(ModContent.ItemType<MechanicalScrap>()), 6)
				.AddItem(new Item(ModContent.ItemType<Sprocket>()), 7);
		}
	}
}
