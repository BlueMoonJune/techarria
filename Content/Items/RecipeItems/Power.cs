﻿using Terraria.ModLoader;

namespace Techarria.Content.Items.RecipeItems
{
    public class Power : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Power");
            Tooltip.SetDefault("Power needed for this recipe");
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 24;
            Item.maxStack = 99999;
        }
    }
}
