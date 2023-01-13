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

namespace Techarria.Content.Tiles
{
    internal class CapacitorRackTE : ModTileEntity
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
					for (int c = 0; c < capacitor.charge; c++)
					{
						Console.WriteLine(Position.X + x + " " + Position.Y + " " + c);
						Wiring.TripWire(Position.X + x, Position.Y, 1, 1);
					}
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

    internal class CapacitorRack : ModTile
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
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.addTile(Type);

			// Etc
			ModTranslation name = CreateMapEntryName();
			name.SetDefault("Player Interface");
			AddMapEntry(new Color(200, 200, 200), name);

			ItemDrop = ModContent.ItemType<Items.Placeables.CapacitorRack>();
		}

		public CapacitorRackTE GetTileEntity(int i, int j)
		{
			Tile tile = Framing.GetTileSafely(i, j);
			i -= tile.TileFrameX / 18 % 3;
			j -= tile.TileFrameY / 18 % 2;
			return TileEntity.ByPosition[new Point16(i, j)] as CapacitorRackTE;
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
			Tile tile = Framing.GetTileSafely(i, j);
			i -= tile.TileFrameX / 18 % 3;
			j -= tile.TileFrameY / 18 % 2;
			Main.NewText(i + " " + j);
			ModContent.GetInstance<CapacitorRackTE>().Kill(i, j);
		}

		public static bool AcceptsItem(Item item)
        {
			return item.ModItem is Capacitor;
        }

        public override bool RightClick(int i, int j)
        {
			Main.NewText("RightClick");
			CapacitorRackTE tileEntity = GetTileEntity(i, j);
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
			if (subTile.X != 1 && subTile.Y == 1)
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
				return;
			}
			if (subTile.X == 1 && subTile.Y == 1 && !tileEntity.discharged)
            {
				tileEntity.discharged = true;
			}


		}
    }
}
