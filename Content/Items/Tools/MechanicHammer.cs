using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Microsoft.Xna.Framework;

namespace Techarria.Content.Items.Tools
{
    public class MechanicHammer : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Mechanic's Hammer");
			Tooltip.SetDefault("Left click to trigger blocks\nRight click to spark wires");

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 5;
			Item.useAnimation = 5;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.value = Item.buyPrice(gold: 3, silver: 25);
			Item.rare = ItemRarityID.Blue;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = false; // Automatically re-swing/re-use this item after its swinging animation is over.
			Item.mech = true;
		}

        public override void UseAnimation(Player player)
		{
			if (player.whoAmI == Main.myPlayer)
			{
				Point pos = Main.MouseWorld.ToTileCoordinates();
				TileLoader.HitWire(pos.X, pos.Y, Main.tile[pos.X, pos.Y].TileType);
			}

        }
		public override bool AltFunctionUse(Player player)
		{
			if (player.whoAmI == Main.myPlayer)
			{
				Point pos = Main.MouseWorld.ToTileCoordinates();
				Wiring.TripWire(pos.X, pos.Y, 1, 1);
				return true;
			}
			return false;
		}

		public override void AddRecipes()
		{
			Recipe recipe = Recipe.Create(ModContent.ItemType<MechanicHammer>());
			recipe.AddTile(TileID.Anvils);
			recipe.AddRecipeGroup(RecipeGroupID.IronBar, 10);
			recipe.AddIngredient(ItemID.SoulofFlight);
			recipe.AddIngredient(ItemID.Wire);
			recipe.Register();
		}
	}
}
