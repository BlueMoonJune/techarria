using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Terraria.ModLoader.IO;

namespace Techarria.Content.Tiles.Machines.Logic
{
	public class ItemPlacerTE : ModTileEntity
	{
		public Item item = new();
		public override bool IsTileValidForEntity(int x, int y) {
			return Main.tile[x, y].TileType == ModContent.TileType<ItemPlacer>();
		}

		public override void SaveData(TagCompound tag) {
			tag.Add("item", item);
			base.SaveData(tag);
		}

		public override void LoadData(TagCompound tag) {
			item = tag.Get<Item>("item");
			base.LoadData(tag);
		}
	}


	// where the TE ends and the tile starts
	public class ItemPlacer : ModTile
	{
		public override void SetStaticDefaults() {
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
			Main.tileFrameImportant[Type] = true;

			AddMapEntry(Color.Blue, CreateMapEntryName());

			DustType = DustID.Stone;
			ItemDrop = ModContent.ItemType<Items.Placeables.Machines.Logic.ItemPlacer>();

			HitSound = SoundID.Tink;
		}

		public override bool Slope(int i, int j) {
			Tile tile = Framing.GetTileSafely(i, j);
			tile.TileFrameX = (short)((tile.TileFrameX + 16) % 64);
			return false;
		}

		public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak) {
			return false;
		}

		public static ItemPlacerTE GetTileEntity(int i, int j) {
			return TileEntity.ByPosition[new Point16(i, j)] as ItemPlacerTE;
		}

		public override void PlaceInWorld(int i, int j, Item item) {
			ModContent.GetInstance<ItemPlacerTE>().Place(i, j);
		}

