using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.Localization;

namespace Techarria.Content.Tiles
{
    /// <summary>
    /// A tile that damages blocks when powered. Default pickaxe power is 40
    /// </summary>
    public class BlockBreaker : ModTile
    {
        public static int power = 40;
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileFrameImportant[Type] = true;

            AddMapEntry(Color.DarkSlateGray, CreateMapEntryName());

            DustType = DustID.Stone;
            ItemDrop = ModContent.ItemType<Items.Placeables.BlockBreaker>();

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
            Main.LocalPlayer.PickTile(i + xOff, j + yOff, power);
        }
    }
}
