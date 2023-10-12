using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

using TE = Techarria.Content.Tiles.StorageCrates.InfinityCrateTE;

namespace Techarria.Content.Tiles.StorageCrates
{
    public class InfinityCrateTE : CrateEntity
    {
        public override void Update()
        {
            item.stack = 99999;
        }
    }

    // Where the TE ends and the Tile starts
    public class InfinityCrate : CrateTile<TE>
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
                Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 32, item.type, item.maxStack);
                tileEntity.item.TurnToAir();
            }

            base.KillTile(i, j, ref fail, ref effectOnly, ref noItem);
        }
        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
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
                player.cursorItemIconText = "∞";
                player.cursorItemIconID = item.type;
            }
        }
    }
}
