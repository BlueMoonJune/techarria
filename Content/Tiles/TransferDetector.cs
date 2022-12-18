using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Techarria.Content.Dusts;
using Terraria;
using Terraria.ModLoader;

namespace Techarria.Content.Tiles
{
    /// <summary>
    /// Outputs a wire pulse when items are transfered through it
    /// </summary>
    internal class TransferDetector : TransferDuct
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ItemDrop = ModContent.ItemType<Items.Placeables.TransferDetector>();
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            return true;
        }

        public override FoundContainer EvaluatePath(int x, int y, Item item, int origin, int depth)
        {
            if (depth >= 256)
            {
                Main.LocalPlayer.PickTile(x, y, 40000);
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
                    Dust.NewDustDirect(new Vector2(x, y) * 16 + new Vector2(4), 0, 0, ModContent.DustType<Indicator>());
                    CreateParticles(x, y, origin);
                    Wiring.TripWire(x, y, 1, 1);
                }
                return target;
            }


            return new FoundContainer().setNull(true);
        }

        public override void HitWire(int i, int j)
        {
        }
    }
}
