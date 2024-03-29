﻿using Terraria.ID;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;
using Terraria;

namespace Techarria.Content.Items.Placeables
{
    public class CashCompactor : ModItem
    {
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Storage Crate");

			/* Tooltip.SetDefault("Stores up to 999999 matching items\n"
				+ "Any inserted items will be cleared of any reforges or similar\n" +
				"'Stores items with 99% accuracy'"); */

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Misc.CashCompactor>());
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
			Recipe recipe = Recipe.Create(ModContent.ItemType<Placeables.CashCompactor>());
			recipe.AddTile(TileID.HeavyWorkBench);
			recipe.AddIngredient(ItemID.CopperCoin);
            recipe.AddIngredient(ItemID.SilverCoin);
            recipe.AddIngredient(ItemID.GoldCoin);
            recipe.AddIngredient(ItemID.PlatinumCoin);
            recipe.AddIngredient(ItemID.Wire, 5);
            recipe.AddRecipeGroup(nameof(ItemID.Chest));
			recipe.Register();
		}
	}
}
