using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Net;
using Terraria.GameContent.NetModules;
using Terraria.GameContent.Creative;
using Techarria.Content.Items.Materials;
using Microsoft.Xna.Framework;
using Techarria.Content.Items.Armor;

namespace Techarria.Content.Items
{
	public class EMPGrenade : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("EMP Grenade");

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 99; // How many items are needed in order to research duplication of this item in Journey mode. See https://terraria.gamepedia.com/Journey_Mode/Research_list for a list of commonly used research amounts depending on item type.
		}

		public override void SetDefaults()
		{
			Item.width = 28; // The item texture's width
			Item.height = 28; // The item texture's height
			Item.maxStack = 99; // The item's max stack value
			Item.value = Item.buyPrice(silver: 1); // The value of the item in copper coins. Item.buyPrice & Item.sellPrice are helper methods that returns costs in copper coins based on platinum/gold/silver/copper arguments provided to it.

			// Use Properties
			Item.useTime = 60; // The item's use time in ticks (60 ticks == 1 second.)
			Item.useAnimation = 60; // The length of the item's use animation in ticks (60 ticks == 1 second.)
			Item.useStyle = ItemUseStyleID.HoldUp; // How you use the item (swinging, holding out, etc.)
			Item.autoReuse = false; // Whether or not you can hold click to automatically use it again.

			Item.mech = true;
			Item.consumable = true;

			Item.shoot = ModContent.ProjectileType<Projectiles.EMPGrenade>();
			Item.shootSpeed = 18f;
		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ModContent.ItemType<SpikeSteelSheet>(), 5)
				.AddIngredient(ModContent.ItemType<Capacitor>(), 1)
				.AddIngredient(ItemID.Wire, 10)
				.AddTile(TileID.Anvils)
				.Register();
		}

        public override void HoldItem(Player player)
        {
			Vector2 origin = player.Center;
			Vector2 mouse = Main.MouseWorld;
			Vector2 dif = mouse - origin;
			dif.Normalize();
			dif *= 334.76f;
			Vector2 center = origin + dif;
			Vector2 offset = new Vector2(Projectiles.EMPGrenade.range, 0);
			offset = offset.RotatedBy(player.GetModPlayer<PowerArmorPlayer>().frames * 0.05);
			for (int i = 0; i < 4; i++)
			{
				Dust dust = Dust.NewDustDirect(center + offset, 0, 0, DustID.BlueFairy);
				dust.alpha = 255;
				offset = offset.RotatedBy(MathHelper.Pi / 2);
			}
		}
    }
}