using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Techarria.Content.Items;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Techarria.Content.Tiles.Machines
{
	public class PlayerInterfaceTE : InventoryTileEntity
	{
		public static Rectangle rect = new Rectangle(16, 16, 16, 32);

		public override Item[] ExtractableItems {
			get
			{
				List<Item> list = new List<Item>();
				foreach (Player player in Main.player) {
					Rectangle offset = rect;
					rect.X += Position.X * 16;
					rect.Y += Position.Y * 16;
					if (player.getRect().Intersects(offset))
						foreach (Item item in player.inventory)
							if (!item.IsAir && !item.favorited)
								list.Add(item);
				}
				return list.ToArray();
			}
		}

        public override bool InsertItem(Item item)
        {
            Rectangle scanRect = new Rectangle(X * 16 + 16, Y * 16 + 16, 16, 32);
            foreach (Player player in Main.player)
            {
                if (player.getRect().Intersects(scanRect))
                {
                    if (item.headSlot >= 0 && player.armor[0].IsAir)
                    {
                        player.armor[0] = item.Clone();
                        player.armor[0].stack = 1;
                        return true;
                    }
                    if (item.bodySlot >= 0 && player.armor[1].IsAir)
                    {
                        player.armor[1] = item.Clone();
                        player.armor[1].stack = 1;
                        return true;
                    }
                    if (item.legSlot >= 0 && player.armor[2].IsAir)
                    {
                        player.armor[2] = item.Clone();
                        player.armor[2].stack = 1;
                        return true;
                    }
                    for (int i = 0; i < player.inventory.Length; i++)
                    {
                        Item slot = player.inventory[i];
                        if (slot.IsAir)
                        {
                            slot = item.Clone();
                            slot.stack = 1;
                            player.inventory[i] = slot;
                            return true;
                        }
                        if (slot.type == item.type && slot.stack < slot.maxStack)
                        {
                            slot.stack++;
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }

	public class PlayerInterface : EntityTile<PlayerInterfaceTE>, IPowerConsumer
	{
		public override void PreStaticDefaults() {

			Main.tileLavaDeath[Type] = false;
			Main.tileNoAttach[Type] = true;
			Main.tileFrameImportant[Type] = true;
			TileID.Sets.DisableSmartCursor[Type] = true;
			TileID.Sets.IgnoredByNpcStepUp[Type] = true; // This line makes NPCs not try to step up this tile during their movement. Only use this for furniture with solid tops.

			DustType = ModContent.DustType<Dusts.Spikesteel>();
			AdjTiles = new int[] { TileID.Tables };

			width = 3;
			height = 3;

			// Etc
			LocalizedText name = CreateMapEntryName();
			// name.SetDefault("Player Interface");
			AddMapEntry(new Color(200, 200, 200), name);
		}

		public override void NumDust(int x, int y, bool fail, ref int num) {
			num = fail ? 1 : 3;
		}

		public override void KillMultiTile(int x, int y, int frameX, int frameY) {
		}

		public void InsertPower(int i, int j, int amount) {
			Tile tile = Framing.GetTileSafely(i, j);
			i -= tile.TileFrameX / 18;
			j -= tile.TileFrameY / 18;
			var scanRect = new Rectangle(i * 16 + 16, j * 16 + 16, 16, 32);

			foreach (Player player in Main.player) {
				if (scanRect.Intersects(player.getRect())) {
					for (int c = 0; c < amount;) {
						bool founditem = false;
						foreach (Item item in player.inventory) {
							if (item.ModItem is ChargableItem chargable) {
								if (c < amount && chargable.Charge(1) == 1) {
									founditem = true;
									c++;
									continue;
								}
							}
						}

						foreach (Item item in player.armor) {
							if (item.ModItem is ChargableItem chargable) {
								if (c < amount && chargable.Charge(1) == 1) {
									founditem = true;
									c++;
									continue;
								}
							}
						}
						if (!founditem)
							break;
					}
				}
			}
		}
	}
}