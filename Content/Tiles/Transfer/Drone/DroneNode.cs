using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Techarria.Content.Dusts;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Techarria.Content.Tiles.Transfer.Drone
{
	public class DroneNodeTE : ModTileEntity
	{

		public static float DRONE_NODE_CONNECTION_RANGE = 256;

		public List<int> connected = new(); 

		public override bool IsTileValidForEntity(int x, int y) {
			return Main.tile[x, y].TileType == ModContent.TileType<DroneNode>();
		}

		public override void Update() {
			var pos = new Vector2(Position.X * 16 + 4, Position.Y * 16 + 4);
			float dist = pos.Distance(Main.MouseWorld);
			float scale = dist > DRONE_NODE_CONNECTION_RANGE ? dist > 2 * DRONE_NODE_CONNECTION_RANGE ? 2 * DRONE_NODE_CONNECTION_RANGE - dist + DRONE_NODE_CONNECTION_RANGE : DRONE_NODE_CONNECTION_RANGE : dist;
			for (int t = 0; t < scale; t++) {
				float theta = MathHelper.TwoPi * t / scale;
				if (scale > 0) {
					//var dust = Dust.NewDustDirect(pos + new Vector2(scale, 0).RotatedBy(theta), 0, 0, ModContent.DustType<DroneNodeDust>());
					//dust.velocity = new Vector2(-scale/50f, 0).RotatedBy(theta);
				}
			}
		}

		public void Connect() {

		}
	}

	public class DroneNode : ModTile
	{

		public static int CanPlace(int i, int j) {
			return 1;
		}

		public override void SetStaticDefaults() {

			Main.tileFrameImportant[Type] = true;

			//ItemDrop = ModContent.ItemType<Items.Placeables.Transfer.Drones.DroneNode>();
		}

		public override void PlaceInWorld(int i, int j, Item item) {
			ModContent.GetInstance<DroneNodeTE>().Place(i, j);
		}

		public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem) {
			ModContent.GetInstance<DroneNodeTE>().Kill(i, j);
		}

		public static DroneNodeTE GetTileEntity(int i, int j) {
			var tileEntity = TileEntity.ByPosition[new Point16(i, j)] as DroneNodeTE;
			return tileEntity;
		}
	}
}
