using Terraria.ModLoader;
using MagicStorage.Items;

namespace Techarria.Content.Items.Placeables.Transfer
{
	[JITWhenModsEnabled("MagicStorage")]
    [ExtendsFromMod("MagicStorage")]
    public class ExternalInterface : StorageComponent
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("A component for transfer ducts to insert and extract from your Magic Storage system");
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createTile = ModContent.TileType<Tiles.Transfer.ExternalInterface>();
        }
    }
}
