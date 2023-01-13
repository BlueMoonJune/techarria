using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria.ModLoader.IO;

namespace Techarria.Content.Items
{
    internal abstract class ChargableItem : ModItem
    {
        public int charge = 0;
        public int maxcharge = 0;

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (charge == 0)
            {
                tooltips.Add(new TooltipLine(Mod, "Depleted", "[c/FF0000:No Charge!]"));
            }
            tooltips.Add(new TooltipLine(Mod, "Charge", "Current charge: " + charge + "/" + maxcharge));
        }

        public virtual int Charge(int amount)
        {
            if (charge >= maxcharge)
            {
                return 0;
            }
            if (charge + amount >= maxcharge)
            {
                int change = maxcharge - charge;
                charge = maxcharge;
                return change;
            }
            charge += amount;
            return amount;
        }

        public virtual int Deplete(int amount)
        {
            if (charge <= 0)
            {
                return 0;
            }
            if (charge - amount <= 0)
            {
                int change = charge;
                charge = 0;
                return change;
            }
            charge -= amount;
            return amount;
        }

        public override void SaveData(TagCompound tag)
        {
            tag.Add("charge", charge);
            base.SaveData(tag);
        }

        public override void LoadData(TagCompound tag)
        {
            if (tag.ContainsKey("charge"))
                charge = tag.GetAsInt("charge");
            base.LoadData(tag);
        }
    }
}
