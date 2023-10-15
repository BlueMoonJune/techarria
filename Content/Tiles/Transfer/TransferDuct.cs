using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Techarria.Content.Dusts;
using Terraria.Audio;
using Microsoft.Xna.Framework.Graphics;
using Techarria.Transfer;
using System;
using System.Collections.Generic;
using Techarria.Content.Sounds;

namespace Techarria.Content.Tiles.Transfer
{
	/// <summary>Basic item transportation tile. Other item transfer tiles should extend this</summary>
	public class TransferDuct : ModTile
	{

		/// <summary>That last direction that an item was sent in. Used for round-robin</summary>
		int[,] lastDir = new int[Main.maxTilesX, Main.maxTilesY];

		/// <summary>takes a direction and returns the X component of the coresponding vector</summary>
		/// <param name="dir">The input direction. 0 = right, 1 = down, 2 = left, 3 = up</param>
		/// <returns>The X component of the vector coresponding to 'dir'</returns>
		public static int dirToX(int dir) {
			switch (dir % 4) {
				case 0:
					return 1;
				case 2:
					return -1;
				default:
					return 0;
			}
		}

		/// <summary>takes a direction and returns the Y component of the coresponding vector</summary>
		/// <param name="dir">The input direction. 0 = right, 1 = down, 2 = left, 3 = up</param>
		/// <returns>The Y component of the vector coresponding to 'dir'</returns>
		public static int dirToY(int dir) {
			switch (dir % 4) {
				case 1:
					return 1;
				case 3:
					return -1;
				default:
					return 0;
			}
		}

		/// <summary>takes a direction and returns the coresponding vector</summary>
		/// <param name="dir">The input direction. 0 = right, 1 = down, 2 = left, 3 = up</param>
		/// <returns>The vector coresponding to 'dir'</returns>
		public static Point dirToVec(int dir) {
			return new Point(dirToX(dir), dirToY(dir));
		}

		/// <summary>Takes a point and returns the coresponding direction</summary>
		/// <param name="dir">The input point</param>
		/// <returns>The coresponding direction. 0 = right, 1 = down, 2 = left, 3 = up</returns>
		public static int posToDir(Point vec) {
			int x = vec.X;
			int y = vec.Y;
			if (x == 1 && y == 0) {
				return 0;
			}
			if (x == 0 && y == 1) {
				return 1;
			}
			if (x == -1 && y == 0) {
				return 2;
			}
			return 3;
		}

		/// <summary>Takes a point and returns the coresponding direction</summary>
		/// <param name="dir">The input point</param>
		/// <returns>The coresponding direction. 0 = right, 1 = down, 2 = left, 3 = up</returns>
		public int posToDir(int x, int y) {
			if (x == 1 && y == 0) {
				return 0;
			}
			if (x == 0 && y == 1) {
				return 1;
			}
			if (x == -1 && y == 0) {
				return 2;
			}
			return 3;
		}

		/// <summary>Checks if the 2 tiles have matching paints. If one of the tiles has no paint, returns true</summary>
		/// <param name="x">X coordinate of tile 1</param>
		/// <param name="y">Y coordinate of tile 1</param>
		/// <param name="i">X coordinate of tile 2</param>
		/// <param name="j">Y coordinate of tile 2</param>
		/// <returns>Whether the 2 tiles have matching paints. If one of the tiles has no paint, will be true</returns>
		public bool MatchingPaint(int x, int y, int i, int j) {
			return
				Main.tile[x, y].TileColor == 0
				||
				Main.tile[i, j].TileColor == 0
				||
				Main.tile[i, j].TileColor == Main.tile[x, y].TileColor;
		}

