using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;

namespace Techarria.Content.Tiles.Machines
{
	public class CableConnectorTE : ModTileEntity 
	{
		public bool isConnected = false;
		public int connectedID = 0;

		public int wireCount = 0;

		public bool recieved = false;

		public bool pulseQueued = false;

		public override bool IsTileValidForEntity(int x, int y) {
			return (Main.tile[x, y].TileType == ModContent.TileType<CableConnector>());
		}

		public override void Update() {
			if (!isConnected)
				return;
			if (TileEntity.ByID.TryGetValue(connectedID, out TileEntity temp) && temp is CableConnectorTE connectedTE) {
				Point16 conP = connectedTE.Position;
				Point16 dif = conP - Position;

				if (new Vector2(dif.X, dif.Y).Length() > wireCount + connectedTE.wireCount) {
					isConnected = false;
					connectedTE.isConnected = false;
				}
			}
			if (pulseQueued && isConnected && !recieved) {
				CableConnectorTE connectingTE = TileEntity.ByID[connectedID] as CableConnectorTE;
				connectingTE.recieved = true;
				Point16 tpos = connectingTE.Position;
				System.Console.WriteLine(Position);
				System.Console.WriteLine(tpos);
				Wiring.TripWire(tpos.X, tpos.Y, 1, 1);
			}

			pulseQueued = false;
			recieved = false;
		}

		public override void SaveData(TagCompound tag) {
			tag.Add("isConnected", isConnected);
			tag.Add("connectedID", connectedID);
			tag.Add("wireCount", wireCount);
		}

		public override void LoadData(TagCompound tag) {
			isConnected = tag.GetBool("isConnected");
			connectedID = tag.GetInt("connectedID");
			wireCount = tag.GetInt("wireCount");
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
				if (TileEntity.ByID.TryGetValue(tileEntity.connectedID, out TileEntity temp) && temp is CableConnectorTE connectingTE)
					connectingTE.isConnected = false;
			}
			Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 32, ModContent.ItemType<Items.Placeables.Machines.CableConnector>());
			if (tileEntity.wireCount > 0)
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
				if (TileEntity.ByID.TryGetValue(tileEntity.connectedID, out TileEntity temp) && temp is CableConnectorTE connectingTE) {
					tileEntity.isConnected = false;
					connectingTE.isConnected = false;
					connectorPlayer.isConnecting = true;
					connectorPlayer.connectingID = connectingTE.ID;
					return true;
				}
			}

			return false;
		}

		public override void MouseOver(int i, int j) {
			CableConnectorTE tileEntity = GetTileEntity(i, j);
			Player player = Main.LocalPlayer;
			player.noThrow = 2;
			if (tileEntity.wireCount > 0) {
				player.cursorItemIconEnabled = true;
				player.cursorItemIconText = "" + tileEntity.wireCount;
				player.cursorItemIconID = ItemID.Wire;
			}
		}

		public override void HitWire(int i, int j) {
			CableConnectorTE tileEntity = GetTileEntity(i, j);
			tileEntity.pulseQueued = true;
		}
	}

	public class CableConnectorPlayer : ModPlayer
	{
		public bool isConnecting = false;
		public int connectingID = 0;
	}
}
