using Microsoft.Xna.Framework;
using System;
using Techarria.Content.Dusts;
using Techarria.Content.Items.Materials;
using Techarria.Content.Items.Materials.Molten;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;

namespace Techarria.Content.Tiles
{
    public class CastingTableTE : ModTileEntity
    {
		public Item item = new Item();
		public Item mold = new Item();
		public float temp;

        public override bool IsTileValidForEntity(int x, int y)
        {
            return Main.tile[x, y].TileType == ModContent.TileType<CastingTable>();
        }

		public bool IsCovered()
        {
			Tile LeftTile = Main.tile[Position.X, Position.Y - 1];
			Tile RightTile = Main.tile[Position.X + 1, Position.Y - 1];
			return (
				LeftTile.HasTile && 
				Main.tileSolid[LeftTile.TileType] && 
				!Main.tileSolidTop[LeftTile.TileType] && 
				RightTile.HasTile && 
				Main.tileSolid[RightTile.TileType] && 
				!Main.tileSolidTop[RightTile.TileType]
			);
        }

		public bool InsertMolten(Item _item)
        {
			MoltenBlob blob = item.ModItem as MoltenBlob;
			if (!IsCovered())
            {
				WorldGen.PlaceLiquid(Position.X, Position.Y - 1, LiquidID.Lava, 32);
				WorldGen.PlaceLiquid(Position.X + 1, Position.Y - 1, LiquidID.Lava, 32);
				temp = (temp + blob.temp) / 2;
				item = _item;
				return true;
			}

			if (item.IsAir)
			{
				item = _item;
				return true;
			}

			return false;
        }

        public override void Update()
        {
		}

        public override void SaveData(TagCompound tag)
        {
			tag.Add("item", item);
			tag.Add("mold", mold);
			base.SaveData(tag);
        }

        public override void LoadData(TagCompound tag)
        {
			item = tag.Get<Item>("item");
			mold = tag.Get<Item>("mold");
			if (item.ModItem is MoltenBlob blob)
				temp = blob.temp;
            base.LoadData(tag);
        }
    }

	// Where the TE ends and the Tile starts
    public class CastingTable : ModTile
    {
		public override void SetStaticDefaults()
		{
			Main.tileLavaDeath[Type] = false;
			Main.tileNoAttach[Type] = true;
			Main.tileFrameImportant[Type] = true;
			TileID.Sets.DisableSmartCursor[Type] = true;
			TileID.Sets.IgnoredByNpcStepUp[Type] = true; // This line makes NPCs not try to step up this tile during their movement. Only use this for furniture with solid tops.

			DustType = ModContent.DustType<Dusts.Wormhole>();
			AdjTiles = new int[] { TileID.Tables };

			// Placement
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x1);
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.LavaDeath = false;
			TileObjectData.addTile(Type);

			// Etc
			ModTranslation name = CreateMapEntryName();
			name.SetDefault("Player Interface");
			AddMapEntry(new Color(200, 200, 200), name);
		}

		public CastingTableTE GetTileEntity(int i, int j)
		{
			Tile tile = Framing.GetTileSafely(i, j);
			i -= tile.TileFrameX / 18 % 2;
			j -= tile.TileFrameY / 18 % 1;
			return TileEntity.ByPosition[new Point16(i, j)] as CastingTableTE;
		}

        public override void PlaceInWorld(int i, int j, Item item)
        {
			Tile tile = Framing.GetTileSafely(i, j);
			i -= tile.TileFrameX / 18 % 2;
			j -= tile.TileFrameY / 18 % 1;
			ModContent.GetInstance<CastingTableTE>().Place(i, j);
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{
			CastingTableTE tileEntity = GetTileEntity(i, j);
			Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 16, ModContent.ItemType<Items.Placeables.CastingTable>());
			Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 16, tileEntity.item);
			Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 16, tileEntity.mold);
			ModContent.GetInstance<CastingTableTE>().Kill(i, j);
		}

        public static bool AcceptsItem(Item item)
        {
			return item.type == Terraria.ID.ItemID.Gel || item.type == Terraria.ID.ItemID.PinkGel;
        }

        public override bool RightClick(int i, int j)
        {
			CastingTableTE tileEntity = GetTileEntity(i, j);
			Item item = tileEntity.item;
			Item playerItem;
			if (!Main.mouseItem.IsAir)
            {
				playerItem = Main.mouseItem;
            } else
            {
				playerItem = Main.player[Main.myPlayer].HeldItem;
            }

			if (playerItem.ModItem is Mold && tileEntity.mold.IsAir)
            {
				tileEntity.mold = playerItem.Clone();
				playerItem.TurnToAir();
				return true;
			}
			if (playerItem.ModItem is MoltenBlob)
            {
				Item tempItem = playerItem.Clone();
				tempItem.stack = 1;
				if (tileEntity.InsertMolten(tempItem))
                {
					playerItem.stack--;
					if (playerItem.stack <= 0)
                    {
						playerItem.TurnToAir();
                    }
					return true;
                }
            }
			if (!tileEntity.item.IsAir)
			{
				Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 16, tileEntity.item);
				tileEntity.item.TurnToAir();
				return true;
			}

			if (!tileEntity.mold.IsAir)
			{
				Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 16, tileEntity.mold);
				tileEntity.mold.TurnToAir();
				return true;
			}

			return false;
		}
		public override void MouseOver(int i, int j)
		{
			CastingTableTE tileEntity = GetTileEntity(i, j);
			Item item = tileEntity.item;
			Player player = Main.LocalPlayer;
			player.noThrow = 2;
			if (!item.IsAir && !(item.ModItem is MoltenBlob))
			{
				player.cursorItemIconEnabled = true;
				player.cursorItemIconID = item.type;
			}
			if (!tileEntity.mold.IsAir)
			{
				player.cursorItemIconEnabled = true;
				player.cursorItemIconID = tileEntity.mold.type;
			}
		}
	}
}
