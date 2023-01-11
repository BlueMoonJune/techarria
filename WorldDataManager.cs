using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

/*
namespace Techarria
{
    /// <summary>
    /// Saves and loads world data for this mod
    /// </summary>
    internal class WorldDataManager : ModSystem
    {
        public override void LoadWorldData(TagCompound tag)
        {
            for (int i = 0; i < Main.maxTilesX; i++)
            {
                for (int j = 0; j < Main.maxTilesY; j++)
                {
                    Techarria.wormholeIDs[i, j] = -1;
                }
            }

            if (tag["filterPosX"] is int[] filterPosX && tag["filterPosY"] is int[] filterPosY && tag["filterItems"] is int[] filterItems)
            {
                for (int i = 0; i < filterPosX.Length; i++)
                {
                    Techarria.filterItems[i] = filterItems[i];
                    Techarria.filterPositions[i] = new Point(filterPosX[i], filterPosY[i]);
                    Techarria.filterIDs[filterPosX[i], filterPosY[i]] = i;
                    
                }
            }

            if (tag["wormholePosX"] is int[] wormholePosX && tag["wormholePosY"] is int[] wormholePosY && tag["wormholeItems"] is int[] wormholeItems && wormholePosX.Length > 0 && wormholePosY.Length > 0 && wormholeItems.Length > 0)
            {
                for (int i = 0; i < wormholePosX.Length; i++)
                {
                    Techarria.wormholeLinkItems[i] = wormholeItems[i];
                    Techarria.wormholePositions[i] = new Point(wormholePosX[i], wormholePosY[i]);
                    Techarria.wormholeIDs[wormholePosX[i], wormholePosY[i]] = i;

                }
            }
        }

        public override void SaveWorldData(TagCompound tag)
        {
            int[] tempFilterItems = new int[Techarria.filterItems.Length];
            int[] tempFilterPosX = new int[Techarria.filterPositions.Length];
            int[] tempFilterPosY = new int[Techarria.filterPositions.Length];
            int count = 0;

            for (int i = 0; i < Techarria.filterItems.Length; i++)
            {
                if (Techarria.filterPositions[i] != Point.Zero)
                {
                    tempFilterItems[count] = Techarria.filterItems[i];
                    tempFilterPosX[count] = Techarria.filterPositions[i].X;
                    tempFilterPosY[count] = Techarria.filterPositions[i].Y;
                    count++;
                }
            }

            int[] shortenedFilterItems = new int[count];
            int[] shortenedFilterPosX = new int[count];
            int[] shortenedFilterPosY = new int[count];

            for (int i = 0; i < count; i++)
            {
                shortenedFilterItems[i] = tempFilterItems[i];
                shortenedFilterPosX[i] = tempFilterPosX[i];
                shortenedFilterPosY[i] = tempFilterPosY[i];
            }

            tag["filterItems"] = shortenedFilterItems;
            tag["filterPosX"] = shortenedFilterPosX;
            tag["filterPosY"] = shortenedFilterPosY;


            int[] tempWormholeItems = new int[Techarria.wormholeLinkItems.Length];
            int[] tempWormholePosX = new int[Techarria.wormholePositions.Length];
            int[] tempWormholePosY = new int[Techarria.wormholePositions.Length];
            count = 0;

            for (int i = 0; i < Techarria.wormholeLinkItems.Length; i++)
            {
                if (Techarria.wormholePositions[i] != Point.Zero)
                {
                    tempWormholeItems[count] = Techarria.wormholeLinkItems[i];
                    tempWormholePosX[count] = Techarria.wormholePositions[i].X;
                    tempWormholePosY[count] = Techarria.wormholePositions[i].Y;
                    count++;
                }
            }

            int[] shortenedWormholeItems = new int[count];
            int[] shortenedWormholePosX = new int[count];
            int[] shortenedWormholePosY = new int[count];

            for (int i = 0; i < count; i++)
            {
                shortenedWormholeItems[i] = tempWormholeItems[i];
                shortenedWormholePosX[i] = tempWormholePosX[i];
                shortenedWormholePosY[i] = tempWormholePosY[i];
            }

            tag["wormholeItems"] = shortenedWormholeItems;
            tag["wormholePosX"] = shortenedWormholePosX;
            tag["wormholePosY"] = shortenedWormholePosY;

            Item[] tempItemPlacerItems = new Item[Techarria.itemPlacerItems.Length];
            int[] tempItemPlacerPosX = new int[Techarria.itemPlacerPositions.Length];
            int[] tempItemPlacerPosY = new int[Techarria.itemPlacerPositions.Length];
            count = 0;

            for (int i = 0; i < Techarria.itemPlacerItems.Length; i++)
            {
                if (Techarria.itemPlacerPositions[i] != Point.Zero)
                {
                    tempItemPlacerItems[count] = Techarria.itemPlacerItems[i];
                    tempItemPlacerPosX[count] = Techarria.itemPlacerPositions[i].X;
                    tempItemPlacerPosY[count] = Techarria.itemPlacerPositions[i].Y;
                    count++;
                }
            }

            int[] shortenedItemPlacerItemTypes = new int[count];
            int[] shortenedItemPlacerItemStacks = new int[count];
            int[] shortenedItemPlacerPosX = new int[count];
            int[] shortenedItemPlacerPosY = new int[count];

            for (int i = 0; i < count; i++)
            {
                shortenedItemPlacerItemTypes[i] = tempItemPlacerItems[i].type;
                shortenedItemPlacerItemStacks[i] = tempItemPlacerItems[i].stack;
                shortenedItemPlacerPosX[i] = tempItemPlacerPosX[i];
                shortenedItemPlacerPosY[i] = tempItemPlacerPosY[i];
            }

            tag["itemPlacerItemTypes"] = shortenedItemPlacerItemTypes;
            tag["itemPlacerItemStacks"] = shortenedItemPlacerItemStacks;
            tag["itemPlacerPosX"] = shortenedItemPlacerPosX;
            tag["itemPlacerPosY"] = shortenedItemPlacerPosY;
        }
    }
}
*/