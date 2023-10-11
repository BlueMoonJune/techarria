using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Techarria.Content.Tiles.Misc;
using Terraria.DataStructures;
using Terraria;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Microsoft.Xna.Framework;

namespace Techarria.Content.Tiles
{
    public abstract class EntityTile : ModTile
    {
        public static Dictionary<Type, EntityTile> te2Tile = new();
        public static Dictionary<int, Type> tileType2TE = new();
        public static Dictionary<int, EntityTile> tileType2Tile = new();

        public int width = 1;
        public int height = 1;

        public Point16 GetTopLeft(int i, int j)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            i -= tile.TileFrameX / 18 % width;
            j -= tile.TileFrameY / 18 % height;
            return new Point16(i, j);
        }
    }

    public abstract class EntityTile<T> : EntityTile where T : ModTileEntity
    {

        public override void SetStaticDefaults()
        {
            Console.WriteLine($"Loading EntityTile<{typeof(T)}>");
            te2Tile.Add(typeof(T), this);
            tileType2TE.Add(Type, typeof(T));
            tileType2Tile.Add(Type, this);

            PreStaticDefaults();
            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.Width = width;
            TileObjectData.newTile.Height = height;
            TileObjectData.newTile.CoordinateHeights = new int[height];
            for (int i = 0; i < height; i++) {
                TileObjectData.newTile.CoordinateHeights[i] = 16;
            }
            ModifyTileObjectData();
            TileObjectData.addTile(Type);
        }

        public virtual void PreStaticDefaults() { }

        public virtual void ModifyTileObjectData() { }

        public T GetTileEntity(int i, int j)
        {
            TileEntity.ByPosition.TryGetValue(GetTopLeft(i, j), out TileEntity te);
            return te as T;
        }

        public override void PlaceInWorld(int i, int j, Item item)
        {
            ModContent.GetInstance<T>().Place(i, j);
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            ModContent.GetInstance<T>().Kill(i, j);
        }
    }
}
