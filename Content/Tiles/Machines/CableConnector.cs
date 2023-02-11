using Microsoft.Xna.Framework;
using Techarria.Content.Dusts;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Techarria.Content.Tiles.Machines
{
	public class CableConnectorTE : ModTileEntity 
	{
		public bool isConnected = false;
		public int connectedID = 0;

		public int wireCount = 0;

		public override bool IsTileValidForEntity(int x, int y) {
			return (Main.tile[x, y].TileType == ModContent.TileType<CableConnector>());
		}
	} 

	public class CableConnector : ModTile
	{
		public override void SetStaticDefaults() {
			Main.tileFrameImportant[Type] = true;

			TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.LavaDeath = false;
			TileObjectData.addTile(Type);
		}

		public static CableConnectorTE GetTileEntity(int i, int j) {
			return TileEntity.ByPosition[new Point16(i, j)] as CableConnectorTE;
		}

		public override void PlaceInWorld(int i, int j, Item item) {
			ModContent.GetInstance<CableConnectorTE>().Place(i, j);
		}

		public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem) {
			if (fail || effectOnly)
				return;

			CableConnectorTE tileEntity = GetTileEntity(i, j);
			if (tileEntity.isConnected) {
				CableConnectorTE connectingTE = TileEntity.ByID[tileEntity.connectedID] as CableConnectorTE;
				connectingTE.isConnected = false;
			}
			Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 32, ModContent.ItemType<Items.Placeables.Machines.CableConnector>(), tileEntity.wireCount);
			Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 32, ItemID.Wire, tileEntity.wireCount);
			ModContent.GetInstance<CableConnectorTE>().Kill(i, j);
		}

		public override bool RightClick(int i, int j) {
			CableConnectorTE tileEntity = GetTileEntity(i, j);
			Item playerItem;
			CableConnectorPlayer connectorPlayer = Main.LocalPlayer.GetModPlayer<CableConnectorPlayer>();
			if (!Main.mouseItem.IsAir) {
				playerItem = Main.mouseItem;
			}
			else {
				playerItem = Main.player[Main.myPlayer].HeldItem;
			}

			if (playerItem.type == ItemID.Wire) {
				tileEntity.wireCount += 1;
				playerItem.stack--;
				if (playerItem.stack <= 0) {
					playerItem.TurnToAir();
				}
				return true;
			}

			if (connectorPlayer.isConnecting && !tileEntity.isConnected) {
				if (connectorPlayer.connectingID == tileEntity.ID) {
					connectorPlayer.isConnecting = false;
					return true;
				}
				CableConnectorTE connectingTE = TileEntity.ByID[connectorPlayer.connectingID] as CableConnectorTE;
				if (connectingTE != null && !connectingTE.isConnected) {
					tileEntity.connectedID = connectorPlayer.connectingID;
					tileEntity.isConnected = true;
					connectingTE.isConnected = true;
					connectingTE.connectedID = tileEntity.ID;
					connectorPlayer.isConnecting = false;
					return true;
				}
			}

			if (!tileEntity.isConnected) {
				connectorPlayer.isConnecting = true;
				connectorPlayer.connectingID = tileEntity.ID;
				return true;
			}

			if (!connectorPlayer.isConnecting) {
				CableConnectorTE connectingTE = TileEntity.ByID[tileEntity.connectedID] as CableConnectorTE;
				tileEntity.isConnected = false;
				connectingTE.isConnected = false;
				connectorPlayer.isConnecting = true;
				connectorPlayer.connectingID = connectingTE.ID;
				return true;
			}

			return false;
		}
	}

	public class CableConnectorPlayer : ModPlayer
	{
		public bool isConnecting = false;
		public int connectingID = 0;
	}
}
