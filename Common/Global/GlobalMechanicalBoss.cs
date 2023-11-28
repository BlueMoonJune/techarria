using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Techarria.Content.Items.Materials;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Techarria.Common.Global
{
    public class GlobalMechanicalBossLootBag : GlobalItem
    {
        public static CommonDrop mechanicalScrapRule = new CommonDrop(ModContent.ItemType<MechanicalScrap>(), 1, 2, 4);

        public override void ModifyItemLoot(Item item, ItemLoot itemLoot)
        {
            if (item.type == ItemID.TwinsBossBag || item.type == ItemID.SkeletronPrimeBossBag || item.type == ItemID.DestroyerBossBag)
            {
                int _ = 0;
                itemLoot.RemoveWhere(r => {
                    Main.NewText("woah");
                    if (r is CommonDrop c)
                    {
                        return c.itemId == ItemID.MechanicalBatteryPiece || c.itemId == ItemID.MechanicalWheelPiece || c.itemId == ItemID.MechanicalWagonPiece;
                    }
                    return false;
                }, false);
                itemLoot.Add(mechanicalScrapRule);
            }
        }
    }

    public class GlobalMechanicalBoss : GlobalNPC
    {
        public static DropBasedOnMasterAndExpertMode mechanicalScrapRule = new DropBasedOnMasterAndExpertMode(new CommonDrop(ModContent.ItemType<MechanicalScrap>(), 1, 2, 4), new DropNothing(), new DropNothing());

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            if (npc.type == NPCID.Retinazer || npc.type == NPCID.Spazmatism || npc.type == NPCID.SkeletronPrime || npc.type == NPCID.TheDestroyer)
            {
                npcLoot.Add(mechanicalScrapRule);
            }
        }
    }
}
