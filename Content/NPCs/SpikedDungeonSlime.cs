using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader.Utilities;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;

namespace Techarria.Content.NPCs
{
    public class SpikedDungeonSlime : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spiked Dungeon Slime");
            Main.npcFrameCount[NPC.type] = Main.npcFrameCount[2];
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
            Banner = Item.NPCtoBanner(NPCID.DungeonSlime);
            BannerItem = Item.BannerToItem(Banner);
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
            return SpawnCondition.DungeonNormal.Chance * 0.1f;
        }
        /* // probably not useful
        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter >= 20)
            {
                NPC.frameCounter = 0;
            }
            NPC.frame.Y = (int)NPC.frameCounter / 10 * frameHeight;
        }
        */
        // loot table
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ItemID.GoldenKey, 1));
            npcLoot.Add(ItemDropRule.Common(ItemID.Spike, 1, 5, 12));
        }
    }
}
