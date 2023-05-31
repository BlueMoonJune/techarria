using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Techarria.Content.Items.Materials;

namespace Techarria.Content.Items.Placeables.FluidTransfer
{
    public class FluidTank : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.FluidTransfer.FluidTank>());
            Item.value = 100;
            Item.maxStack = 99;
            Item.width = 32;
            Item.height = 32;

            Item.useTurn = true;
            Item.autoReuse = true;
            Item.consumable = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 10;
            Item.useAnimation = 15;
        }
        public override void AddRecipes()
        {
            Recipe.Create(ModContent.ItemType<Placeables.FluidTransfer.FluidTank>())
                .AddTile(TileID.WorkBenches)
                .AddIngredient<SpikeSteelSheet>(4)
                .AddIngredient(ItemID.EmptyBucket)
                .Register();
        }
    }
}
