using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Techarria
{
    public class RecipeGroups : ModSystem
    {
        public override void AddRecipeGroups()
        {
            // every non-biome chest as a recipe group
            // Legacy.Misc37 is "Any"
            RecipeGroup ChestRecipeGroup = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.Chest)}",
            ItemID.Chest, ItemID.GoldChest, ItemID.FrozenChest, ItemID.IvyChest, ItemID.LihzahrdChest, ItemID.LivingWoodChest, ItemID.MushroomChest, 
                ItemID.RichMahoganyChest, ItemID.DesertChest, ItemID.SkywareChest, ItemID.WaterChest, ItemID.WebCoveredChest, ItemID.GraniteChest,
                ItemID.MarbleChest, ItemID.ShadowChest, ItemID.GoldenChest, ItemID.GolfChest, ItemID.NebulaChest, ItemID.SolarChest, ItemID.VortexChest,
                ItemID.BoneChest, ItemID.LesionChest, ItemID.FleshChest, ItemID.GlassChest, ItemID.HoneyChest, ItemID.SlimeChest, ItemID. SteampunkChest,
                ItemID.BambooChest, ItemID.BlueDungeonChest, ItemID.BorealWoodChest, ItemID.CactusChest, ItemID.CrystalChest, ItemID.DynastyChest,
                ItemID.EbonwoodChest, ItemID.GreenDungeonChest, ItemID.MartianChest, ItemID.MeteoriteChest, ItemID.ObsidianChest, ItemID.PalmWoodChest,
                ItemID.PearlwoodChest, ItemID.PinkDungeonChest, ItemID.PumpkinChest, ItemID.ShadewoodChest, ItemID.SpiderChest, ItemID.SpookyChest);
            RecipeGroup.RegisterGroup(nameof(ItemID.Chest), ChestRecipeGroup);
        }
    }
}
