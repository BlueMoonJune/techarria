using Terraria.ModLoader;
using Techarria.Content.Tiles.FluidTransfer;
using Terraria;
using Terraria.ID;
using System.IO;
using Terraria.ModLoader.IO;
using System.Collections.Generic;

namespace Techarria.Content.Items.FluidItems
{
    public class FluidBottle : FluidItem
    {
        public int storedItem;

        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = FluidTank.maxStorage;
        }

        public override void SaveData(TagCompound tag)
        {
            tag["storedItem"] = storedItem;
        }

        public override void LoadData(TagCompound tag)
        {
            storedItem = tag.Get<int>("storedItem");
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(storedItem);
        }

        public override void NetReceive(BinaryReader reader)
        {
            storedItem = reader.ReadInt32();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (storedItem != 0)
            {
                tooltips.Add(new TooltipLine(Mod, "StoredLiquid", "Storing: " + Lang.GetItemNameValue(storedItem)));
            } else
            {
                tooltips.Add(new TooltipLine(Mod, "StoredLiquid", "Storing: nothing"));
            }
            
            Player player = Main.player[Main.myPlayer];

            base.ModifyTooltips(tooltips);
        }
    }
}
