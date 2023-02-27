using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Windows.Forms;
using Techarria.Content.Dusts;
using Techarria.Content.Items.RecipeItems;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;


namespace Techarria.Content.Tiles.Machines
{
	public class CryoChamberTE : ModTileEntity
	{
		public Item output = new();
		public Item item = new();
		public int bannerEnemyId = 0;
		public NPC madeUpNPC;
		public int powerNeeded;
		public float progress = 0;
		public int frame = 0;
		public static Rectangle particleRect = new(6, 6, 24, 16);

		public int oldY = 0;
		public override bool IsTileValidForEntity(int x, int y)
		{
			return Main.tile[x, y].TileType == ModContent.TileType<CryoChamber>();
		}

		public override void SaveData(TagCompound tag)
		{
			tag.Add("item", item);
			//tag.Add("frozen dude", madeUpNPC);
			//tag.Add("power needed", powerNeeded);
			base.SaveData(tag);
		}

		public override void LoadData(TagCompound tag)
		{
			item = tag.Get<Item>("item");
			//tag.Add("frozen dude", madeUpNPC);
			//tag.Add("power needed", powerNeeded);
			base.LoadData(tag);
		}
	}

	public class CryoChamber : PowerConsumer
	{
		public override void SetStaticDefaults()
		{
			Main.tileNoAttach[Type] = true;
			Main.tileFrameImportant[Type] = true;
			Main.tileLavaDeath[Type] = false;
			TileID.Sets.DisableSmartCursor[Type] = true;
			TileID.Sets.IgnoredByNpcStepUp[Type] = true; // This line makes NPCs not try to step up this tile during their movement. Only use this for furniture with solid tops.

			DustType = DustID.IceGolem;
			AdjTiles = new int[] { TileID.Tables };

			// Placement
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x4);
			TileObjectData.newTile.StyleHorizontal = false;
			TileObjectData.newTile.LavaDeath = false;
			TileObjectData.addTile(Type);

			// Etc
			ModTranslation name = CreateMapEntryName();
			name.SetDefault("Cryo Chamber");
			AddMapEntry(new Color(200, 200, 200), name);
		}

		public static CryoChamberTE GetTileEntity(int i, int j)
		{
			Tile tile = Framing.GetTileSafely(i, j);
			i -= tile.TileFrameX / 18 % 3;
			j -= tile.TileFrameY / 18 % 4;
			return TileEntity.ByPosition[new Point16(i, j)] as CryoChamberTE;
		}

		public override void PlaceInWorld(int i, int j, Item item)
		{
			Tile tile = Framing.GetTileSafely(i, j);
			i -= tile.TileFrameX / 18 % 3;
			j -= tile.TileFrameY / 18 % 4;
			ModContent.GetInstance<CryoChamberTE>().Place(i, j);
		}

		public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{
			Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 48, 64, ModContent.ItemType<Items.Placeables.Machines.CryoChamber>());

			CryoChamberTE tileEntity = GetTileEntity(i, j);
			Item.NewItem(new EntitySource_TileBreak(i, j), new Rectangle(i * 16, j * 16, 48, 64), tileEntity.item.type, tileEntity.item.stack);

