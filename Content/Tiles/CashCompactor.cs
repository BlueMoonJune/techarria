using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;
using Terraria.UI.Chat;

namespace Techarria.Content.Tiles
{
	public class CashCompactorTE : ModTileEntity
    {
		public static int[] coinTypes = new int[4] { ItemID.CopperCoin, ItemID.SilverCoin, ItemID.GoldCoin, ItemID.PlatinumCoin };
		public static int[] coinValues = new int[4] { 1, 100, 10000, 1000000 };

		public int amount = 0;
		public override bool IsTileValidForEntity(int x, int y)
        {
            return Main.tile[x, y].TileType == ModContent.TileType<CashCompactor>();
        }

		public int[] GetCompactedCoinCounts() {
			int num = amount;
			int[] coins = new int[4];
			if (num >= 1000000) {
				coins[3] = num / 1000000;
				num %= 1000000;
			}
			if (num >= 10000) {
				coins[2] = num / 10000;
				num %= 10000;
			}
			if (num >= 100) {
				coins[1] = num / 100;
				num %= 100;
			}
			coins[0] = num;
			return coins;
		}

		public int[] GetPossibleCoinCounts() {
			int num = amount;
			int[] coins = new int[4];
			coins[3] = num / 1000000;
			coins[2] = num / 10000;
			coins[1] = num / 100;
			coins[0] = num;
			return coins;
		}

		public Item[] GetCompactedCoins() {
			Item[] coins = new Item[4];
			int[] counts = GetCompactedCoinCounts();
			for (int i = 0; i < 4; i++) {
				coins[i] = new Item(coinTypes[i], counts[i]);
			}
			return coins;
		}

		public Item[] GetPossibleCoins() {
			Item[] coins = new Item[4];
			int[] counts = GetPossibleCoinCounts();
			for (int i = 0; i < 4; i++) {
				coins[i] = new Item(coinTypes[i], counts[i]);
			}
			return coins;
		}

		public override void SaveData(TagCompound tag)
		{
			tag.Add("amount", amount);
			base.SaveData(tag);
		}

		public override void LoadData(TagCompound tag)
		{
			amount = tag.Get<int>("amount");
			base.LoadData(tag);
		}
	}

	// Where the TE ends and the Tile starts
	public class CashCompactor : ModTile
	{
		public override void SetStaticDefaults()
		{
			// Spelunker
			Main.tileSpelunker[Type] = true;

			// Properties
			Main.tileLavaDeath[Type] = false;
			Main.tileFrameImportant[Type] = true;
			TileID.Sets.DisableSmartCursor[Type] = true;
			TileID.Sets.IgnoredByNpcStepUp[Type] = true;
			TileID.Sets.PreventsTileRemovalIfOnTopOfIt[Type] = true;

			Main.tileFrameImportant[Type] = true;

			// placement
			TileObjectData.newTile.CopyFrom(TileObjectData.Style4x2);
			TileObjectData.newTile.Origin = new Point16(0, 1);
			TileObjectData.newTile.AnchorInvalidTiles = new int[] { TileID.MagicalIceBlock };
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.LavaDeath = false;
			TileObjectData.addTile(Type);
		}
		public static CashCompactorTE GetTileEntity(int i, int j)
		{
			Tile tile = Framing.GetTileSafely(i, j);
			i -= tile.TileFrameX / 18 % 4;
			j -= tile.TileFrameY / 18 % 2;
			TileEntity.ByPosition.TryGetValue(new Point16(i, j), out TileEntity te);
			return te as CashCompactorTE;
		}
		public override void PlaceInWorld(int i, int j, Item item)
		{
			Tile tile = Framing.GetTileSafely(i, j);
			i -= tile.TileFrameX / 18 % 4;
			j -= tile.TileFrameY / 18 % 2;
			ModContent.GetInstance<CashCompactorTE>().Place(i, j);
		}

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{
			CashCompactorTE tileEntity = GetTileEntity(i, j);
			Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 64, 32, ModContent.ItemType<Items.Placeables.CashCompactor>());
			foreach (Item coin in tileEntity.GetCompactedCoins()) {
				Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 64, 32, coin);
			}
			ModContent.GetInstance<CashCompactorTE>().Kill(i, j);
		}

		public override bool RightClick(int i, int j)
		{
			CashCompactorTE tileEntity = GetTileEntity(i, j);
			Point16 subTile = new Point16(i, j) - tileEntity.Position;
			Item playerItem;
			if (!Main.mouseItem.IsAir)
			{
				playerItem = Main.mouseItem;
			}
			else
			{
				playerItem = Main.player[Main.myPlayer].HeldItem;
			}

			List<int> coinTypes = new List<int>(CashCompactorTE.coinTypes);
			int index = coinTypes.FindIndex(value => value == playerItem.type);
			if (index >= 0) {
				tileEntity.amount += CashCompactorTE.coinValues[index];
				playerItem.stack--;
				if (playerItem.stack <= 0) {
					playerItem.TurnToAir();
				}
				return true;
			}

			if (subTile.Y == 1) {
				Item[] coins = tileEntity.GetPossibleCoins();
				if (coins[subTile.X].stack > 0) {
					Item.NewItem(new EntitySource_TileInteraction(Main.LocalPlayer, i, j), i * 16, j * 16, 16, 16, coins[subTile.X].type);
					tileEntity.amount -= CashCompactorTE.coinValues[subTile.X];
				}
			}

			return false;
		}


		public override void MouseOver(int i, int j)
		{
			CashCompactorTE tileEntity = GetTileEntity(i, j);
			Player player = Main.LocalPlayer;
			player.noThrow = 2;

			string str = "";
			Item[] coins = tileEntity.GetCompactedCoins();
			for (int k = coins.Length - 1; k >= 0; k--) {
				Item item = coins[k];
				if (item.stack > 0) {
					str += $"[i:{item.type}]{item.stack}";
				}
			}
			{
				player.cursorItemIconEnabled = true;
				player.cursorItemIconText = str;
			}
		}
		public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
		{
			CashCompactorTE tileEntity = GetTileEntity(i, j);
			Point16 subTile = new Point16(i, j) - tileEntity.Position;	
			if (subTile.X == 3 && subTile.Y == 1)
			{

				Vector2 TileOffset = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
				Vector2 pos = new Vector2(i, j) * 16 - Main.screenPosition + TileOffset;

				DynamicSpriteFont font = FontAssets.MouseText.Value;

				string text = "";
				Item[] coins = tileEntity.GetCompactedCoins();
				for (int k = coins.Length - 1; k >= 0; k--) {
					Item item = coins[k];
					if (item.stack > 0) {
						text += $"[i:{item.type}]{item.stack}";
						break;
					}
				}

				Vector2 size = font.MeasureString(text);
				Vector2 textScale = Vector2.One / Math.Max(size.X / 48, size.Y / 16);
				ChatManager.DrawColorCodedStringWithShadow(spriteBatch, font, text, -size / 2 * textScale + pos + new Vector2(-16, -4), Color.White, 0, Vector2.Zero, textScale);

			}

		}
	}
}
