using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Techarria.Content.Dusts;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;
using Terraria.GameContent;
using Techarria.Common.Global;

namespace Techarria.Content.Tiles.Machines
{
    public class Electromagnet : PowerConsumer<EmptyTE>
    {
        public override void SetStaticDefaults()
        {
            Main.tileLavaDeath[Type] = false;
            Main.tileNoAttach[Type] = true;
            Main.tileFrameImportant[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;
            TileID.Sets.IgnoredByNpcStepUp[Type] = true; // This line makes NPCs not try to step up this tile during their movement. Only use this for furniture with solid tops.

            DustType = ModContent.DustType<Spikesteel>();
            AdjTiles = new int[] { TileID.Tables };

            // Placement
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.addTile(Type);

            // Etc
            LocalizedText name = CreateMapEntryName();
            // name.SetDefault("Electromagnet");
            AddMapEntry(new Color(200, 200, 200), name);
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
        }

		public override void InsertPower(int i, int j, int amount) {
			Tile tile = Framing.GetTileSafely(i, j);
			i -= tile.TileFrameX / 18 % 3;
			j -= tile.TileFrameY / 18 % 3;

			Rectangle rect = new Rectangle(i * 16, j * 16, 48, 48);
			Vector2 center = new Vector2(i * 16 + 24, j * 16 + 24);
			foreach (Item item in Main.item) {
				if (item.Center == Vector2.Zero) {
					continue;
				}
				Vector2 dif = item.Center - center;
				float dist = dif.LengthSquared();
				if (dist < amount * 65536.0 && !item.getRect().Intersects(rect)) {
					dif.SafeNormalize(Vector2.Zero);
					Vector2 force = dif * amount * 4f / dist;
					item.velocity -= force;
					item.position -= force;
                    item.GetGlobalItem<GlobalMagnetItem>().magentized = true;
				}
			}
		}
	}
}
