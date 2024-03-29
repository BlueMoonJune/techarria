﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.UI;

namespace Techarria
{
	public class RecipeIngredient {
		public Item item = new();
		public RecipeGroup recipeGroup;
		public int count;

		public RecipeIngredient(Item item, int count = 1) {
			this.item = item;
			this.count = count;
		}

		public RecipeIngredient(RecipeGroup recipeGroup, int count = 1) {
			this.recipeGroup = recipeGroup;
			this.count = count;
		}

		public bool AcceptsItem(Item item) {
			return 
				this.item != null &&
				item.type == this.item.type || 
				recipeGroup != null &&
				recipeGroup.ContainsItem(item.type);
		}

		public override string ToString() {
			return $"{{Item: {item}}}";
		}
	}

	public static class HelperMethods {

		public static int[] tempValues = new int[5] {5, 10, 40, 500, 550};
		public static Vector2 vZero = Vector2.Zero;

		public static int Int(this bool b) => b ? 1 : 0;

		public static Vector2 SafeNormalized(this ref Vector2 v, Vector2 def)
		{
			if (v == Vector2.Zero)
			{
				v = def;
				return v;
			}
			float length = MathF.Sqrt(v.X * v.X + v.Y * v.Y);
			v.X /= length;
			v.Y /= length;
			return v;
			
		}

		public static float Dot(this Vector2 a, Vector2 b) {
			return a.X * b.X + a.Y * b.Y;
		}

		public static Vector2 ToVector2(this Point p) {
			return new Vector2(p.X, p.Y);
		}

		public static int GetBaseTemp(int y) 
		{
			int[] yValues = new int[5] { 0, (int)(Main.worldSurface * 0.35f), (int)Main.worldSurface, Main.UnderworldLayer, Main.maxTilesY };

			float output = 0;
			for (int n = 0; n < 5; n++) {
				float aux = 1;
				for (int m = 0; m < 5; m++) {
					if (m != n) {
						aux *= (y - yValues[m]) / (float)(yValues[n] - yValues[m]);
					}
				}
				output += tempValues[n] * aux;
			}
			return (int)output;
		}

		// shoutout to absoluteAquarian#5189 on discord
		public static void DrawItemInWorld(this SpriteBatch spriteBatch, Item item, Vector2 position, float size, float rotation = 0f)
		{
			if (!item.IsAir)
			{
				Texture2D itemTexture = TextureAssets.Item[item.type].Value;
				
				Rectangle rect = Main.itemAnimations[item.type] != null ? Main.itemAnimations[item.type].GetFrame(itemTexture) : itemTexture.Frame();
				Color newColor = Color.White;
				float pulseScale = 1f;
				ItemSlot.GetItemLight(ref newColor, ref pulseScale, item, outInTheWorld: true);

				int width = rect.Width;
				int height = rect.Height;
				float drawScale = 1f;
				if (width > size || height > size)
				{
					if (width > height)
						drawScale = size / width;
					else
						drawScale = size / height;
				}

				Vector2 origin = rect.Size() * 0.5f;

				float totalScale = pulseScale * drawScale;

				if (ItemLoader.PreDrawInWorld(item, spriteBatch, item.GetColor(Color.White), item.GetAlpha(newColor), ref rotation, ref totalScale, item.whoAmI))
				{
					spriteBatch.Draw(itemTexture, position, rect, item.GetAlpha(newColor), rotation, origin, totalScale, SpriteEffects.None, 0f);

					if (item.color != Color.Transparent)
						spriteBatch.Draw(itemTexture, position, rect, item.GetColor(Color.White), rotation, origin, totalScale, SpriteEffects.None, 0f);
				}

				ItemLoader.PostDrawInWorld(item, spriteBatch, item.GetColor(Color.White), item.GetAlpha(newColor), rotation, totalScale, item.whoAmI);

				if (ItemID.Sets.TrapSigned[item.type])
					spriteBatch.Draw(TextureAssets.Wire.Value, position + new Vector2(40f, 40f) * drawScale, new Rectangle(4, 58, 8, 8), Color.White, 0f, new Vector2(4f), drawScale, SpriteEffects.None, 0f);
			}
		}

		//absoluteAquarian being a god once again

		/// <summary>
		/// Atttempts to find the top-left corner of a multitile at location (<paramref name="x"/>, <paramref name="y"/>)
		/// </summary>
		/// <param name="x">The tile X-coordinate</param>
		/// <param name="y">The tile Y-coordinate</param>
		/// <returns>The tile location of the multitile's top-left corner, or the input location if no tile is present or the tile is not part of a multitile</returns>
		public static Point GetTopLeftTileInMultitile(int x, int y, out int width, out int height)
		{
			Tile tile = Main.tile[x, y];

			int frameX = 0;
			int frameY = 0;
			width = 1;
			height = 1;

			if (tile.HasTile)
			{
				int style = 0, alt = 0;
				TileObjectData.GetTileInfo(tile, ref style, ref alt);
				TileObjectData data = TileObjectData.GetTileData(tile.TileType, style, alt);

				if (data != null)
				{
					int size = 16 + data.CoordinatePadding;

					frameX = tile.TileFrameX % (size * data.Width) / size;
					frameY = tile.TileFrameY % (size * data.Height) / size;
					width = data.Width;
					height = data.Height;
				}
			}

			return new Point(x - frameX, y - frameY);
		}

	}
}