		public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem) {
			if (effectOnly || noItem || fail) { return; }
			ItemPlacerTE tileEntity = GetTileEntity(i, j);
			if (tileEntity.item != null) {
				Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 32, tileEntity.item.type, tileEntity.item.stack);
				ModContent.GetInstance<ItemPlacerTE>().Kill(i, j);
			}
		}

		public override bool RightClick(int i, int j) {
			ItemPlacerTE tileEntity = GetTileEntity(i, j);
			Item item = tileEntity.item;
			Item playerItem;
			if (!Main.mouseItem.IsAir) {
				playerItem = Main.mouseItem;
			}
			else {
				playerItem = Main.player[Main.myPlayer].HeldItem;
			}

			if (item.IsAir) {
				item = playerItem.Clone();
				item.stack = 1;
				tileEntity.item = item;
				playerItem.stack--;
				if (playerItem.stack <= 0) {
					playerItem.TurnToAir();
				}
				return true;
			}
			if (!item.IsAir && playerItem.type == item.type && item.stack < item.maxStack) {
				item.stack++;
				playerItem.stack--;
				if (playerItem.stack <= 0) {
					playerItem.TurnToAir();
				}
				return true;
			}
			if (!item.IsAir) {
				item.stack--;
				Item.NewItem(new EntitySource_TileInteraction(Main.player[Main.myPlayer], i, j), i * 16, j * 16, 32, 32, item.type);
				if (item.stack <= 0) {
					item.TurnToAir();
				}
				
				return true;
			}
			return false;
		}
		public override void MouseOver(int i, int j) {
			ItemPlacerTE tileEntity = GetTileEntity(i, j);
			Item item = tileEntity.item;
			Player player = Main.LocalPlayer;
			player.noThrow = 2;
			if (item != null && !item.IsAir) {
				player.cursorItemIconEnabled = true;
				player.cursorItemIconText = "" + item.stack;
				player.cursorItemIconID = item.type;
			}
		}

		public void UseExtractinator(int i, int j, int extractType) {

			int num = 5000;
			int num2 = 25;
			int num3 = 50;
			int num4 = -1;
			if (extractType == 3347) {
				num /= 3;
				num2 *= 2;
				num3 = 20;
				num4 = 10;
			}
			int num5 = -1;
			int num6 = 1;
			if (num4 != -1 && Main.rand.Next(num4) == 0) {
				num5 = 3380;
				if (Main.rand.Next(5) == 0) {
					num6 += Main.rand.Next(2);
				}
				if (Main.rand.Next(10) == 0) {
					num6 += Main.rand.Next(3);
				}
				if (Main.rand.Next(15) == 0) {
					num6 += Main.rand.Next(4);
				}
			}
			else if (Main.rand.Next(2) == 0) {
				if (Main.rand.Next(12000) == 0) {
					num5 = 74;
					if (Main.rand.Next(14) == 0) {
						num6 += Main.rand.Next(0, 2);
					}
					if (Main.rand.Next(14) == 0) {
						num6 += Main.rand.Next(0, 2);
					}
					if (Main.rand.Next(14) == 0) {
						num6 += Main.rand.Next(0, 2);
					}
				}
				else if (Main.rand.Next(800) == 0) {
					num5 = 73;
					if (Main.rand.Next(6) == 0) {
						num6 += Main.rand.Next(1, 21);
					}
					if (Main.rand.Next(6) == 0) {
						num6 += Main.rand.Next(1, 21);
					}
					if (Main.rand.Next(6) == 0) {
						num6 += Main.rand.Next(1, 21);
					}
					if (Main.rand.Next(6) == 0) {
						num6 += Main.rand.Next(1, 21);
					}
					if (Main.rand.Next(6) == 0) {
						num6 += Main.rand.Next(1, 20);
					}
				}
				else if (Main.rand.Next(60) == 0) {
					num5 = 72;
					if (Main.rand.Next(4) == 0) {
						num6 += Main.rand.Next(5, 26);
					}
					if (Main.rand.Next(4) == 0) {
						num6 += Main.rand.Next(5, 26);
					}
					if (Main.rand.Next(4) == 0) {
						num6 += Main.rand.Next(5, 26);
					}
					if (Main.rand.Next(4) == 0) {
						num6 += Main.rand.Next(5, 25);
					}
				}
				else {
					num5 = 71;
					if (Main.rand.Next(3) == 0) {
						num6 += Main.rand.Next(10, 26);
					}
					if (Main.rand.Next(3) == 0) {
						num6 += Main.rand.Next(10, 26);
					}
					if (Main.rand.Next(3) == 0) {
						num6 += Main.rand.Next(10, 26);
					}
					if (Main.rand.Next(3) == 0) {
						num6 += Main.rand.Next(10, 25);
					}
				}
			}
			else if (num != -1 && Main.rand.Next(num) == 0) {
				num5 = 1242;
			}
			else if (num2 != -1 && Main.rand.Next(num2) == 0) {
				num5 = Main.rand.Next(6) switch {
					0 => 181,
					1 => 180,
					2 => 177,
					3 => 179,
					4 => 178,
					_ => 182,
				};
				if (Main.rand.Next(20) == 0) {
					num6 += Main.rand.Next(0, 2);
				}
				if (Main.rand.Next(30) == 0) {
					num6 += Main.rand.Next(0, 3);
				}
				if (Main.rand.Next(40) == 0) {
					num6 += Main.rand.Next(0, 4);
				}
				if (Main.rand.Next(50) == 0) {
					num6 += Main.rand.Next(0, 5);
				}
				if (Main.rand.Next(60) == 0) {
					num6 += Main.rand.Next(0, 6);
				}
			}
			else if (num3 != -1 && Main.rand.Next(num3) == 0) {
				num5 = 999;
				if (Main.rand.Next(20) == 0) {
					num6 += Main.rand.Next(0, 2);
				}
				if (Main.rand.Next(30) == 0) {
					num6 += Main.rand.Next(0, 3);
				}
				if (Main.rand.Next(40) == 0) {
					num6 += Main.rand.Next(0, 4);
				}
				if (Main.rand.Next(50) == 0) {
					num6 += Main.rand.Next(0, 5);
				}
				if (Main.rand.Next(60) == 0) {
					num6 += Main.rand.Next(0, 6);
				}
			}
			else if (Main.rand.Next(3) == 0) {
				if (Main.rand.Next(5000) == 0) {
					num5 = 74;
					if (Main.rand.Next(10) == 0) {
						num6 += Main.rand.Next(0, 3);
					}
					if (Main.rand.Next(10) == 0) {
						num6 += Main.rand.Next(0, 3);
					}
					if (Main.rand.Next(10) == 0) {
						num6 += Main.rand.Next(0, 3);
					}
					if (Main.rand.Next(10) == 0) {
						num6 += Main.rand.Next(0, 3);
					}
					if (Main.rand.Next(10) == 0) {
						num6 += Main.rand.Next(0, 3);
					}
				}
				else if (Main.rand.Next(400) == 0) {
					num5 = 73;
					if (Main.rand.Next(5) == 0) {
						num6 += Main.rand.Next(1, 21);
					}
					if (Main.rand.Next(5) == 0) {
						num6 += Main.rand.Next(1, 21);
					}
					if (Main.rand.Next(5) == 0) {
						num6 += Main.rand.Next(1, 21);
					}
					if (Main.rand.Next(5) == 0) {
						num6 += Main.rand.Next(1, 21);
					}
					if (Main.rand.Next(5) == 0) {
						num6 += Main.rand.Next(1, 20);
					}
				}
				else if (Main.rand.Next(30) == 0) {
					num5 = 72;
					if (Main.rand.Next(3) == 0) {
						num6 += Main.rand.Next(5, 26);
					}
					if (Main.rand.Next(3) == 0) {
						num6 += Main.rand.Next(5, 26);
					}
					if (Main.rand.Next(3) == 0) {
						num6 += Main.rand.Next(5, 26);
					}
					if (Main.rand.Next(3) == 0) {
						num6 += Main.rand.Next(5, 25);
					}
				}
				else {
					num5 = 71;
					if (Main.rand.Next(2) == 0) {
						num6 += Main.rand.Next(10, 26);
					}
					if (Main.rand.Next(2) == 0) {
						num6 += Main.rand.Next(10, 26);
					}
					if (Main.rand.Next(2) == 0) {
						num6 += Main.rand.Next(10, 26);
					}
					if (Main.rand.Next(2) == 0) {
						num6 += Main.rand.Next(10, 25);
					}
				}
			}
			else {
				num5 = Main.rand.Next(8) switch {
					0 => 12,
					1 => 11,
					2 => 14,
					3 => 13,
					4 => 699,
					5 => 700,
					6 => 701,
					_ => 702,
				};
				if (Main.rand.Next(20) == 0) {
					num6 += Main.rand.Next(0, 2);
				}
				if (Main.rand.Next(30) == 0) {
					num6 += Main.rand.Next(0, 3);
				}
				if (Main.rand.Next(40) == 0) {
					num6 += Main.rand.Next(0, 4);
				}
				if (Main.rand.Next(50) == 0) {
					num6 += Main.rand.Next(0, 5);
				}
				if (Main.rand.Next(60) == 0) {
					num6 += Main.rand.Next(0, 6);
				}
			}
			ItemLoader.ExtractinatorUse(ref num5, ref num6, extractType, TileID.Extractinator);
			if (num5 > 0) {
				int number = Item.NewItem(new EntitySource_TileUpdate(i, j), i*16, j*16, 16, 16, num5, num6, noBroadcast: false, -1);
				if (Main.netMode == 1) {
					NetMessage.SendData(21, -1, -1, null, number, 1f);
				}
			}
		}

		public override void HitWire(int i, int j) {
			ItemPlacerTE tileEntity = GetTileEntity(i, j);
			Item item = tileEntity.item;
			Tile tile = Framing.GetTileSafely(i, j);
			var dir = new Direction(tile.TileFrameX / 16);
			int x = i + dir.point.X;
			int y = i + dir.point.Y;

			int xOff = dir.point.X;
			int yOff = dir.point.Y;

			if (Main.tile[i + xOff, j + yOff].TileType == TileID.Extractinator && ItemID.Sets.ExtractinatorMode[item.type] >= 0) {
				UseExtractinator(i + xOff, j + yOff, ItemID.Sets.ExtractinatorMode[item.type]);

				item.stack--;
				if (item.stack <= 0) {
					item.TurnToAir();
					item.createTile = -1;
				}

				return;
			}

			if (item.createTile > -1 && WorldGen.PlaceTile(i + xOff, j + yOff, item.createTile)) {
				ModTile modTile = ModContent.GetModTile(item.createTile);
				if (modTile != null)
					modTile.PlaceInWorld(i + xOff, j + yOff, item);
				item.stack--;
				if (item.stack <= 0) {
					item.TurnToAir();
					item.createTile = -1;
				}
				return;
			}
			if (item.createWall > 0 && Main.tile[i + xOff, j + yOff].WallType > 0) {
				WorldGen.PlaceWall(i + xOff, j + yOff, item.createWall);
				ModWall modWall = ModContent.GetModWall(item.createWall);
				if (modWall != null)
					modWall.PlaceInWorld(i + xOff, j + yOff, item);
				item.stack--;
				if (item.stack <= 0) {
					item.TurnToAir();
					item.createTile = -1;
				}
				return;
			}
			if (item.createTile <= -1 && !item.IsAir) {
				Main.item[Item.NewItem(new EntitySource_TileBreak(i, j), i * 16 - 8, j * 16 - 8, 32, 32, item.type)].velocity = new Vector2(xOff * 5, yOff * 5 - 1);

				item.stack--;
				if (item.stack <= 0) {
					item.TurnToAir();
					item.createTile = -1;
				}
			}
		}
	}
}
