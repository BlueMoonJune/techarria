using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Techarria.Content.Dusts;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;

namespace Techarria.Content.Tiles
{
    public class GelatinousTurbineTE : ModTileEntity
    {
		public Item item;
		public int burnTime = 0;
		public float pulseFraction = 0;
		public int frame = 0;
		public static Rectangle particleRect = new Rectangle(6, 6, 24, 16);

        public override bool IsTileValidForEntity(int x, int y)
        {
            return Main.tile[x, y].TileType == ModContent.TileType<GelatinousTurbine>();
        }

        public override void Update()
        {
			Dust.NewDust(new Vector2(Position.X, Position.Y) * 16, 0, 0, ModContent.DustType<TransferDust>());
			if (item == null)
			{
				item = new Item();
			}

			if (burnTime <= 0)
			{
				if (item.type == ItemID.Gel)
					burnTime = 600;
				if (item.type == ItemID.PinkGel)
					burnTime = 3600;
				item.stack--;
				if (item.stack <= 0)
                {
					item.TurnToAir();
                }
			}

			if (burnTime > 0)
            {
				for (int i = 0; i < (int)Techarria.GenerationMultiplier; i++)
                {
					Wiring.TripWire(Position.X, Position.Y, 3, 2);
					Power.TransferCharge(1, Position.X, Position.Y, 3, 2);
                }
				pulseFraction += Techarria.GenerationMultiplier % 1f;
				if (pulseFraction >= 1)
                {
					pulseFraction -= 1;
					Power.TransferCharge(1, Position.X, Position.Y, 3, 2);
				}
				burnTime--;
            }

			if (burnTime % 15 == 1)
			{
				frame = (frame + 1) % 4;
				for (int i = 0; i < 3; i++)
				{
					for (int j = 0; j < 2; j++)
					{
						Tile tile = Main.tile[Position.X + i, Position.Y + j];
						tile.TileFrameX = (short)(54 + 18 * i);
						tile.TileFrameY = (short)(frame * 36 + j * 18);
                    }
                }
            }

			if (burnTime == 0)
			{
				for (int i = 0; i < 3; i++)
				{
					for (int j = 0; j < 2; j++)
					{
						Tile tile = Main.tile[Position.X + i, Position.Y + j];
						tile.TileFrameX = (short)(i * 18);
						tile.TileFrameY = (short)(j * 18);
					}
				}
			}
			else
			{

				if (new Random().Next(10) == 0)
				{
					Dust dust = Dust.NewDustDirect(new Vector2(Position.X, Position.Y) * 16 + particleRect.TopLeft(), particleRect.Width, particleRect.Height, DustID.Flare);
					//dust.velocity = new Vector2(0, -1);
					//dust.alpha = 127;
				}
				if (new Random().Next(3) == 0)
				{
					Dust dust = Dust.NewDustDirect(new Vector2(Position.X, Position.Y) * 16 + particleRect.TopLeft(), particleRect.Width, particleRect.Height, DustID.Smoke);
					dust.velocity = new Vector2(0, -1);
					dust.alpha = 127;
				}
			}
        }

        public override void SaveData(TagCompound tag)
        {
			tag.Add("item", item);
			tag.Add("burnTime", burnTime);
			base.SaveData(tag);
        }

        public override void LoadData(TagCompound tag)
        {
			item = tag.Get<Item>("item");
			burnTime = tag.GetInt("burnTime");
            base.LoadData(tag);
        }
    }

    public class GelatinousTurbine : ModTile
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

			ItemDrop = ModContent.ItemType<Items.Placeables.GelatinousTurbine>();
		}

		public GelatinousTurbineTE GetTileEntity(int i, int j)
		{
			Tile tile = Framing.GetTileSafely(i, j);
			i -= tile.TileFrameX / 18 % 3;
			j -= tile.TileFrameY / 18 % 2;
			return TileEntity.ByPosition[new Point16(i, j)] as GelatinousTurbineTE;
		}

        public override void PlaceInWorld(int i, int j, Item item)
        {
			Tile tile = Framing.GetTileSafely(i, j);
			i -= tile.TileFrameX / 18 % 3;
			j -= tile.TileFrameY / 18 % 2;
			Main.NewText(i + " " + j);
			ModContent.GetInstance<GelatinousTurbineTE>().Place(i, j);
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{
			Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 48, 64, ModContent.ItemType<Items.Placeables.ExampleTable>());
			ModContent.GetInstance<GelatinousTurbineTE>().Kill(i, j);
		}

        public static bool AcceptsItem(Item item)
        {
			return item.type == Terraria.ID.ItemID.Gel || item.type == Terraria.ID.ItemID.PinkGel;
        }

        public override bool RightClick(int i, int j)
        {
			Main.NewText("RightClick");
			GelatinousTurbineTE tileEntity = GetTileEntity(i, j);
			Item item = tileEntity.item;
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
				tileEntity.item = item;
				playerItem.stack--;
				if (playerItem.stack <= 0)
                {
					playerItem.TurnToAir();
                }
				return true;
            }
			if (!item.IsAir && playerItem.type == item.type && item.stack < item.maxStack)
			{
				Main.NewText("Item Matches: Insert");
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
				Main.NewText("Item Does not Match: Extract");
				item.stack--;
				if (item.stack <= 0)
				{
					item.TurnToAir();
				}
				Item.NewItem(new EntitySource_TileInteraction(Main.player[Main.myPlayer], i, j), i * 16, j * 16, 32, 32, item.type);
				return true;
			}
			return false;
		}
		public override void MouseOver(int i, int j)
		{
			GelatinousTurbineTE tileEntity = GetTileEntity(i, j);
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
	}
}
