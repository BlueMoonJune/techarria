using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Techarria.Content.Tiles;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Techarria.Transfer
{
    public class BlastFurnaceInterface : ContainerInterface
    {
        public BlastFurnaceInterface(int i, int j)
        {
            x = i;
            y = j;
        }

        public static Point FindTopLeft(int i, int j)
        {
            if (Main.tile[i, j].TileType != ModContent.TileType<BlastFurnace>())
                return new Point();
            return new Point(i, j);
        }

        public override List<Item> GetItems()
        {
            if (Main.tile[x, y].TileType != ModContent.TileType<BlastFurnace>())
                return new List<Item>();
            BlastFurnaceTE tileEntity = BlastFurnace.GetTileEntity(x, y);
            Point16 subTile = new Point16(x, y) - tileEntity.Position;

            if (subTile.X == 1 && subTile.Y <= 2)
                return tileEntity.inputs;
            if (subTile.X != 1 && subTile.Y == 3)
            {
                Recipe recipe = tileEntity.GetRecipe();
                if (recipe != null)
                {
                    return new List<Item>() { tileEntity.GetRecipe().createItem };
                }
            }
            return new List<Item>();
        }

        public override bool InsertItem(Item item)
        {
            if (Main.tile[x, y].TileType != ModContent.TileType<BlastFurnace>())
                return false;
            BlastFurnaceTE tileEntity = BlastFurnace.GetTileEntity(x, y);
            Point16 subTile = new Point16(x, y) - tileEntity.Position;

            if (subTile.X == 1 && subTile.Y <= 1)
            {
                return tileEntity.InsertItem(item);
            }

            return false;
        }

        public override bool ExtractItem(Item item)
        {
            if (Main.tile[x, y].TileType != ModContent.TileType<BlastFurnace>())
                return false;
            BlastFurnaceTE tileEntity = BlastFurnace.GetTileEntity(x, y);
            Point16 subTile = new Point16(x, y) - tileEntity.Position;

            if (subTile.X == 1 && subTile.Y <= 2)
                foreach (Item input in tileEntity.inputs)
                    if (input.type == item.type)
                        if (--input.stack <= 0)
                            return true;

            if (subTile.X != 1 && subTile.Y == 3)
            {
                Recipe recipe = tileEntity.GetRecipe();
                if (recipe != null && item.type == recipe.createItem.type)
                {
                    tileEntity.Craft();
                    return true;
                }
            }

            return false;
        }
    }
}
