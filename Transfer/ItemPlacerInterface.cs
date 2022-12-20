using System.Collections;
using Terraria;
using Terraria.ObjectData;
using Terraria.ModLoader;
using techarria.Content.Tiles;
using System.Collections.Generic;

namespace Techarria.Transfer
{
    internal class ItemPlacerInterface : ContainerInterface
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
            array.Add(Techarria.itemPlacerItems[Techarria.itemPlacerIDs[x, y]]);
            return array;
        }

        public override bool InsertItem(Item item)
        {
            Item myItem = Techarria.itemPlacerItems[Techarria.itemPlacerIDs[x, y]];
            if (myItem == null || myItem.IsAir)
            {
                myItem = item.Clone();
                myItem.stack = 1;
                Techarria.itemPlacerItems[Techarria.itemPlacerIDs[x, y]] = myItem;
                return true;
            }
            if (myItem.type == item.type && myItem.stack < myItem.maxStack)
            {
                myItem.stack++;
                return true;
            }

            return false;
        }
    }
}
