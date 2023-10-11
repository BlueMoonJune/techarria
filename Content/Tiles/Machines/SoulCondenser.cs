using Microsoft.CodeAnalysis.FlowAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Techarria.Content.Dusts;
using Techarria.Content.Items.RecipeItems;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;


namespace Techarria.Content.Tiles.Machines
{
	public class SoulCondenserTE : ModTileEntity
	{
		public static short BLOCKS_PER_SOUL = 50;

		public Item item = new();
		public float progress = 0;

		public static List<ushort> evilBlocks = new() { 23, 199, 25, 203, 112, 234, 163, 200, 400, 401, 398, 399, 661, 662 };
		public static List<ushort> pureBlocks = new() { 2, 1, 53, 161, 396, 397, 60 };
		public static List<ushort> hallowedBlocks = new() { 109, 164, 402, 403, 116, 117 };
		public static Dictionary<ushort, ushort> conversions = new()
		{
			{TileID.CorruptGrass, TileID.Grass},
            {TileID.CrimsonGrass, TileID.Grass},
            {TileID.HallowedGrass, TileID.Grass},
            {TileID.Ebonstone, TileID.Stone},
            {TileID.Crimstone, TileID.Stone},
            {TileID.Pearlstone, TileID.Stone},
            {TileID.Ebonsand, TileID.Sand},
            {TileID.Crimsand, TileID.Sand},
            {TileID.Pearlsand, TileID.Sand},
            {TileID.CorruptIce, TileID.IceBlock},
            {TileID.FleshIce, TileID.IceBlock},
            {TileID.HallowedIce, TileID.IceBlock},
            {TileID.CorruptSandstone, TileID.Sandstone},
            {TileID.CrimsonSandstone, TileID.Sandstone},
            {TileID.HallowSandstone, TileID.Sandstone},
            {TileID.CorruptHardenedSand, TileID.HardenedSand},
            {TileID.CrimsonHardenedSand, TileID.HardenedSand},
            {TileID.HallowHardenedSand, TileID.HardenedSand},
            {TileID.CorruptJungleGrass, TileID.JungleGrass},
            {TileID.CrimsonJungleGrass, TileID.JungleGrass},
        };

        public override bool IsTileValidForEntity(int x, int y)
		{
			return Main.tile[x, y].TileType == ModContent.TileType<SoulCondenser>();
		}

		public void InsertPower(int amount)
		{
			float center = Position.X + SoulCondenser.width / 2f;

            for (int i = 0; i < amount; i++)
			{
				float r = Main.rand.NextFloat(-1, 1);
				int x = (int)(center + (r*r*r * 20));
				int y = Main.rand.Next(Position.Y, Main.maxTilesY);

				if (x < 0 || x >= Main.maxTilesX) continue;
				Tile t = Main.tile[x, y];
				if (item.type != ItemID.SoulofLight && evilBlocks.Contains(t.TileType))
				{
					if (item.type != ItemID.SoulofNight)
					{
						item = new Item(ItemID.SoulofNight);
						item.stack = 0;
					}
					t.TileType = conversions[t.TileType];
					progress++;
				}

                if (item.type != ItemID.SoulofNight && hallowedBlocks.Contains(t.TileType))
                {
                    if (item.type != ItemID.SoulofLight)
                    {
                        item = new Item(ItemID.SoulofLight);
                        item.stack = 0;
                    }
                    t.TileType = conversions[t.TileType];
                    progress++;
                }

                if (progress > BLOCKS_PER_SOUL)
				{
					item.stack++;
					progress-=BLOCKS_PER_SOUL;
				}
			}
		}

		public override void SaveData(TagCompound tag)
		{
			tag.Add("item", item);
			tag.Add("progress", progress);
			base.SaveData(tag);
		}

		public override void LoadData(TagCompound tag)
		{
			item = tag.Get<Item>("item");
            //progress = tag.GetInt("progress");
			base.LoadData(tag);
		}
	}

	public class SoulCondenser : EntityTile<SoulCondenserTE>, IPowerConsumer
	{
		public static int width = 3;
        public static int height = 3;

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
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
			TileObjectData.newTile.StyleHorizontal = false;
			TileObjectData.newTile.LavaDeath = false;
			TileObjectData.addTile(Type);

			// Etc
			LocalizedText name = CreateMapEntryName();
			// name.SetDefault("Cryo Chamber");
			AddMapEntry(new Color(200, 200, 200), name);
		}

		public static SoulCondenserTE GetTileEntity(int i, int j)
		{
			Tile tile = Framing.GetTileSafely(i, j);
			i -= tile.TileFrameX / 18 % width;
			j -= tile.TileFrameY / 18 % height;
			return TileEntity.ByPosition[new Point16(i, j)] as SoulCondenserTE;
		}

		public override void PlaceInWorld(int i, int j, Item item)
		{
			Tile tile = Framing.GetTileSafely(i, j);
			i -= tile.TileFrameX / 18 % width;
			j -= tile.TileFrameY / 18 % height;
			ModContent.GetInstance<SoulCondenserTE>().Place(i, j);
		}

		public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{

			SoulCondenserTE tileEntity = GetTileEntity(i, j);
			Item.NewItem(new EntitySource_TileBreak(i, j), new Rectangle(i * 16, j * 16, width*16, height*16), tileEntity.item.type, tileEntity.item.stack);

			ModContent.GetInstance<SoulCondenserTE>().Kill(i, j);
		}

		public override bool RightClick(int i, int j)
		{
			SoulCondenserTE tileEntity = GetTileEntity(i, j);
			Item item = tileEntity.item;
			Point16 subTile = new Point16(i, j) - tileEntity.Position;

			if (!item.IsAir)
			{
				Item.NewItem(new EntitySource_TileInteraction(Main.player[Main.myPlayer], i, j), new Rectangle(i * 16, j * 16, 32, 32), item);
				item.TurnToAir();
				return true;
			}

			return false;

		}
		public override void MouseOver(int i, int j)
		{
			SoulCondenserTE tileEntity = GetTileEntity(i, j);
			
			Item item = tileEntity.item;
			Point16 subTile = new Point16(i, j) - tileEntity.Position;
			Player player = Main.LocalPlayer;

			player.noThrow = 2;
			if (subTile.X <= 2 && subTile.Y >= 2)
			{
				player.cursorItemIconEnabled = true;
				player.cursorItemIconText = tileEntity.progress + " / " + SoulCondenserTE.BLOCKS_PER_SOUL;
				player.cursorItemIconID = item.type;
				return;
			}

			if (item != null && !item.IsAir && subTile.X <= 2 && subTile.Y < 2)
			{
				player.cursorItemIconEnabled = true;
				player.cursorItemIconText = "" + item.stack;
				player.cursorItemIconID = item.type;
			}
		}

        public void InsertPower(int i, int j, int amount)
		{
			GetTileEntity(i, j).InsertPower(amount);
		}
	}
}
