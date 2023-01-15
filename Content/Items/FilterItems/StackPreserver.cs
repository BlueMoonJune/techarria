using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Microsoft.Xna.Framework;

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
    }
}