		public override void SetStaticDefaults() {
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = false;
			Main.tileSolidTop[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileTable[Type] = true;
			Main.tileBlockLight[Type] = false;

			Techarria.tileIsTransferDuct[Type] = true;
			Techarria.tileConnectToPipe[Type] = true;

			AddMapEntry(new Color(58 / 255f, 61 / 255f, 66 / 255f), CreateMapEntryName());

			DustType = DustID.Stone;
			//ItemDrop = ModContent.ItemType<Items.Placeables.Transfer.TransferDuct>();

			HitSound = SoundID.Tink;
		}

		public override bool Slope(int i, int j) {
			return false;
		}

		/// <summary>
		/// Returns whether the source tile should connect to the target tile
		/// </summary>
		/// <param name="i">X coordinate of the target tile</param>
		/// <param name="j">Y coordinate of the target tile</param>
		/// <param name="x">X coordinate of the source tile</param>
		/// <param name="y">Y coordinate of the source tile</param>
		/// <returns></returns>
		public bool ShouldConnect(int i, int j, int x, int y) {
			return
				Techarria.tileConnectToPipe[Main.tile[i, j].TileType]
				&&
				MatchingPaint(x, y, i, j)
				||
				FindContainer(i, j) != null;

		}


		public override void PlaceInWorld(int i, int j, Item item)
		{
			Tile tile = Framing.GetTileSafely(i, j);
			tile.TileFrameX += (Int16)(new Random(j * Main.maxTilesX + i).Next(3) * 64);
		}

		public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak) {
			SetStaticDefaults();

			Tile tile = Framing.GetTileSafely(i, j);
			int variant = tile.TileFrameX / 64;
			tile.TileFrameX = 0;
			if (ShouldConnect(i + 1, j, i, j)) {
				tile.TileFrameX += 16;
			}
			if (ShouldConnect(i - 1, j, i, j)) {
				tile.TileFrameX += 32;
			}
			tile.TileFrameX += (short)(variant * 64);

			tile.TileFrameY = 0;
			if (ShouldConnect(i, j + 1, i, j)) {
				tile.TileFrameY += 16;
			}
			if (ShouldConnect(i, j - 1, i, j)) {
				tile.TileFrameY += 32;
			}

			//tile.TileFrameX += (Int16)(new Random(j * Main.maxTilesY + i).Next(3) * 64);

			return true;
		}
        

        /// <summary>
        /// Finds a container at the specified coordinates
        /// </summary>
        /// <param name="i">X coordinate</param>
        /// <param name="j">Y coordinate</param>
        /// <returns>The ContainerInterface object coresponding to the found container</returns>
        public ContainerInterface FindContainer(int i, int j) {
			return ContainerInterface.Find(i, j);
		}

		/// <summary>
		/// Finds a container adjacent to the specified coordinates
		/// </summary>
		/// <param name="i">X coordinate</param>
		/// <param name="j">Y coordinate</param>
		/// <returns>The ContainerInterface object coresponding to the found container</returns>
		public ContainerInterface FindAdjacentContainer(int i, int j) {
			ContainerInterface container;
			container = FindContainer(i + 1, j);
			if (container != null) {
				container.dir = 0;
				return container;
			}
			container = FindContainer(i, j + 1);
			if (container != null) {
				container.dir = 1;
				return container;
			}
			container = FindContainer(i - 1, j);
			if (container != null) {
				container.dir = 2;
				return container;
			}
			container = FindContainer(i, j - 1);
			if (container != null) {
				container.dir = 3;
				return container;
			}
			return container;
		}

		/// <summary>
		/// Checks to see if the tile at the specified coordinates is of the specified type. Does not check for extending types
		/// </summary>
		/// <typeparam name="T">The type to test for</typeparam>
		/// <param name="x">X coordinate</param>
		/// <param name="y">Y coordinate</param>
		/// <returns></returns>
		public bool IsMatchingTile<T>(int x, int y) where T : ModTile {
			return Main.tile[x, y].TileType == ModContent.TileType<T>();
		}

		/// <summary>
		/// Creates particles for item transfer
		/// </summary>
		/// <param name="x">X coordinate</param>
		/// <param name="y">Y coordinates</param>
		/// <param name="dir">Direction of item transfer. -1 causes no motion and is used for transfer failure</param>
		public virtual void CreateParticles(int x, int y, int dir) {
			if (!Main.rand.NextBool(60)) return;
			var dust1 = Dust.NewDustDirect(new Vector2(x, y) * 16 + new Vector2(4), 0, 0, ModContent.DustType<TransferDust>());
			var dust2 = Dust.NewDustDirect(new Vector2(x, y) * 16 + new Vector2(dirToX(dir), dirToY(dir)) * 8 + new Vector2(4), 0, 0, ModContent.DustType<TransferDust>());
			if (dir >= 0) {
				dust1.velocity = new Vector2(dirToX(dir), dirToY(dir));
				dust2.velocity = new Vector2(dirToX(dir), dirToY(dir));
				return;
			}
		}

