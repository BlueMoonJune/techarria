using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Techarria.Content.Tiles
{
    internal class PowerDebug : PowerConsumer
    {
        public override void InsertPower(int i, int j, int amount)
        {
            Console.WriteLine(i + " " + j + " " + amount);
        }

        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileFrameImportant[Type] = true;

            AddMapEntry(Color.DarkSlateGray, CreateMapEntryName());

            DustType = DustID.Stone;
            ItemDrop = ModContent.ItemType<Items.Placeables.PowerDebug>();

            HitSound = SoundID.Tink;
        }
    }
}
