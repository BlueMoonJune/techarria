using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Terraria.ModLoader.IO;
using System.Collections.Generic;
using Techarria.Transfer;

namespace Techarria.Content.Tiles.Machines.Logic
{
	public class AdvancedBlockBreakerTE : ModTileEntity
	{
		public List<Item> items = new();
		public override bool IsTileValidForEntity(int x, int y) {
			return Main.tile[x, y].TileType == ModContent.TileType<AdvancedBlockBreaker>();
		}

		public void InsertItem(Item input) {
			foreach (Item item in items) {
				if (item.type == input.type) {
					item.stack += input.stack;
					return;
				}
			}
			items.Add(input);
		}

		public override void SaveData(TagCompound tag) {
			tag.Add("items", items);
			base.SaveData(tag);
		}

		public override void LoadData(TagCompound tag) {
			items = tag.Get<List<Item>>("items");
			base.LoadData(tag);
		}
	}


	// where the TE ends and the tile starts
	public class AdvancedBlockBreaker : ModTile
	{
		public static int power = 110;

		public override void SetStaticDefaults() {
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
			Main.tileFrameImportant[Type] = true;

			AddMapEntry(Color.Blue, CreateMapEntryName());

			DustType = DustID.Stone;
			ItemDrop = ModContent.ItemType<Items.Placeables.Machines.Logic.AdvancedBlockBreaker>();

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

		public static AdvancedBlockBreakerTE GetTileEntity(int i, int j) {
			return TileEntity.ByPosition[new Point16(i, j)] as AdvancedBlockBreakerTE;
		}

		public override void PlaceInWorld(int i, int j, Item item) {
			ModContent.GetInstance<AdvancedBlockBreakerTE>().Place(i, j);
		}

		public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem) {
			if (effectOnly || noItem || fail) { return; }
			AdvancedBlockBreakerTE tileEntity = GetTileEntity(i, j);
			foreach (Item item in tileEntity.items) {
				Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 32, item.type, item.stack);
				ModContent.GetInstance<AdvancedBlockBreakerTE>().Kill(i, j);
			}
		}

		public override bool RightClick(int i, int j) {
			AdvancedBlockBreakerTE tileEntity = GetTileEntity(i, j);
			Item item = tileEntity.items[0];

			if (!item.IsAir) {
				item.stack--;
				Item.NewItem(new EntitySource_TileInteraction(Main.player[Main.myPlayer], i, j), i * 16, j * 16, 32, 32, item.type);
				if (item.stack <= 0) {
					item.TurnToAir();
					tileEntity.items.RemoveAt(0);
				}
				
				return true;
			}
			return false;
		}
		public override void MouseOver(int i, int j) {
			AdvancedBlockBreakerTE tileEntity = GetTileEntity(i, j);
		}

		public override void HitWire(int i, int j) {
			AdvancedBlockBreakerTE tileEntity = GetTileEntity(i, j);
			Tile tile = Framing.GetTileSafely(i, j);
			var dir = new Direction(tile.TileFrameX / 16);
			int xOff = dir.point.X;
			int yOff = dir.point.Y;

			tile.TileFrameY += 16;
			tile.TileFrameY %= 32;

			int tx = i + xOff;
			int ty = j + yOff;

			Item[] preItems = new Item[Main.item.Length];
			Main.item.CopyTo(preItems, 0);
			Point pos = ChestInterface.FindTopLeft(tx, ty);
			if (pos != Point.Zero) {
				Chest chest = Main.chest[Chest.FindChest(pos.X, pos.Y)];

				for (int x = 0; x < chest.item.Length; x++) {
					Item item = chest.item[x];
					tileEntity.InsertItem(item);
					chest.item[x].TurnToAir();
				}
				Main.LocalPlayer.PickTile(tx, ty, power);
				return;
			}
			pos = ChestInterface.FindTopLeft(tx, ty - 1);
			if (pos != Point.Zero) {
				Chest chest = Main.chest[Chest.FindChest(pos.X, pos.Y)];

				for (int x = 0; x < chest.item.Length; x++) {
					Item item = chest.item[x];
					tileEntity.InsertItem(item);
					chest.item[x].TurnToAir();
				}
				Main.LocalPlayer.PickTile(tx, ty - 1, power);
				return;
			}
			Main.LocalPlayer.PickTile(tx, ty, power);

			for (int x = 0; x < Main.item.Length; x++) {
				if (Main.item[x] != preItems[x] && !Main.item[x].IsAir) {
					tileEntity.InsertItem(Main.item[x].Clone());
					Main.item[x].TurnToAir();
				}
			}
		}
	}
}
