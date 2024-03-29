﻿using Terraria.ID;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;
using Techarria.Content.Items.Materials;

namespace Techarria.Content.Items.Placeables.Furniture
{
    public class TravelingMarket : ModItem
    {
        public override void SetStaticDefaults()
        {
            // journey mode
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }
        public override void SetDefaults()
        {
            Item.createTile = ModContent.TileType<Tiles.Furniture.TravelingMarket>(); // This sets the id of the tile that this item should place when used.

            Item.width = 54; // The item texture's width
            Item.height = 56; // The item texture's height

            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 10;
            Item.useAnimation = 15;

            Item.maxStack = 99;
            Item.consumable = true;
            Item.value = 150;
        }

        //adds the recipe
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddTile(TileID.Sawmill)
                .AddRecipeGroup(RecipeGroupID.Wood, 55)
                .AddIngredient(ItemID.WoodenChair, 2)
                .AddIngredient(ItemID.Bar)
                .AddIngredient(ItemID.Silk, 12)
                .AddIngredient(ItemID.Barrel, 3)
                .Register();
        }
    }
}
