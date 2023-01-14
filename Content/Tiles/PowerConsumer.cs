using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace Techarria.Content.Tiles
{
    internal abstract class PowerConsumer : ModTile
    {
        public abstract int InsertPower(int i, int j, int amount);
    }
}
