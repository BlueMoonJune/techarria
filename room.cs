using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Microsoft.Xna.Framework;

namespace Techarria
{
    internal class room
    {
    }

	public class RoomTester : ModItem
	{
		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault("Displays room info when clicking");

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

		public override void UseAnimation(Player player)
		{
			if (player.whoAmI == Main.myPlayer)
			{
				Vector2 pos = Main.MouseWorld;
				Point tile = Main.MouseWorld.ToTileCoordinates();

				Main.NewText($"Tile Coords: X:{tile.X}, Y:{tile.Y}\nEntity Coords: X:{(int)pos.X}, Y:{(int)pos.Y}\nTile Type: {Main.tile[tile].TileType}");
			}

		}
	}
}
