﻿using Terraria.ID;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;
using Techarria.Content.Items.Materials;

namespace Techarria.Content.Items.Placeables.Furniture
{
    public class SpikeSteelChair : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Spikesteel Chair");

            // journey mode
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }
        public override void SetDefaults()
        {
            Item.createTile = ModContent.TileType<Tiles.Furniture.SpikeSteel.SpikeSteelChair>(); // This sets the id of the tile that this item should place when used.

            Item.width = 16; // The item texture's width
            Item.height = 16; // The item texture's height

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
                .AddTile(TileID.Anvils)
                .AddIngredient<SpikeSteelSheet>(4)
                .Register();
        }
    }
}
