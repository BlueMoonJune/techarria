using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Microsoft.Xna.Framework;

namespace Techarria.Content.Items.Tools
{
    internal class CoordinateProbe : ModItem
	{
		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault("Displays the coordinates of the clicked tile");

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.width = 52;
			Item.height = 52;
			Item.useTime = 15;
			Item.useAnimation = 15;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.value = 10000;
			Item.rare = ItemRarityID.Blue;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true; // Automatically re-swing/re-use this item after its swinging animation is over.
		}

        public override void UseAnimation(Player player)
        {
			if (player.whoAmI == Main.myPlayer)
			{
				Vector2 pos = Main.MouseWorld;
				Point tile = Main.MouseWorld.ToTileCoordinates();
				Main.NewText("Entity Coordinates: " + pos.X + ", " + pos.Y);
				Main.NewText("Tile Coordinates: " + tile.X + ", " + tile.Y);

			}

        }
    }
}
