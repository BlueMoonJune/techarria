using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace techarria.Common.Global
{
    internal class GlobalMagnetItem : GlobalItem
    {
        public override bool InstancePerEntity => true;

        public bool magentized = false;

        public override void Update(Item item, ref float gravity, ref float maxFallSpeed)
        {
            if (magentized)
            {
                if (item.velocity.Y == 0)
                {
                    magentized = false;
                    return;
                }
                item.velocity.X /= 0.95f;
            }
        }
    }
}
