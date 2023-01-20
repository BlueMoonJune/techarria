using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Techarria.Content.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Techarria
{
    public class Wire
    {
        public int X;
        public int Y;
        public int C;

        public Wire(int i, int j, int channel)
        {
            X = i;
            Y = j;
            C = channel;
        }

        public Wire(Point p, int channel)
        {
            X = p.X;
            Y = p.Y;
            C = channel;
        }

        public override bool Equals(object obj)
        {
            if (obj is Wire wire)
            {
                return (wire.X == X) && (wire.Y == Y) && (wire.C == C);
            }
            return false;
        }
    }

    public class Power
    {

        public static byte GetWire(Tile tile)
        {
            byte val = 0;
            if (tile.RedWire) val += 1;
            if (tile.BlueWire) val += 2;
            if (tile.GreenWire) val += 4;
            if (tile.YellowWire) val += 8;
            return val;
        }

        public static bool GetWire(Tile tile, int channel)
        {
            switch (channel)
            {
                case 0: return tile.RedWire;
                case 1: return tile.BlueWire;
                case 2: return tile.GreenWire;
                case 3: return tile.YellowWire;
                default: return false;
            }
        }
    }
}
