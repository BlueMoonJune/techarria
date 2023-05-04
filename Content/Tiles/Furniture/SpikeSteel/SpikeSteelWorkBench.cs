using Microsoft.Xna.Framework;
using Techarria.Content.Dusts;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;


namespace Techarria.Content.Tiles.Furniture.SpikeSteel
{
    public class SpikeSteelWorkBench : ModTile
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

            AdjTiles = new int[] { TileID.WorkBenches };
			DustType = ModContent.DustType<Spikesteel>();

			// placement
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x1);
            TileObjectData.newTile.CoordinateHeights = new[] { 18 };
            TileObjectData.addTile(Type);

            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTable);

            // map info
            LocalizedText name = CreateMapEntryName();
            AddMapEntry(new Color(200, 200, 200), Language.GetText("MapObject.WorkBench"));
        }
        public override void KillMultiTile(int x, int y, int frameX, int frameY)
        {
        }
    }
}
