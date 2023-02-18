using Microsoft.Xna.Framework;
using System;
using Techarria.Content.Dusts;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;

namespace Techarria.Content.Tiles.Machines
{
	public class PowerBlock : ModTile
    {
		public static int POWERBLOCK_GENERATION = 50000;
		public override void SetStaticDefaults()
		{
			Main.tileLavaDeath[Type] = false;
			Main.tileNoAttach[Type] = true;
			Main.tileFrameImportant[Type] = true;
			TileID.Sets.DisableSmartCursor[Type] = true;
			TileID.Sets.IgnoredByNpcStepUp[Type] = true;

			// Etc
			ModTranslation name = CreateMapEntryName();
			name.SetDefault("PowerBlock");
			AddMapEntry(new Color(200, 200, 200), name);
		}

		// a little more protection than most dev items, for obvious reasons
        public override void HitWire(int i, int j)
        {
			if (ModContent.GetInstance<Common.Configs.TecharriaServerConfig>().DevItemsEnabled)
            {
				Power.TransferCharge(POWERBLOCK_GENERATION, i, j);
			}
		}

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
			Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 16, ModContent.ItemType<Items.Placeables.Machines.PowerBlock>());
		}
    }
}
