using Microsoft.Xna.Framework;
using Techarria.Content.Items.RecipeItems;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Techarria.Content.Tiles.Machines.Logic
{

	public class TemperatureProbeTE : ModTileEntity
	{
		public int targetTemp = 25;
		public int difSign = 1;
		public Direction direction = new(0);

		public override bool IsTileValidForEntity(int x, int y)
		{
			return Main.tile[x, y].TileType == ModContent.TileType<TemperatureProbe>();
		}

		public int? GetTemp() {
			Point target = new Point(Position.X, Position.Y) + direction;
			if (Main.tile[target].TileType == ModContent.TileType<BlastFurnace>()) {
				BlastFurnaceTE te = BlastFurnace.GetTileEntity(target.X, target.Y);
				return te != null ? (int)te.temp : 0;
			}
			if (Main.tile[target].TileType == ModContent.TileType<CastingTable>()) {
				CastingTableTE te = CastingTable.GetTileEntity(target.X, target.Y);
				return te != null ? (int)te.temp : 0;
			}
			return null;
		}

		public void SetTargetTemperature() 
		{
			int? temp = GetTemp();
			if (temp != null)
				targetTemp = (int)temp;
		}

		public override void Update() 
		{
			int? t = GetTemp();
			if (t == null) 
			{
				return;
			}
			int temp = (int)t;
			if (targetTemp < temp) {
				if (difSign == 1) {
					Wiring.TripWire(Position.X, Position.Y, 1, 1);
					difSign = -1;
				}
				return;
			}

			if (targetTemp > temp) {
				if (difSign == -1) {
					Wiring.TripWire(Position.X, Position.Y, 1, 1);
					difSign = 1;
				}
				return;
			}
		}

		public override void SaveData(TagCompound tag) {
			tag.Add("targetTemp", targetTemp);
			base.SaveData(tag);
		}

		public override void LoadData(TagCompound tag) {
			targetTemp = tag.GetInt("targetTemp");
			direction = Main.tile[Position.X, Position.Y].TileFrameX / 16;
			base.LoadData(tag);
		}
	}

	internal class TemperatureProbe : ModTile
	{
		public override void SetStaticDefaults() 
		{
			TileID.Sets.CanBeSloped[Type] = true;
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = false;
			Main.tileSolid[Type] = true;

			AddMapEntry(new Color(60 / 255f, 50 / 255f, 44 / 255f), CreateMapEntryName());

			DustType = DustID.Stone;
			//ItemDrop = ModContent.ItemType<Items.Placeables.Transfer.TransferDuct>();

			HitSound = SoundID.Tink;
		}

		public override bool Slope(int i, int j) {
			Tile tile = Framing.GetTileSafely(i, j);
			tile.TileFrameX = (short)((tile.TileFrameX + 16) % 64);
			((TemperatureProbeTE)TileEntity.ByPosition[new Point16(i, j)]).direction = tile.TileFrameX / 16;
			return false;
		}

		public override void PlaceInWorld(int i, int j, Item item) {
			ModContent.GetInstance<TemperatureProbeTE>().Place(i, j);
		}

		public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem) {
			if (fail || effectOnly)
				return;
			ModContent.GetInstance<TemperatureProbeTE>().Kill(i, j);
		}

		public override void HitWire(int i, int j) {
			((TemperatureProbeTE)TileEntity.ByPosition[new Point16(i, j)]).SetTargetTemperature();
		}

		public override bool RightClick(int i, int j) {
			((TemperatureProbeTE)TileEntity.ByPosition[new Point16(i, j)]).SetTargetTemperature();
			return true;
		}

		public override void MouseOver(int i, int j) {
			TemperatureProbeTE te = (TemperatureProbeTE)TileEntity.ByPosition[new Point16(i, j)];
			Player player = Main.LocalPlayer;
			player.cursorItemIconEnabled = true;
			player.cursorItemIconID = ModContent.ItemType<Temperature>();
			int? t = te.GetTemp();
			if (t == null) 
			{
				player.cursorItemIconText = "Invalid tile";
				return;
			}
			int temp = (int)t;
			string symbol;
			if (temp < te.targetTemp)
				symbol = "<";
			else if (temp > te.targetTemp)
				symbol = ">";
			else
				symbol = "=";
			player.cursorItemIconText = $"{temp} {symbol} {te.targetTemp}";
		}
	}
}
