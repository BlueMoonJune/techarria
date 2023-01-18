using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Techarria.Transfer;
using Terraria;

namespace Techarria
{
    public abstract class ContainerInterface
    {
        public int x = 0;
        public int y = 0;
        public int dir = 0;

        public static ContainerInterface Find(int x, int y)
        {

            Point point = StorageCrateInterface.FindTopLeft(x, y);
            if (point != Point.Zero)
                return new StorageCrateInterface(point.X, point.Y);

            point = ChestInterface.FindTopLeft(x, y);
            if (point != Point.Zero)
                return new ChestInterface(point.X, point.Y);

            point = ExternalInterfaceInterface.FindTopLeft(x, y);
            if (point != Point.Zero)
                return new ExternalInterfaceInterface(point.X, point.Y);

            point = GelatinousTurbineInterface.FindTopLeft(x, y);
            if (point != Point.Zero)
                return new GelatinousTurbineInterface(point.X, point.Y);

            if (ItemPlacerInterface.Check(x, y))
                return new ItemPlacerInterface(x, y);


            return null;
        }

        public static Item decrementItem(Item item)
        {
            item.stack--;
            if (item.stack <= 0)
                item.TurnToAir();
            return item;
        }

        public virtual bool ExtractItem(Item item)
        {
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
