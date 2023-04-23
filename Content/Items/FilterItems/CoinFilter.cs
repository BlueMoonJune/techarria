using System.Collections.Generic;
using Techarria.Content.Tiles;
using Terraria;
using Terraria.GameContent.Creative;

namespace Techarria.Content.Items.FilterItems
{
	/// <summary>
	/// Prevents extraction of singular items
	/// </summary>
	public class CoinFilter : FilterItem
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
			return new List<int>(CashCompactorTE.coinTypes).Contains(item.type);
        }
    }
}
