using Terraria.ModLoader;
using MagicStorage.Components;
using Terraria;
using Terraria.DataStructures;

namespace Techarria.Content.Tiles
{
    [JITWhenModsEnabled("MagicStorage")]
    [ExtendsFromMod("MagicStorage")]
    internal class ExternalInterface : StorageAccess
    {
        public override int ItemType(int frameX, int frameY) => ModContent.ItemType<Items.Placeables.ExternalInterface>();
    }
}
