using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.Localization;
using Techarria;
using Terraria.DataStructures;

namespace techarria.Content.Tiles
{
    internal class ItemPlacer : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileFrameImportant[Type] = true;

            AddMapEntry(Color.Blue, CreateMapEntryName());

            DustType = DustID.Stone;
            ItemDrop = ModContent.ItemType<Items.Placeables.ItemPlacer>();

            HitSound = SoundID.Tink;
        }

        public override bool Slope(int i, int j)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            tile.TileFrameX = (short)((tile.TileFrameX + 16) % 64);
            return false;
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            return false;
        }

        public override void HitWire(int i, int j)
        {
            int xOff = 0;
            int yOff = 0;
            Tile tile = Framing.GetTileSafely(i, j);
            if (tile.TileFrameX == 0) {
                xOff = 1;
            } else if (tile.TileFrameX == 16) {
                yOff = -1;
            } else if (tile.TileFrameX == 32) {
                xOff = -1;
            } else {
                yOff = 1;
            }
            WorldGen.PlaceTile(i + xOff, j + yOff, 31);
        }
        public override void PlaceInWorld(int i, int j, Item item)
        {
            base.PlaceInWorld(i, j, item);
            for (int x = 0; x < 2048; x++)
            {
                if (Techarria.Techarria.itemPlacerPositions[x] == Point.Zero)
                {
                    Techarria.Techarria.itemPlacerPositions[x] = new Point(i, j);
                    Techarria.Techarria.itemPlacerIDs[i, j] = x;
                    return;
                }
            }
        }

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            base.KillTile(i, j, ref fail, ref effectOnly, ref noItem);
            if (Techarria.Techarria.itemPlacerIDs[i, j] >= 0)
            {
                Techarria.Techarria.itemPlacerPositions[Techarria.Techarria.itemPlacerIDs[i, j]] = Point.Zero;
                Techarria.Techarria.itemPlacerIDs[i, j] = -1;
            }
        }

        public override bool RightClick(int i, int j)
        {
            Item item = Techarria.Techarria.itemPlacerItems[Techarria.Techarria.itemPlacerIDs[i, j]];
            Item playerItem = Main.player[Main.myPlayer].HeldItem;
            if (item == null)
            {
                item = new Item();
                item.TurnToAir();
            }
            if (playerItem.IsAir)
            {
                if (!item.IsAir)
                {
                    Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 32, item.type);
                    item.stack--;
                    if (item.stack <= 0)
                    {
                        item.TurnToAir();
                    }
                }
            } else {
                if (item.IsAir)
                {
                    item.type = playerItem.type;
                    item.stack = 1;
                } else if ((item.type == playerItem.type) && (item.stack < item.maxStack))
                {
                    item.stack++;
                    playerItem.stack--;
                    if (playerItem.stack <= 0)
                    {
                        playerItem.TurnToAir();
                    }
                }
            }
            return true;
        }
        public override void MouseOver(int i, int j)
        {
            Item item = Techarria.Techarria.itemPlacerItems[Techarria.Techarria.itemPlacerIDs[i, j]];
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            if ((item != null) && (item.type != 0))
            {
                player.cursorItemIconEnabled = true;
                player.cursorItemIconText = ""+item.stack;
                player.cursorItemIconID = item.type;
            }
        }
    }
}
