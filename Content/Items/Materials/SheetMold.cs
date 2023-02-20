using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace Techarria.Content.Items.Materials
{
	public class SheetMold : Mold
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Used in the Casting Table to make sheets");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.maxStack = 1;
        }
        public override void AddRecipes()
        {
            Recipe recipe = Recipe.Create(ModContent.ItemType<Items.Materials.SheetMold>());
            recipe.AddTile(TileID.HeavyWorkBench);
            recipe.AddIngredient<Materials.CoalCoke>(12);
            recipe.Register();
        }
    }
}
