using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Microsoft.Xna.Framework;
using Techarria.Content.Items.RecipeItems;

namespace Techarria.Content.Items.Materials.Molten
{
    public class MoltenSpikeSteel : ModItem
    {
        public override void UpdateInventory(Player player)
        {
            if (player.HasItem(ModContent.ItemType<MoltenSpikeSteel>()))
            {
                player.AddBuff(BuffID.Burning, 60);
                player.AddBuff(BuffID.OnFire, 60);

            }
        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            if (Item.velocity.Y == 0)
            {
                WorldGen.PlaceLiquid((int)Item.Center.X / 16, (int)Item.Center.Y / 16, LiquidID.Lava, 255);
                Item.TurnToAir();
            }
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 22;
            Item.maxStack = 999;
        }

        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("'I would recommend putting this down'");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 25;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup(RecipeGroupID.IronBar)
                .AddIngredient(ItemID.Spike, 5)
                .AddIngredient(ModContent.ItemType<Volts>(), 600)
                .Register();

            CreateRecipe()
                .AddIngredient(ItemID.IronOre, 6)
                .AddIngredient(ItemID.Spike, 10)
                .AddIngredient(ModContent.ItemType<Temperature>(), 1500)
                .Register();

            CreateRecipe()
                .AddIngredient(ItemID.LeadOre, 6)
                .AddIngredient(ItemID.Spike, 10)
                .AddIngredient(ModContent.ItemType<Temperature>(), 1500)
                .Register();
        }
    }
}
