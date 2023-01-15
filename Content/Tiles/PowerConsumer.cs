using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace Techarria.Content.Tiles
{
    public abstract class PowerConsumer : ModTile
    {
        public virtual bool IsConsumer(int i, int j)
        {
            return true;
        }

        public abstract void InsertPower(int i, int j, int amount);
    }
}
