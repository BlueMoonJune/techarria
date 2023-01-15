using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Techarria.Content.Dusts;
using Techarria.Content.Items;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;
using System.Collections.Generic;

namespace Techarria.Content.Tiles
{
    public class CapacitorRackTE : ModTileEntity
    {
		public Item[] items = new Item[3] { new Item(), new Item(), new Item() };
		public int lastCharged = 0;
		public bool discharged = false;


        public override bool IsTileValidForEntity(int x, int y)
        {
            return Main.tile[x, y].TileType == ModContent.TileType<CapacitorRack>();
        }

        public override void Update()
		{
			Dust.NewDust(new Vector2(Position.X, Position.Y) * 16, 0, 0, ModContent.DustType<TransferDust>());
			if (discharged)
			{
				for (int x = 0; x < 3; x++)
				{

					Capacitor capacitor = items[x].ModItem as Capacitor;
					if (capacitor == null)
					{
						Main.NewText("Item is null");
						continue;
					}
					Power.TransferCharge(capacitor.charge, Position.X + x, Position.Y);
					Wiring.TripWire(Position.X + x, Position.Y, 1, 1);
					
					capacitor.charge = 0;
					discharged = false;
				}
			}
		}

        public override void SaveData(TagCompound tag)
        {
			tag.Add("item", items);
			base.SaveData(tag);
        }

        public override void LoadData(TagCompound tag)
        {
			items = tag.Get<Item[]>("item");
            base.LoadData(tag);
        }
    }

    public class CapacitorRack : PowerConsumer
	{
		public static Dictionary<int, string> capacitorTextures = new Dictionary<int, string>();

		public override void SetStaticDefaults()
		{
			capacitorTextures.Add(ModContent.ItemType<Capacitor>(), "Capacitor");

			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;
			Main.tileFrameImportant[Type] = true;
			TileID.Sets.DisableSmartCursor[Type] = true;
			TileID.Sets.IgnoredByNpcStepUp[Type] = true; // This line makes NPCs not try to step up this tile during their movement. Only use this for furniture with solid tops.

			DustType = ModContent.DustType<Dusts.Wormhole>();
			AdjTiles = new int[] { TileID.Tables };

			// Placement
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.addTile(Type);

			// Etc
			ModTranslation name = CreateMapEntryName();
			name.SetDefault("Player Interface");
			AddMapEntry(new Color(200, 200, 200), name);
		}

		public CapacitorRackTE GetTileEntity(int i, int j)
		{
			Tile tile = Framing.GetTileSafely(i, j);
			i -= tile.TileFrameX / 18 % 3;
			j -= tile.TileFrameY / 18 % 2;
			TileEntity.ByPosition.TryGetValue(new Point16(i, j), out TileEntity tileEntity);
			return tileEntity as CapacitorRackTE;
		}

        public override void PlaceInWorld(int i, int j, Item item)
        {
			Tile tile = Framing.GetTileSafely(i, j);
			i -= tile.TileFrameX / 18 % 3;
			j -= tile.TileFrameY / 18 % 2;
			Main.NewText(i + " " + j);
			ModContent.GetInstance<CapacitorRackTE>().Place(i, j);
        }

		public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
		{
			CapacitorRackTE tileEntity = GetTileEntity(i, j);
			if (tileEntity == null) return;
			Tile tile = Framing.GetTileSafely(i, j);
			int c = tile.TileFrameX / 18;
			if (tile.TileFrameY / 36 == 0 && !tileEntity.items[c].IsAir) { 
				Item newItem = Main.item[Item.NewItem(new EntitySource_TileInteraction(Main.player[Main.myPlayer], i, j), i * 16, j * 16, 32, 32, tileEntity.items[c].type)];
				Capacitor newCapacitor = newItem.ModItem as Capacitor;
				Capacitor capacitor = tileEntity.items[c].ModItem as Capacitor;
				newCapacitor.charge = capacitor.charge;
			}
			if (tile.TileFrameX % 54 == 0 && tile.TileFrameY % 36 == 0)
            {
				Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 48, 32, ModContent.ItemType<Items.Placeables.CapacitorRack>());

				ModContent.GetInstance<CapacitorRackTE>().Kill(i, j);
			}
		}

		public static bool AcceptsItem(Item item)
        {
			return item.ModItem is Capacitor;
        }

