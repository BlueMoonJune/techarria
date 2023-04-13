using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Techarria.Content.Dusts;
using Techarria.Content.Items;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;
using System.Collections.Generic;
using Terraria.Localization;
using Techarria.Content.Items.Materials;

namespace Techarria.Content.Tiles.Machines
{
	public class ChargingRackTE : ModTileEntity
	{
		public Item item = new Item();

		public bool fullyCharged = false;


		public override bool IsTileValidForEntity(int x, int y) {
			return Main.tile[x, y].TileType == ModContent.TileType<ChargingRack>();
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

	public class ChargingRack : PowerConsumer
	{
		public static Dictionary<int, string> capacitorTextures = new();

		public override void SetStaticDefaults() {
			Main.tileLavaDeath[Type] = false;
			Main.tileNoAttach[Type] = true;
			Main.tileFrameImportant[Type] = true;
			TileID.Sets.DisableSmartCursor[Type] = true;

			DustType = ModContent.DustType<Wormhole>();

			// Placement
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 18 };
			TileObjectData.newTile.LavaDeath = false;
			TileObjectData.addTile(Type);

			// map info
			AddMapEntry(new Color(200, 200, 200), Language.GetText("Charging Rack"));
		}

		public ChargingRackTE GetTileEntity(int i, int j) {
			Tile tile = Framing.GetTileSafely(i, j);
			i -= tile.TileFrameX / 18 % 3;
			j -= tile.TileFrameY / 18 % 2;
			TileEntity.ByPosition.TryGetValue(new Point16(i, j), out TileEntity tileEntity);
			return tileEntity as ChargingRackTE;
		}

		public override void PlaceInWorld(int i, int j, Item item) {
			Tile tile = Framing.GetTileSafely(i, j);
			i -= tile.TileFrameX / 18 % 3;
			j -= tile.TileFrameY / 18 % 2;
			ModContent.GetInstance<ChargingRackTE>().Place(i, j);
		}

		public override void KillMultiTile(int i, int j, int frameX, int frameY) {
			ChargingRackTE tileEntity = GetTileEntity(i, j);
			Item item = tileEntity.item;
			Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 48, 64, item);
			ModContent.GetInstance<ChargingRackTE>().Kill(i, j);
		}

		public static bool AcceptsItem(Item item) {
			return item.ModItem is Capacitor;
		}

		public override bool RightClick(int i, int j) {
			ChargingRackTE tileEntity = GetTileEntity(i, j);
			Point16 subtile = new Point16(i, j) - tileEntity.Position;
			if (tileEntity == null)
				return false;

			Item item = tileEntity.item;
			Item playerItem;
			if (!Main.mouseItem.IsAir) {
				playerItem = Main.mouseItem;
			}
			else {
				playerItem = Main.player[Main.myPlayer].HeldItem;
			}
			if (tileEntity.item.IsAir && playerItem.ModItem is ChargableItem) {
				tileEntity.item = playerItem.Clone();
				playerItem.TurnToAir();
			} else if (!tileEntity.item.IsAir) {
				Item.NewItem(new EntitySource_TileInteraction(Main.player[Main.myPlayer], i, j), i * 16, j * 16, 16, 16, tileEntity.item);
				tileEntity.item.TurnToAir();
			}
			return false;
		}

		public override void MouseOver(int i, int j) {
			ChargingRackTE tileEntity = GetTileEntity(i, j);
			if (tileEntity == null)
				return;

			Item item = tileEntity.item;
			var modItem = item.ModItem as ChargableItem;
			Player player = Main.LocalPlayer;
			player.noThrow = 2;
			if (item != null && !item.IsAir) {
				player.cursorItemIconEnabled = true;
				player.cursorItemIconText = modItem.charge + "/" + modItem.maxcharge;
				player.cursorItemIconID = item.type;
			}
		}

		public override void InsertPower(int i, int j, int amount) {
			ChargingRackTE tileEntity = GetTileEntity(i, j);
			if (tileEntity.item.ModItem is ChargableItem item) {
				if (item.Charge(amount) == 0) {
					Wiring.TripWire(tileEntity.Position.X, tileEntity.Position.Y, 3, 2);
				}
			}
		}

		public override void PostDraw(int i, int j, SpriteBatch spriteBatch) {
			Tile tile = Main.tile[i, j];

			ChargingRackTE tileEntity = GetTileEntity(i, j);
			if (tileEntity == null)
				return;

			Point16 subtile = new Point16(i, j) - tileEntity.Position;
			if (subtile != new Point16(2, 1)) {
				return;
			}

			Vector2 TileOffset = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
			Vector2 pos = new Vector2(i, j) * 16 + new Vector2(-22, 6) - Main.screenPosition + TileOffset;

			ChargableItem item = tileEntity.item.ModItem as ChargableItem;
			if (item == null)
				return;
			float percentage = item.charge / (float)item.maxcharge;
			Rectangle sourceRect = new Rectangle(0, 0, (int)(28 * percentage), 6);
			Color color;
			if (percentage > 0.5f) {
				color = new Color(2 - percentage * 2, 1, 0);
			} else {
				color = new Color(1, percentage * 2, 0);
			}

			spriteBatch.Draw(ModContent.Request<Texture2D>("Techarria/Content/Tiles/Machines/ChargingRack_Overlay").Value, pos, sourceRect, color);

		}
	}
}
