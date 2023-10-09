using Microsoft.Xna.Framework;
using Techarria.Content.Dusts;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace techarria.Content.Tiles.Furniture.WorkStations
{
    public class SoulGrinder : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileLavaDeath[Type] = false;
            Main.tileTable[Type] = true;
            Main.tileSolidTop[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileFrameImportant[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;
            TileID.Sets.IgnoredByNpcStepUp[Type] = true;

            DustType = ModContent.DustType<Spikesteel>();

            // placement
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
            //TileObjectData.newTile.CoordinateHeights = new[] { 18 };
            TileObjectData.addTile(Type);

            // map info
            LocalizedText name = CreateMapEntryName();
            AddMapEntry(new Color(200, 200, 200), Language.GetText("MapObject.WorkBench"));
        }
        public override void KillMultiTile(int x, int y, int frameX, int frameY)
        {
        }
    }
}