        public override bool RightClick(int i, int j)
        {
			Main.NewText("RightClick");
			CapacitorRackTE tileEntity = GetTileEntity(i, j);
			if (tileEntity == null) return false;

			Item item = tileEntity.items[i-tileEntity.Position.X];
			Item playerItem;
			if (!Main.mouseItem.IsAir)
            {
				playerItem = Main.mouseItem;
            } else
            {
				playerItem = Main.player[Main.myPlayer].HeldItem;
            }

			Main.NewText("Item Interaction: " + playerItem);
			if (item.IsAir && AcceptsItem(playerItem))
			{
				Main.NewText("Item Empty: Insert");
				item = playerItem.Clone();
				item.stack = 1;
                tileEntity.items[i - tileEntity.Position.X] = item;
				playerItem.stack--;
				if (playerItem.stack <= 0)
                {
					playerItem.TurnToAir();
                }
				return true;
            }
			if (!item.IsAir)
			{
				Main.NewText("Item Does not Match: Extract");
				Item newItem = Main.item[Item.NewItem(new EntitySource_TileInteraction(Main.player[Main.myPlayer], i, j), i * 16, j * 16, 32, 32, item.type)];
				Capacitor newCapacitor = newItem.ModItem as Capacitor;
				Capacitor capacitor = item.ModItem as Capacitor;
				newCapacitor.charge = capacitor.charge;
				item.TurnToAir();
				return true;
			}
			return false;
		}

		public override void MouseOver(int i, int j)
		{
			CapacitorRackTE tileEntity = GetTileEntity(i, j);
			if (tileEntity == null) return;
			if (j != tileEntity.Position.Y)
            {
				return;
            }
			Item item = tileEntity.items[i-tileEntity.Position.X];
			Capacitor modItem = item.ModItem as Capacitor;
			Player player = Main.LocalPlayer;
			player.noThrow = 2;
			if ((item != null) && (!item.IsAir))
			{
				player.cursorItemIconEnabled = true;
				player.cursorItemIconText = modItem.charge + "/" + modItem.maxcharge;
				player.cursorItemIconID = item.type;
			}
		}

        public override void HitWire(int i, int j)
		{
			CapacitorRackTE tileEntity = GetTileEntity(i, j);
			Point16 subTile = new Point16(i, j) - tileEntity.Position;
			if (subTile.X == 1 && subTile.Y == 1 && !tileEntity.discharged)
            {
				tileEntity.discharged = true;
			}
		}

		public override bool IsConsumer(int i, int j)
		{
			CapacitorRackTE tileEntity = GetTileEntity(i, j);
			Point16 subTile = new Point16(i, j) - tileEntity.Position;
			if (subTile.X != 1 && subTile.Y == 1)
			{
				Main.NewText("Is Consumer");
				return true;
			}
			return false;
		}

        public override void InsertPower(int i, int j, int amount)
		{
			CapacitorRackTE tileEntity = GetTileEntity(i, j);
			Point16 subTile = new Point16(i, j) - tileEntity.Position;
			if (subTile.X != 1 && subTile.Y == 1)
			{
				for (int c = 0; c < amount; c++)
				{
					bool charged = false;
					for (int x = 0; x < 3 && !charged; x++)
					{
						tileEntity.lastCharged++;
						tileEntity.lastCharged %= 3;
						Capacitor capacitor = tileEntity.items[tileEntity.lastCharged].ModItem as Capacitor;
						if (capacitor == null)
						{
							Main.NewText("Item is null");
							continue;
						}
						if (capacitor.Charge(1) == 1)
							charged = true;
					}
				}
				return;
			}
		}

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
			Tile tile = Main.tile[i, j];
			if (tile.TileFrameY != 0)
            {
				return;
            }
			Vector2 TileOffset = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
			Vector2 pos = new Vector2(i, j) * 16 - Main.screenPosition + TileOffset;

			CapacitorRackTE tileEntity = GetTileEntity(i, j);
			if (tileEntity == null) return;
			int c = tile.TileFrameX / 18;
				Item item = tileEntity.items[c];
				if (item == null || item.IsAir)
					return;
				spriteBatch.Draw(ModContent.Request<Texture2D>("Techarria/Content/Tiles/Capacitors/" + capacitorTextures[item.type]).Value, new Rectangle((int)pos.X, (int)pos.Y, 16, 16), Color.White);
		}
    }
}
