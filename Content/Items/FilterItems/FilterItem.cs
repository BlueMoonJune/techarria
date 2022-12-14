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

        public virtual bool AcceptsItem(Item item)
        {
            return true;
        }
    }
}
