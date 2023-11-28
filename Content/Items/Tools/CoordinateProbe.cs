using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Microsoft.Xna.Framework;

namespace Techarria.Content.Items.Tools
{
    public class CoordinateProbe : ModItem
	{
		public override void SetStaticDefaults()
		{
			// Tooltip.SetDefault("Displays the coordinates of the clicked tile");

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.width = 32;
			Item.height = 32;
			Item.useTime = 15;
			Item.useAnimation = 15;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.value = 10000;
			Item.rare = ItemRarityID.Blue;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true; // Automatically re-swing/re-use this item after its swinging animation is over.
		}

        public override void UpdateInventory(Player player)
        {
			InfoDisplay.Compass.Active();
            InfoDisplay.DepthMeter.Active();
			InfoDisplay.Watches.Active();
        }

        public override void UseAnimation(Player player)
        {
			if (player.whoAmI == Main.myPlayer)
			{
				Vector2 pos = Main.MouseWorld;
				Point tile = Main.MouseWorld.ToTileCoordinates();

				Main.NewText($"Tile Coords: X:{tile.X}, Y:{tile.Y}\n" +
					$"Entity Coords: X:{(int)pos.X}, Y:{(int)pos.Y}\n" +
					$"Tile Type: {Main.tile[tile].TileType}\n" +
					$"Tile Frame: {Main.tile[tile].TileFrameX}, {Main.tile[tile].TileFrameY}\n" +
					$"Liquid Type: {Main.tile[tile].LiquidType}\n" +
					$"Liquid Amount: {Main.tile[tile].LiquidAmount}");
				
			}

        }

        public override void AddRecipes()
        {
            Recipe recipe = Recipe.Create(ModContent.ItemType<CoordinateProbe>());
            recipe.AddTile(TileID.HeavyWorkBench);
            recipe.AddIngredient(ItemID.GPS);
            recipe.AddIngredient(ItemID.Wire, 5);
			recipe.AddIngredient(ItemID.AnnouncementBox);
            recipe.Register();
        }
    }
}
