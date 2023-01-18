using Microsoft.Xna.Framework;
using Techarria.Content.Items;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Techarria.Content.Tiles
{
	public class PlayerInterface : PowerConsumer
	{
		public override void SetStaticDefaults()
		{
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;
			Main.tileFrameImportant[Type] = true;
			TileID.Sets.DisableSmartCursor[Type] = true;
			TileID.Sets.IgnoredByNpcStepUp[Type] = true; // This line makes NPCs not try to step up this tile during their movement. Only use this for furniture with solid tops.

			DustType = ModContent.DustType<Dusts.Wormhole>();
			AdjTiles = new int[] { TileID.Tables };

			// Placement
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
			TileObjectData.addTile(Type);

			// Etc
			ModTranslation name = CreateMapEntryName();
			name.SetDefault("Player Interface");
			AddMapEntry(new Color(200, 200, 200), name);
		}

		public override void NumDust(int x, int y, bool fail, ref int num)
		{
			num = fail ? 1 : 3;
		}

		public override void KillMultiTile(int x, int y, int frameX, int frameY)
		{
			Item.NewItem(new EntitySource_TileBreak(x, y), x * 16, y * 16, 48, 32, ModContent.ItemType<Items.Placeables.ExampleTable>());
		}

        public override void InsertPower(int i, int j, int amount)
        {
			Tile tile = Framing.GetTileSafely(i, j);
			i -= tile.TileFrameX / 18;
			j -= tile.TileFrameY / 18;
			Rectangle scanRect = new Rectangle(i * 16 + 16, j * 16 + 16, 16, 32);

			foreach (Player player in Main.player)
            {
				if (scanRect.Intersects(player.getRect()))
                {
					for (int c = 0; c < amount;)
					{
						bool founditem = false;
						foreach (Item item in player.inventory)
						{
							if (item.ModItem is ChargableItem chargable)
							{
								if (c < amount && chargable.Charge(1) == 1)
								{
									founditem = true;
									c++;
									continue;
								}
							}
						}

						foreach (Item item in player.armor)
						{
							if (item.ModItem is ChargableItem chargable)
							{
								if (c < amount && chargable.Charge(1) == 1)
								{
									founditem = true;
									c++;
									continue;
								}
							}
						}
						if (!founditem) break;
					}
				}
            }
        }
    }
}