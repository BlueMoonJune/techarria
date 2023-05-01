using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Techarria.Transfer;
using Terraria;
using Terraria.ModLoader;

namespace Techarria.Content.Tiles.Transfer
{
	/// <summary>
	/// Sends items straight through itself
	/// </summary>
	public class Junction : TransferDuct
	{
		public override void SetStaticDefaults() {
			base.SetStaticDefaults();
			ItemDrop = ModContent.ItemType<Items.Placeables.Transfer.Junction>();
		}

		public override void PlaceInWorld(int i, int j, Item item) {
		}

		public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak) {
			return false;
		}

		public override ContainerInterface EvaluatePath(int x, int y, Item item, int origin, int depth) {
			if (depth >= 256) {
				Main.LocalPlayer.PickTile(x, y, 40000);
			}
			ContainerInterface container = FindAdjacentContainer(x, y);
			if (container != null && container.dir == origin) {
				CreateParticles(x, y, container.dir);
				return container;
			}

			int i = x + dirToX(origin);
			int j = y + dirToY(origin);
			if (Techarria.tileIsTransferDuct[Main.tile[i, j].TileType]) {
				ContainerInterface target = ((TransferDuct)TileLoader.GetTile(Main.tile[i, j].TileType)).EvaluatePath(x + dirToX(origin), y + dirToY(origin), item, origin, depth + 1);
				if (target != null) {
					CreateParticles(x, y, origin);
					return target;
				}
			}


			return null;
		}

		public override void HitWire(int x, int y) {
		}

		public override bool PreDraw(int i, int j, SpriteBatch spriteBatch) {

			Vector2 TileOffset = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
			Vector2 pos = new Vector2(i, j) * 16 - new Vector2(2) - Main.screenPosition + TileOffset;
			Rectangle sourceRect = new Rectangle(0, 0, 20, 20);
			if (Main.tile[i - 1, j].HasTile) {
				sourceRect.X = 2;
				sourceRect.Width = 18;
				pos.X += 2;
			}
			if (Main.tile[i, j - 1].HasTile) {
				sourceRect.Y = 2;
				sourceRect.Height = 18;
				pos.Y += 2;
			}
			if (Main.tile[i + 1, j].HasTile)
				sourceRect.Width -= 2;

			spriteBatch.Draw(ModContent.Request<Texture2D>("Techarria/Content/Tiles/Transfer/JunctionOutline").Value, pos, sourceRect, Lighting.GetColor(i, j));

			return true;
		}
	}
}
