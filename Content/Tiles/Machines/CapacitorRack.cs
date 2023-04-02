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
	public class CapacitorRackTE : ModTileEntity
	{
		public Item[] items = new Item[3] { new Item(), new Item(), new Item() };
		public int lastCharged = 0;
		public bool discharged = false;


		public override bool IsTileValidForEntity(int x, int y) {
			return Main.tile[x, y].TileType == ModContent.TileType<CapacitorRack>();
		}

		public override void Update() {
			Dust.NewDust(new Vector2(Position.X, Position.Y) * 16, 0, 0, ModContent.DustType<TransferDust>());
			if (discharged) {
				for (int x = 0; x < 3; x++) {

					var capacitor = items[x].ModItem as Capacitor;
					if (capacitor == null) {
						continue;
					}
					if (capacitor.charge > 0) {
						Power.TransferCharge(capacitor.charge, Position.X + x, Position.Y);
						Wiring.TripWire(Position.X + x, Position.Y, 1, 1);
					}

					capacitor.charge = 0;
					discharged = false;
				}
			}
		}

		public override void SaveData(TagCompound tag) {
			tag.Add("item", items);
			base.SaveData(tag);
		}

		public override void LoadData(TagCompound tag) {
			items = tag.Get<Item[]>("item");
			base.LoadData(tag);
		}
	}

	public class CapacitorRack : PowerConsumer
	{
		public static Dictionary<int, string> capacitorTextures = new();

		public override void SetStaticDefaults() {
			Main.tileLavaDeath[Type] = false;
			Main.tileTable[Type] = true;
			Main.tileSolidTop[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileFrameImportant[Type] = true;
			TileID.Sets.DisableSmartCursor[Type] = true;
			TileID.Sets.IgnoredByNpcStepUp[Type] = true;

			AdjTiles = new int[] { TileID.WorkBenches };

			// placement
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
			TileObjectData.newTile.CoordinateHeights = new[] { 18, 18 };
			TileObjectData.addTile(Type);

			AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTable);

			// map info
			LocalizedText name = CreateMapEntryName();
			AddMapEntry(new Color(200, 200, 200), Language.GetText("Casting Table"));
		}

		public CapacitorRackTE GetTileEntity(int i, int j) {
			Tile tile = Framing.GetTileSafely(i, j);
			i -= tile.TileFrameX / 18 % 3;
			j -= tile.TileFrameY / 18 % 2;
			TileEntity.ByPosition.TryGetValue(new Point16(i, j), out TileEntity tileEntity);
			return tileEntity as CapacitorRackTE;
		}

		public override void PlaceInWorld(int i, int j, Item item) {
			Tile tile = Framing.GetTileSafely(i, j);
			i -= tile.TileFrameX / 18 % 3;
			j -= tile.TileFrameY / 18 % 2;
			ModContent.GetInstance<CapacitorRackTE>().Place(i, j);
		}

		public override void KillMultiTile(int i, int j, int frameX, int frameY) {
			CapacitorRackTE tileEntity = GetTileEntity(i, j);
			foreach (Item item in tileEntity.items) {
				Item newItem = Main.item[Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 48, 64, item.type, item.stack)];
				if (item.ModItem is Capacitor capacitor && newItem.ModItem is Capacitor newCapacitor) {
					newCapacitor.charge = capacitor.charge;
				}
			}
			Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 48, 64, ModContent.ItemType<Items.Placeables.Machines.CapacitorRack>());
			ModContent.GetInstance<CapacitorRackTE>().Kill(i, j);
		}

		public static bool AcceptsItem(Item item) {
			return item.ModItem is Capacitor;
		}

		public override bool RightClick(int i, int j) {
			CapacitorRackTE tileEntity = GetTileEntity(i, j);
			Point16 subtile = new Point16(i, j) - tileEntity.Position;
			if (tileEntity == null)
				return false;

			Item item = tileEntity.items[i - tileEntity.Position.X];
			Item playerItem;
			if (!Main.mouseItem.IsAir) {
				playerItem = Main.mouseItem;
			}
			else {
				playerItem = Main.player[Main.myPlayer].HeldItem;
			}

			if (subtile.Y == 0) {
				int index = subtile.X;
				if (tileEntity.items[index].IsAir && playerItem.ModItem is Capacitor) {
					tileEntity.items[index] = playerItem.Clone();
					playerItem.TurnToAir();
				} else if (!tileEntity.items[index].IsAir) {
					Item.NewItem(new EntitySource_TileInteraction(Main.player[Main.myPlayer], i, j), i * 16, j * 16, 16, 16, tileEntity.items[index]);
					tileEntity.items[index].TurnToAir();
				}
			}
			return false;
		}

		public override void MouseOver(int i, int j) {
			CapacitorRackTE tileEntity = GetTileEntity(i, j);
			if (tileEntity == null)
				return;
			if (j != tileEntity.Position.Y) {
				return;
			}
			Item item = tileEntity.items[i - tileEntity.Position.X];
			var modItem = item.ModItem as Capacitor;
			Player player = Main.LocalPlayer;
			player.noThrow = 2;
			if (item != null && !item.IsAir) {
				player.cursorItemIconEnabled = true;
				player.cursorItemIconText = modItem.charge + "/" + modItem.maxcharge;
				player.cursorItemIconID = item.type;
			}
		}

		public override void HitWire(int i, int j) {
			CapacitorRackTE tileEntity = GetTileEntity(i, j);
			Point16 subTile = new Point16(i, j) - tileEntity.Position;
			if (subTile.X == 1 && subTile.Y == 1 && !tileEntity.discharged) {
				tileEntity.discharged = true;
			}
		}

		public override bool IsConsumer(int i, int j) {
			CapacitorRackTE tileEntity = GetTileEntity(i, j);
			Point16 subTile = new Point16(i, j) - tileEntity.Position;
			if (subTile.X != 1 && subTile.Y == 1) {
				return true;
			}
			return false;
		}

		public override void InsertPower(int i, int j, int amount) {
			CapacitorRackTE tileEntity = GetTileEntity(i, j);
			Point16 subTile = new Point16(i, j) - tileEntity.Position;
			if (subTile.X != 1 && subTile.Y == 1) {
				for (int c = 0; c < amount; c++) {
					bool charged = false;
					for (int x = 0; x < 3 && !charged; x++) {
						tileEntity.lastCharged++;
						tileEntity.lastCharged %= 3;
						var capacitor = tileEntity.items[tileEntity.lastCharged].ModItem as Capacitor;
						if (capacitor == null) {
							continue;
						}
						if (capacitor.Charge(1) == 1)
							charged = true;
					}
				}
				return;
			}
		}

		public override void PostDraw(int i, int j, SpriteBatch spriteBatch) {
			Tile tile = Main.tile[i, j];
			if (tile.TileFrameY != 0) {
				return;
			}
			Vector2 TileOffset = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
			Vector2 pos = new Vector2(i, j) * 16 - Main.screenPosition + TileOffset;

			CapacitorRackTE tileEntity = GetTileEntity(i, j);
			if (tileEntity == null)
				return;
			int c = tile.TileFrameX / 18;

			Item item = tileEntity.items[c];
			if (item == null || item.IsAir)
				return;
			spriteBatch.Draw(ModContent.Request<Texture2D>("Techarria/Content/Tiles/Capacitors/" + capacitorTextures[item.type]).Value, new Rectangle((int)pos.X, (int)pos.Y, 16, 16), Lighting.GetColor(i, j));
			if (item.ModItem is Capacitor capacitor) {
				float v = capacitor.charge / (float)capacitor.maxcharge;
				Lighting.AddLight(new Vector2(i * 16 + 8, j * 16 + 8), 1f * v, 0.3f * v, 0.3f * v);
				if (v > 0)
					spriteBatch.Draw(ModContent.Request<Texture2D>("Techarria/Content/Tiles/Capacitors/" + capacitorTextures[item.type] + "_Glow").Value, new Rectangle((int)pos.X, (int)pos.Y, 16, 16), Color.White);
			}
		}
	}
}
