using Terraria;
using Terraria.ModLoader;
using System.Collections.Generic;
using Techarria.Content.Tiles.Machines.Logic;

namespace Techarria.Transfer
{
	public class AdvancedBlockBreakerInterface : ContainerInterface
    {
        public AdvancedBlockBreakerInterface(int i, int j)
        {
            x = i;
            y = j;
        }

        public static bool Check(int i, int j)
        {
            return Main.tile[i, j].TileType == ModContent.TileType<AdvancedBlockBreaker>();
        }

        public override List<Item> GetItems()
        {
			return AdvancedBlockBreaker.GetTileEntity(x, y).items;

		}

        public override bool InsertItem(Item item)
        {
            AdvancedBlockBreakerTE tileEntity = AdvancedBlockBreaker.GetTileEntity(x, y);
            foreach (Item myItem in tileEntity.items) {
				if (myItem.type == item.type && myItem.stack < myItem.maxStack) {
					myItem.stack++;
					return true;
				}
			}
			tileEntity.items.Add(item);
			return true;
        }

		public override bool ExtractItem(Item item) {
			bool val = base.ExtractItem(item);
			if (item.IsAir) {
				AdvancedBlockBreaker.GetTileEntity(x, y).items.Remove(item);
			}
			return val;
		}
	}
}