			ModContent.GetInstance<CryoChamberTE>().Kill(i, j);
		}

		public override bool RightClick(int i, int j)
		{
			CryoChamberTE tileEntity = GetTileEntity(i, j);
			Item item = tileEntity.item;
			Point16 subTile = new Point16(i, j) - tileEntity.Position;
			if (subTile.X <= 2 && subTile.Y <= 2)
			{
				Item playerItem;
				if (!Main.mouseItem.IsAir)
					playerItem = Main.mouseItem;
				else
					playerItem = Main.player[Main.myPlayer].HeldItem;

				if (item.IsAir)
				{
					// checks for enemy banners
					if(playerItem.createTile == TileID.Banners && playerItem.placeStyle > 21)
                    {
						item = playerItem.Clone();
						item.stack = 1;
						tileEntity.item = item;
						playerItem.stack--;

						// banner to enemy
						tileEntity.bannerEnemyId = Item.BannerToNPC(item.placeStyle - 21);
						Main.NewText(tileEntity.bannerEnemyId);

						// sets NPC madeUpNPC to the npcID from the banner
						tileEntity.madeUpNPC = ContentSamples.NpcsByNetId[tileEntity.bannerEnemyId];
						// determines how much power is needed for 1 item roll
						tileEntity.powerNeeded = (tileEntity.madeUpNPC.lifeMax * tileEntity.madeUpNPC.damage);


						if (playerItem.stack <= 0)
						{ 
							playerItem.TurnToAir();
						}
						return true;
					} else if (!playerItem.IsAir)
                    {
						MessageBox.Show("You need to input a vanilla enemy banner" +
                            "\nSorry for no mod support :(" +
                            "\nBut hey, atleast I figured out how to make these cool message boxes", "Read tooltips next time (that message was for anyone who wasn't trying to input a modded enemy banner, if you were you're perfect and did nothing wrong)",
						MessageBoxButtons.OK, MessageBoxIcon.Warning);

					}
				}
				if (!item.IsAir)
				{
					item.stack--;
					Item dropItem = item.Clone();
					dropItem.stack = 1;
					Item.NewItem(new EntitySource_TileInteraction(Main.player[Main.myPlayer], i, j), new Rectangle(i * 16, j * 16, 32, 32), dropItem);

					// banner data removal
					tileEntity.bannerEnemyId = 0;
					tileEntity.madeUpNPC = ContentSamples.NpcsByNetId[0];
					tileEntity.progress = 0;
					tileEntity.powerNeeded = 0;
					Main.NewText(tileEntity.bannerEnemyId);

					if (item.stack <= 0)
					{
						item.TurnToAir();
					}
					return true;
				}
			}

			return false;

		}
		public override void MouseOver(int i, int j)
		{
			CryoChamberTE tileEntity = GetTileEntity(i, j);
			
			Item item = tileEntity.item;
			Point16 subTile = new Point16(i, j) - tileEntity.Position;
			Player player = Main.LocalPlayer;

			Item playerItem;

			if (!Main.mouseItem.IsAir)
				playerItem = Main.mouseItem;
			else
				playerItem = Main.player[Main.myPlayer].HeldItem;

			player.noThrow = 2;
			if (subTile.X <= 2 && subTile.Y >= 3)
			{
				player.cursorItemIconEnabled = true;
				player.cursorItemIconText = tileEntity.progress + " / " + tileEntity.powerNeeded;
				player.cursorItemIconID = ModContent.ItemType<Items.RecipeItems.Power>();
			}

			if (item != null && !item.IsAir && subTile.X <= 2 && subTile.Y <= 2)
			{
				player.cursorItemIconEnabled = true;
				player.cursorItemIconText = "" + item.Name;
				player.cursorItemIconID = item.type;
			}
		}

        public override void InsertPower(int i, int j, int amount)
		{
			CryoChamberTE tileEntity = GetTileEntity(i, j);

			if (tileEntity.bannerEnemyId != 0)
            {
				tileEntity.progress += amount;
				if (tileEntity.progress >= tileEntity.powerNeeded)
				{
					tileEntity.madeUpNPC.position = new Vector2(i * 16 + 8, (j - 2) * 16 + 8);

					DropAttemptInfo info = new()
					{
						player = Main.LocalPlayer,
						npc = tileEntity.madeUpNPC,
						IsExpertMode = Main.expertMode,
						IsMasterMode = Main.masterMode,
						rng = Main.rand,
					};
					Main.ItemDropSolver.TryDropping(info);
					tileEntity.progress -= tileEntity.powerNeeded;
				}
			}
			
		}
	}
}
