using Terraria.ModLoader;
using MagicStorage.Items;

namespace Techarria.Content.Items.Placeables
{
    [JITWhenModsEnabled("MagicStorage")]
    [ExtendsFromMod("MagicStorage")]
    internal class ExternalInterface : StorageComponent
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createTile = ModContent.TileType<Tiles.ExternalInterface>();
        }
    }
}
