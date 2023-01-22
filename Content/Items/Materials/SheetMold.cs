﻿namespace Techarria.Content.Items.Materials
{
	public class SheetMold : Mold
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Used in the Casting Table to make sheets");
        }

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.maxStack = 1;
        }
    }
}
