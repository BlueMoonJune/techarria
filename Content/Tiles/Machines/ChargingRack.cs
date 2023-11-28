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
	public class ChargingRackTE : InventoryTileEntity
	{
		public Item item = new Item();

		public bool fullyCharged = false;

		public override Item[] ExtractableItems => new Item[] { item };

        public override bool IsTileValidForEntity(int x, int y) {
			return Main.tile[x, y].TileType == ModContent.TileType<ChargingRack>();
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
            if (item.type == myItem.type && myItem.stack < myItem.maxStack)
            {
                myItem.stack++;
                return true;
            }
            return false;
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

	public class ChargingRack : EntityTile<ChargingRackTE>, IPowerConsumer
	{
		public static Dictionary<int, string> capacitorTextures = new();

		public override void PreStaticDefaults() {
			Main.tileLavaDeath[Type] = false;
			Main.tileNoAttach[Type] = true;
			Main.tileFrameImportant[Type] = true;
			TileID.Sets.DisableSmartCursor[Type] = true;

			DustType = ModContent.DustType<Spikesteel>();

			width = 3; 
			height = 2;

			// map info
			AddMapEntry(new Color(200, 200, 200), Language.GetText("Charging Rack"));
		}

		public override bool RightClick(int i, int j) {
			ChargingRackTE tileEntity = GetTileEntity(i, j);
			if (tileEntity == null)
				return false;

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

		public void InsertPower(int i, int j, int amount) {
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
