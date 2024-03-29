﻿using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria.GameContent.Creative;

namespace Techarria.Content.Items.Tools.Adhesive
{
	public abstract class Adhesive : ModItem
	{
		public int type;

        public override void SetStaticDefaults()
        {
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

        public override void SetDefaults() {
			Item.width = 28;
			Item.height = 24;
			Item.maxStack = 1;
			Item.value = Item.buyPrice(0, 0, 1, 0);

			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTurn = true;
			Item.useAnimation = 5;
			Item.useTime = 5;
			Item.autoReuse = true;

			Item.useAmmo = 1;
		}

		public override bool? CanChooseAmmo(Item ammo, Player player) {
			if (ammo.type == ItemID.Gel || ammo.type == ItemID.PinkGel || ammo.type == ItemID.HoneyBlock) {
				if (ammo.stack <= 0) {
					ammo.TurnToAir();
				}
				return true;
			}
			return false;
		}

		public override bool CanUseItem(Player player) {
			Vector2 vec = Main.MouseWorld / 16;
			Point pos = new((int)vec.X, (int)vec.Y);
			Tile tile = Main.tile[pos];
			return !tile.Get<Glue>().GetChannel(type);
		}

		public override void UseAnimation(Player player) {
			Vector2 vec = Main.MouseWorld / 16;
			Point pos = new((int)vec.X, (int)vec.Y);
			Tile tile = Main.tile[pos];
			tile.Get<Glue>().SetChannel(type, true);
			SoundEngine.PlaySound(SoundID.NPCHit25, Main.MouseWorld);
		}
	}

	public class SlimyAdhesive : Adhesive 
	{
		public override void SetDefaults() {
			base.SetDefaults();
			
			type = 0;
		}

		public override void SetStaticDefaults()
		{
			base.SetStaticDefaults();
			/* Tooltip.SetDefault("Places blue adhesive\n" +
				"'Reccomended for ingestion'"); */
		}

        public override void AddRecipes()
        {
			Recipe recipeSlime = Recipe.Create(ModContent.ItemType<SlimyAdhesive>());
			recipeSlime.AddTile(TileID.Blendomatic);
			recipeSlime.AddRecipeGroup(RecipeGroupID.IronBar, 5);
			recipeSlime.AddIngredient(ItemID.SlimeBlock, 3);
			recipeSlime.Register();
		}
    }
	public class FrigidAdhesive : Adhesive
	{
		public override void SetDefaults() {
			base.SetDefaults();
			
			type = 1;
		}

        public override void SetStaticDefaults()
        {
			base.SetStaticDefaults();
			/* Tooltip.SetDefault("Places light blue adhesive\n" +
				"'Its like smearing a popsicle on the ground'"); */
		}
		public override void AddRecipes()
		{
			Recipe recipeIce = Recipe.Create(ModContent.ItemType<FrigidAdhesive>());
			recipeIce.AddTile(TileID.Blendomatic);
			recipeIce.AddRecipeGroup(RecipeGroupID.IronBar, 5);
			recipeIce.AddIngredient(ItemID.FrozenSlimeBlock, 3);
			recipeIce.Register();
		}
	}
	public class ElasticAdhesive : Adhesive
	{
		public override void SetDefaults() {
			base.SetDefaults();
			
			type = 2;
		}
		public override void SetStaticDefaults()
		{
			base.SetStaticDefaults();
			/* Tooltip.SetDefault("Places pink adhesive\n" +
				"'Reccomended for ingestion'"); */
		}
		public override void AddRecipes()
		{
			Recipe recipePink = Recipe.Create(ModContent.ItemType<ElasticAdhesive>());
			recipePink.AddTile(TileID.Blendomatic);
			recipePink.AddRecipeGroup(RecipeGroupID.IronBar, 5);
			recipePink.AddIngredient(ItemID.PinkSlimeBlock, 3);
			recipePink.Register();
		}
	}
	public class SweetAdhesive : Adhesive
	{
		public override void SetDefaults() {
			base.SetDefaults();
			
			type = 3;
		}
		public override void SetStaticDefaults()
		{
			base.SetStaticDefaults();
			/* Tooltip.SetDefault("Places yellow adhesive\n" +
				"'Reccomended for ingestion'"); */
		}
		public override void AddRecipes()
		{
			Recipe recipeHoney = Recipe.Create(ModContent.ItemType<SweetAdhesive>());
			recipeHoney.AddTile(TileID.Blendomatic);
			recipeHoney.AddRecipeGroup(RecipeGroupID.IronBar, 5);
			recipeHoney.AddIngredient(ItemID.HoneyBlock, 3);
			recipeHoney.Register();
		}
	}


	public class AdhesiveSolvent : ModItem
	{
        public override void SetStaticDefaults()
        {
			// Tooltip.SetDefault("Removes adhesive");
        }
        public override void SetDefaults() {
			Item.width = 32;
			Item.height = 32;
			Item.maxStack = 1;
			Item.value = Item.buyPrice(0, 0, 1, 0);

			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTurn = true;
			Item.useAnimation = 15;
			Item.useTime = 5;
			Item.autoReuse = true;
		}

		public override void UseAnimation(Player player) {
			Vector2 vec = Main.MouseWorld / 16;
			Point pos = new((int)vec.X, (int)vec.Y);
			Tile tile = Main.tile[pos];

			if (tile.Get<Glue>().types != 0) {
				tile.Get<Glue>().types = 0;
				SoundEngine.PlaySound(SoundID.NPCDeath28, Main.MouseWorld);
			}
		}

		public override void AddRecipes()
		{
			Recipe recipe = Recipe.Create(ModContent.ItemType<AdhesiveSolvent>());
			recipe.AddTile(TileID.Blendomatic);
			recipe.AddRecipeGroup(RecipeGroupID.IronBar, 5);
			recipe.AddIngredient(ItemID.Torch, 3);
			recipe.Register();
		}
	}
}
