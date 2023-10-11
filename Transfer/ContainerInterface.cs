using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Techarria.Transfer
{
	public abstract class ContainerInterface
	{
		public static Point16 negOne = new Point16(-1, -1);

		public int x = 0;
		public int y = 0;
		public int dir = 0;

		public static ContainerInterface Find(int x, int y) {
            Point16 point = ChestInterface.FindTopLeft(x, y);
			if (point != negOne)
				return new ChestInterface(point.X, point.Y);

            point = InventoryEntityInterface.GetTopLeft(x, y);
			if (point != negOne)
				return new InventoryEntityInterface(point.X, point.Y);

			return null;
		}

		public static Item decrementItem(Item item) {
			item.stack--;
			if (item.stack <= 0)
				item.TurnToAir();
			return item;
		}

        /// <summary>
        /// Extracts an item from this container
		/// By default, decrements the stack size by one
        /// </summary>
        /// <returns>If the extraction was successful</returns>
        public virtual bool ExtractItem(Item item) {
			decrementItem(item);
			return true;
		}

		/// <summary>
		/// Returns an array of all items in this container
		/// </summary>
		/// <returns></returns>
		public abstract List<Item> GetItems();

		/// <summary>
		/// Inserts an item into this container
		/// </summary>
		/// <param name="item">The item to insert</param>
		/// <returns>Whether this was sucessful or not</returns>
		public abstract bool InsertItem(Item item);

		/// <summary>
		/// Checks if the container is empty
		/// </summary>
		/// <returns></returns>
		public virtual bool IsEmpty() { return GetItems().Count == 0; }
	}
}
