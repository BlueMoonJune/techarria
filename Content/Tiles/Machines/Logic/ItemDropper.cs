﻿using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Terraria.ModLoader.IO;

using TE = Techarria.Content.Tiles.Machines.Logic.ItemDropperTE;

namespace Techarria.Content.Tiles.Machines.Logic
{
	public class ItemDropperTE : InventoryTileEntity
	{
		public Item item = new();
        public override Item[] ExtractableItems => new Item[] { item };

        public override void SaveData(TagCompound tag) {
			tag.Add("item", item);
			base.SaveData(tag);
		}

		public override void LoadData(TagCompound tag) {
			item = tag.Get<Item>("item");
			base.LoadData(tag);
		}

        public override bool InsertItem(Item item)
        {
            Item myItem = this.item;
            if (myItem == null || myItem.IsAir)
            {
                myItem = item.Clone();
                myItem.stack = 1;
                this.item = myItem;
                return true;
            }
            if (myItem.type == item.type && myItem.stack < myItem.maxStack)
            {
                myItem.stack++;
                return true;
            }

            return false;
        }
    }


	// where the TE ends and the tile starts
	public class ItemDropper : EntityTile<TE>
	{
		public override void PreStaticDefaults() {
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
			Main.tileFrameImportant[Type] = true;

			AddMapEntry(Color.Gray, CreateMapEntryName());

			DustType = DustID.Stone;
			//ItemDrop = ModContent.ItemType<Items.Placeables.Machines.Logic.ItemDropper>();

			HitSound = SoundID.Tink;
		}

		// rotates the tile
		public override bool Slope(int i, int j) {
			Tile tile = Framing.GetTileSafely(i, j);
			tile.TileFrameX = (short)((tile.TileFrameX + 16) % 64);
			return false;
		}

		public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak) {
			return false;
		}

		public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem) {
			if (effectOnly || noItem || fail) { return; }
			ItemDropperTE tileEntity = GetTileEntity(i, j);
			if (tileEntity.item != null) {
				Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 32, tileEntity.item.type, tileEntity.item.stack);
				ModContent.GetInstance<ItemDropperTE>().Kill(i, j);
			}
		}

		public override bool RightClick(int i, int j) {
			ItemDropperTE tileEntity = GetTileEntity(i, j);
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
			ItemDropperTE tileEntity = GetTileEntity(i, j);
			Item item = tileEntity.item;
			Player player = Main.LocalPlayer;
			player.noThrow = 2;
			if (item != null && !item.IsAir) {
				player.cursorItemIconEnabled = true;
				player.cursorItemIconText = "" + item.stack;
				player.cursorItemIconID = item.type;
			}
		}

		public override void HitWire(int i, int j) {
			ItemDropperTE tileEntity = GetTileEntity(i, j);
			Item item = tileEntity.item;
            if (item.IsAir) return;
            Tile tile = Framing.GetTileSafely(i, j);
			var dir = new Direction(tile.TileFrameX / 16);

			int xOff = dir.point.X;
			int yOff = dir.point.Y;

			Main.item[Item.NewItem(new EntitySource_TileUpdate(i, j), i * 16 - 8, j * 16 - 8, 32, 32, item)].velocity = new Vector2(xOff * 5, yOff * 5 - 1);
			item.TurnToAir();
		}
	}
}
