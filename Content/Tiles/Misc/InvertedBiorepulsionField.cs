using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Techarria.Content.Tiles.Misc
{
    public class InvertedBiorepulsionField : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;

            HitSound = SoundID.Meowmere;
            //ItemDrop = ModContent.ItemType<Items.Placeables.InvertedBiorepulsionField>();
        }
    }
}