using Techarria.Content.Items.FilterItems;
using Terraria;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.ModLoader.IO;
using Techarria.Transfer;

namespace Techarria.Content.Tiles.Transfer
{
	public class FilterTE : ModTileEntity
	{
		public int item;

		public override bool IsTileValidForEntity(int x, int y) {
			return Main.tile[x, y].TileType == ModContent.TileType<Filter>();
		}

		public override void SaveData(TagCompound tag) {
			tag.Add("item", item);
			base.SaveData(tag);
		}

		public override void LoadData(TagCompound tag) {
			item = tag.GetAsInt("item");
			base.LoadData(tag);
		}
	}

	/// <summary>
	/// Restricts item transfer based on the item
	/// </summary>
	public class Filter : TransferDuct
	{
		public override void SetStaticDefaults() {
			base.SetStaticDefaults();
			ItemDrop = ModContent.ItemType<Items.Placeables.Transfer.Filter>();
		}

		public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak) {
			return true;
		}

		public override ContainerInterface EvaluatePath(int x, int y, Item item, int origin, int depth) {
			bool mode = Main.tile[x, y].TileFrameX != 0;
			FilterTE tileEntity = GetTileEntity(x, y);
			int filterItemType = tileEntity.item;
			if (filterItemType != 0 && ModContent.GetModItem(filterItemType) is FilterItem filterItem) {

				if (!filterItem.AcceptsItem(item) ^ mode) {
					return null;
				}
			}
			else if ((filterItemType != 0 && item.type != filterItemType) ^ mode) {
				return null;
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



		  public override void PlaceInWorld(int i, int j, Item item) {
			ModContent.GetInstance<FilterTE>().Place(i, j);
		}

		public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem) {
			ModContent.GetInstance<FilterTE>().Kill(i, j);
		}

		public static FilterTE GetTileEntity(int i, int j) {
			var tileEntity = TileEntity.ByPosition[new Point16(i, j)] as FilterTE;
			return tileEntity;
		}

		public override void HitWire(int i, int j) {
			Tile tile = Main.tile[i, j];
			tile.TileFrameX += 16;
			tile.TileFrameX %= 32;
		}

		public override bool RightClick(int i, int j) {
			FilterTE tileEntity = GetTileEntity(i, j);
			tileEntity.item = Main.player[Main.myPlayer].HeldItem.type;
			return true;
		}
		public override void MouseOver(int i, int j) {
			FilterTE tileEntity = GetTileEntity(i, j);
			Player player = Main.LocalPlayer;
			player.noThrow = 2;
			if (tileEntity.item != 0) {
				player.cursorItemIconEnabled = true;
				player.cursorItemIconID = tileEntity.item;
			}
		}
	}
}
