using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Techarria.Content.Tiles.Machines;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Techarria.Transfer
{
	internal class PlayerInterfaceInterface : ContainerInterface
    {
        public PlayerInterfaceInterface(int i, int j)
        {
            x = i;
            y = j;
        }

        public static Point FindTopLeft(int i, int j)
        {
            if (i < 0 || j < 0)
            {
                return new Point();
            }
            Tile tile = Main.tile[i, j];
            if (tile.TileType == ModContent.TileType<PlayerInterface>())
            {
                i -= tile.TileFrameX / 18 % 3;
                j -= tile.TileFrameY / 18 % 3;

                return new Point(i, j);
            }
            return new Point();
        }

        public override List<Item> GetItems()
        {
            Rectangle scanRect = new Rectangle(x * 16 + 16, y * 16 + 16, 16, 32);
            foreach (Player player in Main.player)
            {
                if (player.getRect().Intersects(scanRect))
                {
                    List<Item> items = new(player.inventory);
                    for (int i = 0; i < items.Count; i++) {
                        if (items[i].IsAir)
                        {
                            items.RemoveAt(i);
                            i--;
                        }
                    }
                    return items;
                }
            }
            return new();
        }

        public override bool InsertItem(Item item)
        {
            Rectangle scanRect = new Rectangle(x * 16 + 16, y * 16 + 16, 16, 32);
            foreach (Player player in Main.player)
            {
                if (player.getRect().Intersects(scanRect))
                {
                    if (item.headSlot >= 0 && player.armor[0].IsAir)
                    {
                        player.armor[0] = item.Clone();
                        player.armor[0].stack = 1;
                        return true;
                    }
                    if (item.bodySlot >= 0 && player.armor[1].IsAir)
                    {
                        player.armor[1] = item.Clone();
                        player.armor[1].stack = 1;
                        return true;
                    }
                    if (item.legSlot >= 0 && player.armor[2].IsAir)
                    {
                        player.armor[2] = item.Clone();
                        player.armor[2].stack = 1;
                        return true;
                    }
                    for (int i = 0; i < player.inventory.Length; i++)
                    {
                        Item slot = player.inventory[i];
                        if (slot.IsAir)
                        {
                            slot = item.Clone();
                            slot.stack = 1;
                            player.inventory[i] = slot;
                            return true;
                        }
                        if (slot.type == item.type && slot.stack < slot.maxStack)
                        {
                            slot.stack++;
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}
