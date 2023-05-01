using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Techarria.Content.Tiles
{
	public class BiorepulsionField : ModTile
	{
		public override void SetStaticDefaults() {
			Main.tileSolid[Type] = false;

			HitSound = SoundID.Meowmere;
			ItemDrop = ModContent.ItemType<Items.Placeables.BiorepulsionField>();
		}
	}
}