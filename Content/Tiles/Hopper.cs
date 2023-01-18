using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.ObjectData;
using Techarria.Content.Dusts;
using Terraria.Audio;

namespace Techarria.Content.Tiles
{
    public class Hopper : TransferDuct
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.tileSolidTop[Type] = true;
            ItemDrop = ModContent.ItemType<Items.Placeables.Hopper>();
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            return false;
        }

        public override void HitWire(int x, int y)
        {
            Rectangle collectArea = new Rectangle(x * 16 + 4, (y - 1) * 16, 16, 16);
            Dust suction = Dust.NewDustDirect(new Vector2(x, y - 1) * 16 + new Vector2(4), 0, 0, ModContent.DustType<Suction>());
            suction.velocity = new Vector2(0, 1);
            for (int i = 0; i < Main.item.Length; i++)
            {
                Item item = Main.item[i];
                if (item != null && item.getRect().Intersects(collectArea))
                {
                    ContainerInterface target = EvaluatePath(x, y, item, 1, 0);
                    if (target != null && target.InsertItem(item))
                    {
                        SoundEngine.PlaySound(new SoundStyle("Techarria/Content/Sounds/Transfer"), new Vector2(x, y) * 16);
                        item.stack--;
                        if (item.stack <= 0)
                        {
                            Main.item[i] = new Item();
                        }
                        return;
                    }
                }
            }
        }
    }
}
