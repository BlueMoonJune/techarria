using Terraria.ModLoader;
using MagicStorage.Items;

namespace Techarria.Content.Items.Placeables.Transfer
{
	[JITWhenModsEnabled("MagicStorage")]
    [ExtendsFromMod("MagicStorage")]
    public class ExternalInterface : StorageComponent
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createTile = ModContent.TileType<Tiles.Transfer.ExternalInterface>();
        }
    }
}
