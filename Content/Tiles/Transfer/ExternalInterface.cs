using Terraria.ModLoader;
using MagicStorage.Components;

namespace Techarria.Content.Tiles.Transfer
{
	[JITWhenModsEnabled("MagicStorage")]
	[ExtendsFromMod("MagicStorage")]
	public class ExternalInterface : StorageAccess
	{
		public override int ItemType(int frameX, int frameY) => ModContent.ItemType<Items.Placeables.Transfer.ExternalInterface>();
	}
}
