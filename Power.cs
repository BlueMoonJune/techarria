using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Techarria.Content.Tiles;
using Terraria;
using Terraria.ModLoader;

namespace Techarria
{
	public class PowerDisplayInfo
    {
        public bool Right;
        public bool Left;
        public bool Up;
        public bool Down;
        public int channel;
        public int age = 0;
        
        public PowerDisplayInfo(int c, bool r = false, bool l = false, bool u = false, bool d = false)
        {
            channel = c;
            Right = r;
            Left = l;
            Up = u;
            Down = d;
        }

        public void AddDirection(Direction dir)
        {
            if (dir == Direction.Right)
                Right = true;
            if (dir == Direction.Left)
                Left = true;
            if (dir == Direction.Up)
                Up = true;
            if (dir == Direction.Down)
                Down = true;
        }
    }

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
        public static List<Wire> scanned = new List<Wire>();

        public static Dictionary<Wire, PowerDisplayInfo> DisplayInfos = new Dictionary<Wire, PowerDisplayInfo>() { };

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

        public static List<T> Concat<T>(List<T> a, List<T> b)
        {
            List<T> ret = new List<T>();
            foreach (T obj in a)
            {
                ret.Add(obj);
            }
            foreach (T obj in b)
            {
                ret.Add(obj);
            }
            return ret;
        }

        public static bool TransferCharge(int amount, int i, int j, int width = 1, int height = 1)
        {
            scanned.Clear();
            List<Point> consumers = new List<Point>();
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int c = 0; c < 4; c++)
                    {
                        if (GetWire(Main.tile[i + x, j + y], c))
                            consumers = Concat(consumers, Search(i + x, j + y, c));
                    }
                }
            }
            if (consumers.Count() == 0)
            {
                return false;
            }
            int splitCharge = amount / consumers.Count();
            foreach (Point p in consumers)
            {
                Tile tile = Main.tile[p];
                if (ModContent.GetModTile(tile.TileType) is PowerConsumer consumer)
                    consumer.InsertPower(p.X, p.Y, splitCharge);
            }
            return true;
        }

        public static List<Point> Search(int i, int j, int channel)
        {
            return Search(new Wire(i, j, channel));
        }

        public static List<Point> Search(Wire wire)
        {
            List<Point> list = new List<Point>();
            scanned.Add(wire);
            Point p = new Point(wire.X, wire.Y);

            Tile tile = Main.tile[p];
            if (ModContent.GetModTile(tile.TileType) is PowerConsumer consumer && consumer.IsConsumer(p.X, p.Y))
            {
                list.Add(p);
            }

            PowerDisplayInfo displayInfo = new PowerDisplayInfo(wire.C);
            foreach (Direction dir in Direction.directions())
            {
                Point target = p + dir.point;
                Wire w = new Wire(target.X, target.Y, wire.C);
                tile = Main.tile[target];
                if (!scanned.Contains(w) && GetWire(tile, wire.C))
                {
                    displayInfo.AddDirection(dir);
                    list = Concat<Point>(list, Search(w));
                }
            }
            foreach (Wire point in DisplayInfos.Keys)
            {
                if (point.X == p.X && point.Y == p.Y)
                {
                    DisplayInfos[point].age = 0;
                    return list;
                }
            }
            DisplayInfos.Add(wire, displayInfo);
            return list;

        }
    }
}
