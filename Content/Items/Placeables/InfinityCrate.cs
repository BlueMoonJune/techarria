using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Terraria.Localization;

namespace Techarria.Content.Items.Placeables
{
	public class InfinityCrate : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Infinity Crate");

			Tooltip.SetDefault("Can be infinetely inserted into or extracted from\n"
				+ "Any inserted items will be cleared of any reforges or similar\n"
				+ "'Check my recipe ;)'");
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.InfinityCrate>());
			Item.value = 100;
			Item.maxStack = 99;
			Item.width = 32;
			Item.height = 32;

			Item.useTurn = true;
			Item.autoReuse = true;
			Item.consumable = true;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = 10;
			Item.useAnimation = 15;
		}
		public override void AddRecipes()
		{
			// recipe.AddCondition(NetworkText.FromKey("\n"), r => true); is a new line in the recipe

			Recipe recipe = Recipe.Create(ModContent.ItemType<Placeables.InfinityCrate>());
			// requires various tiles
			recipe.AddTile(TileID.Heart);
			recipe.AddTile(TileID.DemonAltar);
			// requires water candle buff
			recipe.AddCondition(Recipe.Condition.InWaterCandle);
			// requires expert mode
			recipe.AddCondition(NetworkText.FromKey("\n"), r => true);
			recipe.AddCondition(NetworkText.FromKey("No casuals"), r => Main.masterMode);
			// npc conditions
			recipe.AddCondition(NetworkText.FromKey("\n"), r => true);
			recipe.AddCondition(NetworkText.FromKey("You have talked to Angler"), r => NPC.savedAngler);
			recipe.AddCondition(NetworkText.FromKey("Goblin Tinkerer has not been saved"), r => !NPC.savedGoblin);
			// active NPC
			recipe.AddCondition(NetworkText.FromKey("\n"), r => true);
			//recipe.AddCondition(NetworkText.FromKey("Currently fighting EOL"), r => NPC.AnyNPCs(NPCID.HallowBoss));
			// boss downed conditions
			recipe.AddCondition(NetworkText.FromKey("The evil presence has been left alive"), r => !NPC.downedBoss1);
			recipe.AddCondition(NetworkText.FromKey("The Old Man is cursed"), r => !NPC.downedBoss3);
			recipe.AddCondition(NetworkText.FromKey("Golem has not been destroyed"), r => !NPC.downedGolemBoss);
			// time conditions
			recipe.AddCondition(NetworkText.FromKey("\n"), r => true);
			recipe.AddCondition(NetworkText.FromKey("Daytime"), r => Main.dayTime);
			// player conditions
			recipe.AddCondition(NetworkText.FromKey("\n"), r => true);
			recipe.AddCondition(NetworkText.FromKey("100 max hp"), r => Main.LocalPlayer.statLifeMax == 100);

			// items required
			recipe.AddIngredient(ItemID.GoldWaterStrider, 3);
			recipe.AddIngredient(ItemID.Waldo);
			// register
			recipe.Register();
		}
	}
}
