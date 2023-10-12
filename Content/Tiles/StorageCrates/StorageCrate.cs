using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;

using TE = Techarria.Content.Tiles.StorageCrates.StorageCrateTE;

namespace Techarria.Content.Tiles.StorageCrates
{
    public class StorageCrateTE : CrateEntity
    {
    }

    // Where the TE ends and the Tile starts
    public class StorageCrate : CrateTile<TE>
    {
        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            TE tileEntity = GetTileEntity(i, j);
            Item item = tileEntity.item;
            if (!item.IsAir)
            {
                fail = true;
                int amount = Math.Min(item.maxStack, item.stack);
                item.stack -= amount;
                Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 32, item.type, amount);
                if (item.stack <= 0)
                {
                    item.TurnToAir();
                }
                return;
            }
        }

        public override void MouseOver(int i, int j)
        {
            TE tileEntity = GetTileEntity(i, j);
            Item item = tileEntity.item;
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            if (item != null && !item.IsAir)
            {
                player.cursorItemIconEnabled = true;
                player.cursorItemIconText = item.stack.ToString("#,# /") + maxStorage.ToString(" #,#");
                player.cursorItemIconID = item.type;
            }
        }
    }
}
