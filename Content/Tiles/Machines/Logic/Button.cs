using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;

namespace Techarria.Content.Tiles.Machines.Logic
{
    public class Button : ModTile
    {
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileLavaDeath[Type] = false;
			TileID.Sets.FramesOnKillWall[Type] = true;

			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			TileObjectData.newTile.AnchorBottom = AnchorData.Empty;
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.AnchorWall = true;
			TileObjectData.addTile(Type);

			// map entry
			LocalizedText name = CreateMapEntryName();
			// name.SetDefault("Button");
			AddMapEntry(new Color(200, 200, 200), name);
		}
		public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{
			Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 32, ModContent.ItemType<Items.Placeables.Machines.Logic.Button>());
		}

        public override bool RightClick(int i, int j)
        {
			Tile tile = Framing.GetTileSafely(i, j);

			if (tile.TileFrameX == 18)
			{
				i -= 1;
			}
			if (tile.TileFrameY == 18)
            {
				j -= 1;
            } 

			Wiring.TripWire(i, j, 2, 2);
			
			return true;
        }

        public override void MouseOver(int i, int j)
        {
			Player player = Main.LocalPlayer;
			player.noThrow = 2;

			player.cursorItemIconEnabled = true;
			player.cursorItemIconID = ModContent.ItemType<Items.Placeables.Machines.Logic.Button>();
		}
    }
}
