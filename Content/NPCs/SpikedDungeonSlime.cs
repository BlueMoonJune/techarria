using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader.Utilities;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;

namespace Techarria.Content.NPCs
{
    public class SpikedDungeonSlime : ModNPC
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Spiked Dungeon Slime");
            Main.npcFrameCount[NPC.type] = Main.npcFrameCount[2];

            // debuff immunity
            NPCID.Sets.DebuffImmunitySets.Add(Type, new NPCDebuffImmunityData
            {
                SpecificallyImmuneTo = new int[] {
                    BuffID.Poisoned // This NPC will be immune to the Poisoned debuff.
				}
            });
        }

        public override void SetDefaults()
        {
            NPC.width = 56;
            NPC.height = 44;
            NPC.damage = 39;
            NPC.defense = 9;
            NPC.lifeMax = 165;
            NPC.value = 175f;
            NPC.aiStyle = 1;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;

            // grabbing how other enemies work
            AIType = NPCID.SpikedJungleSlime;
            AnimationType = NPCID.SpikedJungleSlime;

			// banner stuff
			Banner = NPC.type;
            BannerItem = ModContent.ItemType<SpikedDungeonSlimeBanner>();
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				// Sets the spawning conditions of this NPC that is listed in the bestiary.
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheDungeon,

				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement("These dungeon slimes met an unfortunate fate trying to hop the spike-filled pitfalls commonplace of the dungeon. " +
                "Watch out as they try and free themselves of their burden"),
            });
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return SpawnCondition.DungeonNormal.Chance * 0.05f;
        }

        // loot table
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ItemID.GoldenKey, 1));
            npcLoot.Add(ItemDropRule.Common(ItemID.Spike, 1, 5, 12));
        }
    }

	public class SpikedDungeonSlimeBanner : ModItem {
		public override void SetStaticDefaults() {
			// DisplayName.SetDefault("Industrial Coal");
			// Tooltip.SetDefault("'Even less festive than normal'");

			// journey mode
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 200;

			ItemID.Sets.SortingPriorityMaterials[Item.type] = 58;
		}
		public override void SetDefaults() {
			Item.width = 22; // The item texture's width
			Item.height = 22; // The item texture's height

			Item.maxStack = 999; // The item's max stack value
								 // The value of the item in copper coins. Item.buyPrice & Item.sellPrice are helper methods that returns costs in copper coins based on
								 // platinum/gold/silver/copper arguments provided to it.
			Item.value = Item.buyPrice(silver: 1, copper: 25);
		}
	}
}
