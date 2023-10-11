using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Techarria.Content.Dusts;
using Techarria.Content.Items.FluidItems;
using Techarria.Content.Tiles;
using Techarria.Structures;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Techarria.Content.Tiles.FluidTransfer
{
	public class FluidPumpTE : ModTileEntity
	{
		public static int maxCapacity = 1020;

		public Item fluid = new Item();

		public int progress = 0;

		public int animFrame = 0;

		public override bool IsTileValidForEntity(int x, int y) {
			return Main.tile[x, y].TileType == ModContent.TileType<FluidPump>();
		}

		public override void Update() {
		}

		public void InsertCharge(int amount) {
			progress += amount;
			while (progress > 15) {
				progress -= 15;
				animFrame++;
				animFrame %= 6;
				for (int i = 0; i < 2; i++) {
					Tile tile = Main.tile[Position.ToPoint() + new Point(1, i)];
					if (tile.LiquidAmount > 0) {
						if (fluid.IsAir) {
							int intake = Math.Min((int)tile.LiquidAmount, 16);
							fluid = new Item(FluidItem.liquidToFluidItem[tile.LiquidType], intake);
							tile.LiquidAmount -= (byte)intake;
						}
						if (fluid.ModItem is FluidItem f && f.liquidType == tile.LiquidType) {
							int intake = Math.Min(Math.Min((int)tile.LiquidAmount, 16), maxCapacity - fluid.stack);
							fluid.stack += intake;
							tile.LiquidAmount -= (byte)intake;
						}
					}
				}
			}
		}
	}

	public class FluidPump : PowerConsumer<FluidPumpTE>
	{
		public override void SetStaticDefaults() {

			// Spelunker
			Main.tileSpelunker[Type] = true;

			// Properties
			Main.tileLavaDeath[Type] = false;
			Main.tileFrameImportant[Type] = true;
			TileID.Sets.DisableSmartCursor[Type] = true;
			TileID.Sets.IgnoredByNpcStepUp[Type] = true;
			TileID.Sets.PreventsTileRemovalIfOnTopOfIt[Type] = true;

			DustType = ModContent.DustType<Spikesteel>();

			// placement
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.LavaDeath = false;
			TileObjectData.addTile(Type);
		}

		public virtual FluidPumpTE GetTileEntity(int i, int j) {
			Tile tile = Framing.GetTileSafely(i, j);
			i -= tile.TileFrameX / 18 % 2;
			j -= tile.TileFrameY / 18 % 2;
			TileEntity.ByPosition.TryGetValue(new Point16(i, j), out TileEntity te);
			return te as FluidPumpTE;
		}
		public override void PlaceInWorld(int i, int j, Item item) {
			Tile tile = Framing.GetTileSafely(i, j);
			i -= tile.TileFrameX / 18 % 2;
			j -= tile.TileFrameY / 18 % 2;
			ModContent.GetInstance<FluidPumpTE>().Place(i, j);
		}

		public override void KillMultiTile(int i, int j, int frameX, int frameY) {
			FluidPumpTE te = GetTileEntity(i, j);
			ModContent.GetInstance<FluidPumpTE>().Kill(i, j);
		}

		public override void InsertPower(int i, int j, int amount) {
			FluidPumpTE te = GetTileEntity(i, j);
			te.InsertCharge(amount);

		}

		public override void MouseOver(int i, int j) {
			FluidPumpTE te = GetTileEntity(i, j);
			Player player = Main.LocalPlayer;
			if (!te.fluid.IsAir) {
				player.cursorItemIconEnabled = true;
				player.cursorItemIconID = te.fluid.type;
				player.cursorItemIconText = $"{te.fluid.stack} L / {FluidPumpTE.maxCapacity} L";
			}
		}

		public override bool RightClick(int i, int j) {

			FluidPumpTE tileEntity = GetTileEntity(i, j);
			if (!tileEntity.fluid.IsAir) {
				tileEntity.fluid.TurnToAir();
				return true;
			}
			return false;
		}

		public override void PostDraw(int i, int j, SpriteBatch spriteBatch) {
			FluidPumpTE tileEntity = GetTileEntity(i, j);
			Point16 subTile = new Point16(i, j) - tileEntity.Position;

			Vector2 TileOffset = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
			Vector2 pos = new Vector2(i, j) * 16 - Main.screenPosition + TileOffset;

			spriteBatch.Draw(ModContent.Request<Texture2D>("Techarria/Content/Tiles/FluidTransfer/FluidPump_Overlay").Value, pos, new Rectangle(subTile.X * 18 + tileEntity.animFrame * 36, subTile.Y * 18, 16, 16), Lighting.GetColor(i, j));
		}
	}
}
