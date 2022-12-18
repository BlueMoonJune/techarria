using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;

namespace Techarria.Content.Items.FilterItems
{
    /// <summary>
    /// A class for items that do something special in a Filter
    /// </summary>
    internal abstract class FilterItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 5;
        }

        public override void SetDefaults()
        {
            Item.width = 10;
            Item.height = 10;
            Item.maxStack = 999;
            Item.value = Item.buyPrice(0, 0, 1, 0);
        }


        /// <summary>
        /// Whether or not this filter should accept 'item'
        /// </summary>
        /// <param name="item">The item to test</param>
        /// <returns></returns>
        public virtual bool AcceptsItem(Item item)
        {
            return true;
        }
    }
}
