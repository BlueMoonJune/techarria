using Techarria.Structures;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Techarria.Content.Tiles.Machines
{
	public class GreenhouseControllerTE : ModTileEntity {

		int updateTimer = 0;
		public Greenhouse greenhouse;

		public override bool IsTileValidForEntity(int x, int y) {
			return Main.tile[x, y].TileType == ModContent.TileType<GreenhouseController>();
		}

		public override void Update() {
			updateTimer--;
			if (updateTimer <= 0) {
				if (greenhouse != null) {
					if (!greenhouse.CheckStructure()) {
						greenhouse = null;
					}
				}
				if (greenhouse == null) {
					greenhouse = Greenhouse.CreateGreenhouse(Position.X, Position.Y);
				}
				updateTimer = 60;
			}
		}
	}

	public class GreenhouseController : ModTile {
		public override void SetStaticDefaults() {

			// Spelunker
			Main.tileSpelunker[Type] = true;

			// Properties
			Main.tileLavaDeath[Type] = false;
			Main.tileFrameImportant[Type] = true;
			TileID.Sets.DisableSmartCursor[Type] = true;
			TileID.Sets.IgnoredByNpcStepUp[Type] = true;
			TileID.Sets.PreventsTileRemovalIfOnTopOfIt[Type] = true;

			// placement
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.LavaDeath = false;
			TileObjectData.addTile(Type);
		}

		public virtual GreenhouseControllerTE GetTileEntity(int i, int j) {
			Tile tile = Framing.GetTileSafely(i, j);
			i -= tile.TileFrameX / 18 % 2;
			j -= tile.TileFrameY / 18 % 2;
			TileEntity.ByPosition.TryGetValue(new Point16(i, j), out TileEntity te);
			return te as GreenhouseControllerTE;
		}
		public override void PlaceInWorld(int i, int j, Item item) {
			Tile tile = Framing.GetTileSafely(i, j);
			i -= tile.TileFrameX / 18 % 2;
			j -= tile.TileFrameY / 18 % 2;
			ModContent.GetInstance<GreenhouseControllerTE>().Place(i, j);
		}

		public override void KillMultiTile(int i, int j, int frameX, int frameY) {
			((GreenhouseControllerTE)GetTileEntity(i, j)).greenhouse.Remove();
			ModContent.GetInstance<GreenhouseControllerTE>().Kill(i, j);
		}
	}
}
