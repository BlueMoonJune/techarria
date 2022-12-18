using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Techarria.Content.Items.FilterItems;
using Terraria;
using Terraria.ModLoader;
using Techarria;

namespace Techarria.Content.Tiles
{
    /// <summary>
    /// Restricts item transfer based on the item
    /// </summary>
    internal class Filter : TransferDuct
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ItemDrop = ModContent.ItemType<Items.Placeables.Junction>();
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            return true;
        }

        public override FoundContainer EvaluatePath(int x, int y, Item item, int origin, int depth)
        {
            int filterItemType = Techarria.filterItems[Techarria.filterIDs[x, y]];
            if (filterItemType != 0 && ModContent.GetModItem(filterItemType) is FilterItem filterItem) 
            {   
                
                if (!filterItem.AcceptsItem(item))
                {
                    return new FoundContainer().setNull(true);
                }
            }
            else if (filterItemType != 0 && item.type != filterItemType)
            {
                return new FoundContainer().setNull(true);
            }

            FoundContainer container = FindAdjacentContainer(x, y);
            
            if (!container.isNull && container.dir == origin)
            {
                CreateParticles(x, y, container.dir);
                return container;
            }

            int i = x + dirToX(origin);
            int j = y + dirToY(origin);
            if (Techarria.tileIsTransferDuct[Main.tile[i, j].TileType])
            {
                FoundContainer target = ((TransferDuct)TileLoader.GetTile(Main.tile[i, j].TileType)).EvaluatePath(x + dirToX(origin), y + dirToY(origin), item, origin, depth + 1);
                if (!target.isNull)
                {
                    CreateParticles(x, y, origin);
                    return target;
                }
            }


            return new FoundContainer().setNull(true);
        }

        public override void HitWire(int i, int j)
        {
        }

        public override bool RightClick(int i, int j)
        {
            Techarria.filterItems[Techarria.filterIDs[i, j]] = Main.player[Main.myPlayer].HeldItem.type;
            return true;
        }
        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            if (Techarria.filterItems[Techarria.filterIDs[i, j]] != 0)
            {
                player.cursorItemIconEnabled = true;
                player.cursorItemIconID = Techarria.filterItems[Techarria.filterIDs[i, j]];
            }
        }
    }
}
