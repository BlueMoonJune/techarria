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
            
            Main.NewText("Techarria.Techarria.itemPlacerIDs[i, j]");
            Item item = Techarria.Techarria.itemPlacerItems[Techarria.Techarria.itemPlacerIDs[i, j]];
            if (item == null) { return; }
            Main.NewText(item.type);
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
            if (item == null)
            {
                item = new Item();
                item.TurnToAir();
                Techarria.Techarria.itemPlacerItems[Techarria.Techarria.itemPlacerIDs[i, j]] = item;
            }
            if (item.createTile > -1 && WorldGen.PlaceTile(i + xOff, j + yOff, item.createTile)) {
                item.stack--;
            } else if (item.createTile <= -1)
            {
                Main.item[Item.NewItem(new EntitySource_TileBreak(i, j), i * 16 - 8, j * 16 - 8, 32, 32, item.type)].velocity = new Vector2(xOff * 5, yOff * 5 - 1);
                
                item.stack--;
            }

            if (item.stack <= 0)
            {
                item.TurnToAir();
                item.createTile = -1;
            }
        }
        public override void PlaceInWorld(int i, int j, Item item)
        {
            base.PlaceInWorld(i, j, item);
            for (int x = 0; x < 2048; x++)
            {
                if (Techarria.Techarria.itemPlacerPositions[x] == Point.Zero)
                {
                    Main.NewText("Placed ItemPlacer " + x);
                    Techarria.Techarria.itemPlacerPositions[x] = new Point(i, j);
                    Techarria.Techarria.itemPlacerIDs[i, j] = x;

                    Item myItem = Techarria.Techarria.itemPlacerItems[Techarria.Techarria.itemPlacerIDs[i, j]];
                    if (myItem == null)
                    {
                        Main.NewText("Item was null");
                        myItem = new Item();
                        myItem.TurnToAir();
                        Techarria.Techarria.itemPlacerItems[Techarria.Techarria.itemPlacerIDs[i, j]] = myItem;
                    }
                    return;
                }
            }
        }

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            base.KillTile(i, j, ref fail, ref effectOnly, ref noItem);
            if (effectOnly || noItem || fail) { return; }
            Item item = Techarria.Techarria.itemPlacerItems[Techarria.Techarria.itemPlacerIDs[i, j]];
            if (item != null) {
                Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 32, item.type, item.stack);
            }
            if (Techarria.Techarria.itemPlacerIDs[i, j] >= 0)
            {
                Techarria.Techarria.itemPlacerPositions[Techarria.Techarria.itemPlacerIDs[i, j]] = Point.Zero;
                Techarria.Techarria.itemPlacerItems[Techarria.Techarria.itemPlacerIDs[i, j]] = null;
                Techarria.Techarria.itemPlacerIDs[i, j] = -1;
            }
        }

        public override bool RightClick(int i, int j)
        {
            Item item = Techarria.Techarria.itemPlacerItems[Techarria.Techarria.itemPlacerIDs[i, j]];
            Item playerItem;
            if (Main.mouseItem != null && !Main.mouseItem.IsAir)
            {
                playerItem = Main.mouseItem;
            } 
            else
            {
                playerItem = Main.player[Main.myPlayer].HeldItem;
            }
            if (!Main.mouseItem.IsAir)
            {

            }
            if (item == null)
            {
                item = new Item();
                item.TurnToAir();
                Techarria.Techarria.itemPlacerItems[Techarria.Techarria.itemPlacerIDs[i, j]] = item;
            }
            if (playerItem.type != item.type && !item.IsAir)
            {
                Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 32, item.type);
                item.stack--;
                if (item.stack <= 0)
                {
                    item.TurnToAir();
                }
            } else
            {
                Main.NewText("Player's Item isn't air");
                if (item.IsAir)
                {
                    item = playerItem.Clone();
                    Techarria.Techarria.itemPlacerItems[Techarria.Techarria.itemPlacerIDs[i, j]] = item;
                    item.stack = 1;
                    playerItem.stack--;
                } else if (item.type == playerItem.type)
                {
                    Main.NewText("Not empty, incrementing stack size");
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
            int id = Techarria.Techarria.itemPlacerIDs[i, j];
            Main.NewText(id);
            Item item = Techarria.Techarria.itemPlacerItems[id];
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            if ((item != null) && (!item.IsAir))
            {
                player.cursorItemIconEnabled = true;
                player.cursorItemIconText = ""+item.stack;
                player.cursorItemIconID = item.type;
            }
        }
    }
}
