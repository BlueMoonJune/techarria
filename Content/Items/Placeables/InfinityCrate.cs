using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Terraria.Localization;

namespace Techarria.Content.Items.Placeables
{
    public class InfinityCrate : ModItem
    {
		public override bool IsLoadingEnabled(Mod mod)
		{
			return ModContent.GetInstance<Common.Configs.TecharriaServerConfig>().DevItemsEnabled;
		}
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Infinity Crate");

			/* Tooltip.SetDefault("Can be infinetely inserted into or extracted from\n"
				+ "Any inserted items will be cleared of any reforges or similar\n"
				+ "'Check my recipe ;)'"); */
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Misc.InfinityCrate>());
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
			// recipe.AddCondition(Language.GetText("\n"), () => true); is a new line in the recipe

			Recipe recipe = Recipe.Create(ModContent.ItemType<Placeables.InfinityCrate>());
			// requires various tiles
			recipe.AddTile(TileID.Heart);
			recipe.AddTile(TileID.DemonAltar);
			// requires water candle buff
			recipe.AddCondition(Condition.InWaterCandle);
			// requires expert mode
			recipe.AddCondition(Language.GetText("\n"), () => true);
			recipe.AddCondition(Language.GetText("No casuals"), () => Main.masterMode);
			// npc conditions
			recipe.AddCondition(Language.GetText("\n"), () => true);
			recipe.AddCondition(Language.GetText("You have talked to Angler"), () => NPC.savedAngler);
			recipe.AddCondition(Language.GetText("Goblin Tinkerer has not been saved"), () => !NPC.savedGoblin);
			// active NPC
			recipe.AddCondition(Language.GetText("\n"), () => true);
			//recipe.AddCondition(Language.GetText("Currently fighting EOL"), () => NPC.AnyNPCs(NPCID.HallowBoss));
			// boss downed conditions
			recipe.AddCondition(Language.GetText("The evil presence has been left alive"), () => !NPC.downedBoss1);
			recipe.AddCondition(Language.GetText("The Old Man is cursed"), () => !NPC.downedBoss3);
			recipe.AddCondition(Language.GetText("Golem has not been destroyed"), () => !NPC.downedGolemBoss);
			// time conditions
			recipe.AddCondition(Language.GetText("\n"), () => true);
			recipe.AddCondition(Language.GetText("Daytime"), () => Main.dayTime);
			// player conditions
			recipe.AddCondition(Language.GetText("\n"), () => true);
			recipe.AddCondition(Language.GetText("100 max hp"), () => Main.LocalPlayer.statLifeMax == 100);

			// items required
			recipe.AddIngredient(ItemID.GoldWaterStrider, 3);
			recipe.AddIngredient(ItemID.Waldo);
			// register
			recipe.Register();
		}
	}
}
