using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader.Utilities;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ObjectData;
using Terraria.Enums;
using Microsoft.Xna.Framework;
using System;
using Microsoft.CodeAnalysis;

namespace Techarria.Content.NPCs
{
    public class SpikedDungeonSlime : ModNPC
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Spiked Dungeon Slime");
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

	public class SpikedDungeonSlimeGlobalProjectile : GlobalProjectile
	{
        public override bool AppliesToEntity(Projectile entity, bool lateInstantiation)
        {
			return entity.type == ProjectileID.JungleSpike;
        }

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if (source is EntitySource_Parent p && p.Entity is NPC npc && npc.ModNPC is SpikedDungeonSlime)
            {
                Projectile.NewProjectile(source, projectile.position, projectile.velocity, ModContent.ProjectileType<SpikedDungeonSlimeProjectile>(), projectile.damage, projectile.knockBack);
                projectile.Kill();
            }
        }
    }

	public class SpikedDungeonSlimeProjectile : ModProjectile
	{
        public override void SetDefaults()
        {
			AIType = ProjectileID.JungleSpike;
            Projectile.aiStyle = 1;
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = true;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.Bleeding, 600);
        }
    }

	//Banner code from https://github.com/HolyDrillDev/DarknessFallenMod
	public class SpikedDungeonSlimeBanner : ModItem {
		public override void SetStaticDefaults() {
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}
		public override void SetDefaults()
		{
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTurn = true;
			Item.useAnimation = 15;
			Item.useTime = 10;
			Item.autoReuse = true;
			Item.maxStack = 999;
			Item.consumable = true;
			Item.createTile = ModContent.TileType<SpikedDungeonSlimeBannerTile>();
			Item.width = 10;
			Item.height = 24;
			Item.value = 500;
			Item.rare = ItemRarityID.Blue;
		}
    }


	public class SpikedDungeonSlimeBannerTile : ModTile
	{
		public override void SetStaticDefaults() {
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2Top);
			TileObjectData.newTile.Height = 3;
			TileObjectData.newTile.CoordinateHeights = new[] { 16, 16, 16 };
			TileObjectData.newTile.DrawYOffset = -2;
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide | AnchorType.SolidBottom, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.StyleWrapLimit = 111;
			TileObjectData.addTile(Type);
			AddMapEntry(Color.Purple, CreateMapEntryName());
		}

		public override void NearbyEffects(int i, int j, bool closer) {
			if (closer) {
				int type = ModContent.NPCType<SpikedDungeonSlime>();

				Main.SceneMetrics.hasBanner = true;
				Main.SceneMetrics.NPCBannerBuff[type] = true;
			}
		}

		public override void KillMultiTile(int i, int j, int frameX, int frameY) {
			//Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 16, 48, ModContent.ItemType<SpikedDungeonSlimeBanner>(), 1);
		}
	}
}
