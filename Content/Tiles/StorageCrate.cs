using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;

namespace Techarria.Content.Tiles
{
	public class StorageCrateTE : ModTileEntity
    {
		public Item item = new Item();
		public override bool IsTileValidForEntity(int x, int y)
        {
            return Main.tile[x, y].TileType == ModContent.TileType<StorageCrate>();
        }

		public override void SaveData(TagCompound tag)
		{
			tag.Add("item", item);
			base.SaveData(tag);
		}

		public override void LoadData(TagCompound tag)
		{
			item = tag.Get<Item>("item");
			base.LoadData(tag);
		}
	}

	// Where the TE ends and the Tile starts
	public class StorageCrate : ModTile
	{
		public override void SetStaticDefaults()
		{
			// Spelunker
			Main.tileSpelunker[Type] = true;

			// Properties
			Main.tileLavaDeath[Type] = false;
			Main.tileSolidTop[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileFrameImportant[Type] = true;
			TileID.Sets.DisableSmartCursor[Type] = true;
			TileID.Sets.IgnoredByNpcStepUp[Type] = true;
			Main.tileTable[Type] = true;

			// placement
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			TileObjectData.newTile.Origin = new Point16(0, 1);
			TileObjectData.newTile.CoordinateHeights = new[] { 16, 18 };
			TileObjectData.newTile.HookCheckIfCanPlace = new PlacementHook(Chest.FindEmptyChest, -1, 0, true);
			TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(Chest.AfterPlacement_Hook, -1, 0, false);
			TileObjectData.newTile.AnchorInvalidTiles = new int[] { TileID.MagicalIceBlock };
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.LavaDeath = false;
			TileObjectData.addTile(Type);
		}
		public StorageCrateTE GetTileEntity(int i, int j)
		{
			Tile tile = Framing.GetTileSafely(i, j);
			i -= tile.TileFrameX / 18 % 2;
			j -= tile.TileFrameY / 18 % 2;
			TileEntity.ByPosition.TryGetValue(new Point16(i, j), out TileEntity te);
			return te as StorageCrateTE;
		}
		public override void PlaceInWorld(int i, int j, Item item)
		{
			Tile tile = Framing.GetTileSafely(i, j);
			i -= tile.TileFrameX / 18 % 2;
			j -= tile.TileFrameY / 18 % 2;
			ModContent.GetInstance<StorageCrateTE>().Place(i, j);
		}

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
		{
			StorageCrateTE tileEntity = GetTileEntity(i, j);
			Item item = tileEntity.item;
			if (!item.IsAir)
            {
				fail = true;
				int amount = Math.Min(item.maxStack, item.stack);
				item.stack -= amount;
				Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 32, item.type, amount);
			}

			base.KillTile(i, j, ref fail, ref effectOnly, ref noItem);
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{
			StorageCrateTE tileEntity = GetTileEntity(i, j);
			Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 32, ModContent.ItemType<Items.Placeables.StorageCrate>());
			Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 32, tileEntity.item.type, tileEntity.item.stack);
			if (tileEntity.item.IsAir)
			ModContent.GetInstance<StorageCrateTE>().Kill(i, j);
		}

		public override bool RightClick(int i, int j)
		{
			StorageCrateTE tileEntity = GetTileEntity(i, j);
			Item item = tileEntity.item;
			Item playerItem;
			if (!Main.mouseItem.IsAir)
			{
				playerItem = Main.mouseItem;
			}
			else
			{
				playerItem = Main.player[Main.myPlayer].HeldItem;
			}

			if (item.IsAir)
			{
				item = playerItem.Clone();
				item.stack = 1;
				tileEntity.item = item;
				playerItem.stack--;
				if (playerItem.stack <= 0)
				{
					playerItem.TurnToAir();
				}
				return true;
			}
			if (!item.IsAir && playerItem.type == item.type && item.stack < 999999 /* <- max storage within a single storage crate */) 
			{
				item.stack++;
				playerItem.stack--;
				if (playerItem.stack <= 0)
				{
					playerItem.TurnToAir();
				}
				return true;
			}
			if (!item.IsAir)
			{
				item.stack--;
				Item dropItem = item.Clone();
				dropItem.stack = 1;
				Item.NewItem(new EntitySource_TileInteraction(Main.player[Main.myPlayer], i, j), new Rectangle(i * 16, j * 16, 32, 32), dropItem);
				if (item.stack <= 0)
				{
					item.TurnToAir();
				}
				return true;
			}
			return false;
		}


		public override void MouseOver(int i, int j)
		{
			StorageCrateTE tileEntity = GetTileEntity(i, j);
			Item item = tileEntity.item;
			Player player = Main.LocalPlayer;
			player.noThrow = 2;
			if ((item != null) && (!item.IsAir))
			{
				player.cursorItemIconEnabled = true;
				player.cursorItemIconText = "" + item.stack;
				player.cursorItemIconID = item.type;
			}
		}

		/*
		public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
		{
			StorageCrateTE tileEntity = GetTileEntity(i, j);
			Point16 subTile = new Point16(i, j) - tileEntity.Position;	
			if (subTile.X == 1 && subTile.Y == 1)
			{
				Item item = tileEntity.item;

				Vector2 TileOffset = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
				Vector2 pos = new Vector2(i, j) * 16 - Main.screenPosition + TileOffset;

				HelperMethods.DrawItemInWorld(spriteBatch, item, pos, 16);

			}

		}
		*/
	}
}
