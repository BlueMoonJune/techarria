﻿using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Techarria.Content.Items.FilterItems
{
	/// <summary>
	/// Prevents extraction of singular items
	/// </summary>
	public class StackPreserver : FilterItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 5;

            /* Tooltip.SetDefault("Can be placed in a Filter\n" +
                "If in a whitelist filter will preserve a single item from a stack\n" +
                "If in a blacklist filter will only take single items"); */
        }
        
        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.maxStack = 999;
            Item.value = Item.buyPrice(0, 0, 1, 0);
        }

        public override bool AcceptsItem(Item item)
        {
            bool accepts = item.stack > 1;
            return accepts;
        }

        public override void AddRecipes()
        {
            Recipe recipe = Recipe.Create(ModContent.ItemType<StackPreserver>());
            recipe.AddIngredient(ItemID.ChestLock);
            recipe.AddIngredient(ItemID.Cobweb);
            recipe.Register();
        }
    }
}