		/// <summary>
		/// The pathfinding function for item transfer. This is a recursive algorithm that stops once a container is found
		/// </summary>
		/// <param name="x">X coordinate</param>
		/// <param name="y">Y coordinate</param>
		/// <param name="item">The item that is trying to be transfered</param>
		/// <param name="origin">The direction the item is being sent from</param>
		/// <param name="depth">How 'deep' the recursive algorithm has gone. Should cause breakage after a certain threshold</param>
		/// <returns>The container that was found, either by this call or passed back up from the recursion</returns>
		public virtual ContainerInterface EvaluatePath(int x, int y, Item item, int origin, int depth) {
			origin = (origin + 2) % 4;
			if (depth >= 256) {
				Main.LocalPlayer.PickTile(x, y, 40000);
			}
			ContainerInterface container = FindAdjacentContainer(x, y);
			if (container != null && !(container.dir == origin)) {
				CreateParticles(x, y, container.dir);
				return container;
			}
			for (int c = 0; c < 4; c++) {
				int dir = (c + lastDir[x, y] + 1) % 4;
				int i = x + dirToX(dir);
				int j = y + dirToY(dir);
				if (Techarria.tileIsTransferDuct[Main.tile[i, j].TileType] && MatchingPaint(x, y, i, j) && dir != origin) {
					ContainerInterface target = ((TransferDuct)TileLoader.GetTile(Main.tile[i, j].TileType)).EvaluatePath(x + dirToX(dir), y + dirToY(dir), item, dir, depth + 1);
					if (target != null) {
						lastDir[x, y] = dir;
						CreateParticles(x, y, dir);
						return target;
					}
				}
			}


			return null;
		}

		public override void HitWire(int i, int j) {
			ContainerInterface container = FindAdjacentContainer(i, j);
			if (container != null && !container.IsEmpty()) {
				if (Main.rand.NextBool(10)) {
					var suction = Dust.NewDustDirect(new Vector2(i + dirToX(container.dir), j + dirToY(container.dir)) * 16 + new Vector2(-4), 24, 24, ModContent.DustType<Suction>());
					suction.customData = new List<Vector2>() { new(i * 16 + 8, j * 16 + 8), new(i * 16 + 8 + dirToX(container.dir)*32, j * 16 + 8 + dirToY(container.dir)*32), };
					suction.velocity = new Vector2(-dirToX(container.dir), -dirToY(container.dir)) * 0.5f;
				}
				foreach (Item item in container.GetItems()) {
					if (item == null)
						continue;
					ContainerInterface target = EvaluatePath(i, j, item, (container.dir + 2) % 4, 0);
					if (target != null && target.InsertItem(item)) {
						TransferSound.PlaySound(new(i, j));
						container.ExtractItem(item);
						return;
					}
				}
			}
			else if (container == null) {
				Main.tile[i, j].TileType = (ushort)ModContent.TileType<DisabledTransferDuct>();
			}
		}

		public Rectangle[] GetExtensionDestination(int i, int j) {
			Vector2 TileOffset = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
			Vector2 pos = new Vector2(i, j) * 16 - Main.screenPosition + TileOffset;

			var rects = new Rectangle[4];
			if (FindContainer(i + 1, j) != null) {
				rects[0] = new Rectangle((int)pos.X + 16, (int)pos.Y, 16, 16);
			}
			if (FindContainer(i, j + 1) != null) {
				rects[1] = new Rectangle((int)pos.X, (int)pos.Y + 16, 16, 16);
			}
			if (FindContainer(i - 1, j) != null) {
				rects[2] = new Rectangle((int)pos.X - 16, (int)pos.Y, 16, 16);
			}
			if (FindContainer(i, j - 1) != null) {
				rects[3] = new Rectangle((int)pos.X, (int)pos.Y - 16, 16, 16);
			}
			return rects;
		}

		public Rectangle[] GetExtensionSource(int i, int j) {
			var rects = new Rectangle[4];
			if (FindContainer(i + 1, j) != null) {
				rects[0] = new Rectangle(0, 0, 16, 16);
			}
			if (FindContainer(i, j + 1) != null) {
				rects[1] = new Rectangle(16, 16, 16, 16);
			}
			if (FindContainer(i - 1, j) != null) {
				rects[2] = new Rectangle(0, 16, 16, 16);
			}
			if (FindContainer(i, j - 1) != null) {
				rects[3] = new Rectangle(16, 0, 16, 16);
			}
			return rects;
		}

		public override void PostDraw(int i, int j, SpriteBatch spriteBatch) {
			bool idc = false;
			TileFrame(i, j, ref idc, ref idc);
			Rectangle[] sourceRects = GetExtensionSource(i, j);
			Rectangle[] destinationRects = GetExtensionDestination(i, j);
			for (int x = 0; x < 4; x++) {
				if (sourceRects[x] != new Rectangle())
					spriteBatch.Draw(ModContent.Request<Texture2D>("Techarria/Content/Tiles/Transfer/TransferDuctExtensions").Value, destinationRects[x], sourceRects[x], Lighting.GetColor(i, j));
			}
		}
	}
}
