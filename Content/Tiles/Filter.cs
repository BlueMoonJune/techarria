using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Techarria.Content.Items.FilterItems;
using Terraria;
using Terraria.ModLoader;

namespace Techarria.Content.Tiles
{
    internal class Filter : TransferDuct
    {

        Item[,] filters = new Item[Main.maxTilesX, Main.maxTilesY];
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
            if (filters[x, y] != null && filters[x, y].ModItem is FilterItem filterItem) 
            {   
                
                if (!filterItem.AcceptsItem(item))
                {
                    return new FoundContainer().setNull(true);
                }
            }
            else if (filters[x, y] != null && item.type != filters[x, y].type)
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
            filters[i, j] = Main.player[Main.myPlayer].HeldItem.Clone();
            Main.NewText("Used Item " + Main.player[Main.myPlayer].HeldItem.Name + " on Filter");
            return true;
        }
    }
}
