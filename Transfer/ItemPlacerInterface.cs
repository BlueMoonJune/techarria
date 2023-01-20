using System.Collections;
using Terraria;
using Terraria.ObjectData;
using Terraria.ModLoader;
using techarria.Content.Tiles;
using System.Collections.Generic;

namespace Techarria.Transfer
{
    public class ItemPlacerInterface : ContainerInterface
    {
        public ItemPlacerInterface(int i, int j)
        {
            x = i;
            y = j;
        }

        public static bool Check(int i, int j)
        {
            return Main.tile[i, j].TileType == ModContent.TileType<ItemPlacer>();
        }

        public override List<Item> GetItems()
        {
            List<Item> array = new List<Item>();
            array.Add(ItemPlacer.GetTileEntity(x, y).item);
            return array;
        }

        public override bool InsertItem(Item item)
        {
            ItemPlacerTE tileEntity = ItemPlacer.GetTileEntity(x, y);
            Item myItem = tileEntity.item;
            if (myItem == null || myItem.IsAir)
            {
                myItem = item.Clone();
                myItem.stack = 1;
                tileEntity.item = myItem;
                decrementItem(item);
                return true;
            }
            if (myItem.type == item.type && myItem.stack < myItem.maxStack)
            {
                myItem.stack++;
                decrementItem(item);
                return true;
            }

            return false;
        }
    }
}
