using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace Techarria.Content.Tiles
{
    public class EmptyTE : ModTileEntity
    {
        public override bool IsTileValidForEntity(int x, int y)
        {
            return false;
        }
    }
}
