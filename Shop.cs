/*
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Techarria
{
    public class Shop : GlobalNPC
    {
		public override void ModifyShop(NPCShop shop) {
		{
			// This example does not use the AppliesToEntity hook, as such, we can handle multiple npcs here by using if statements.
			if (shop.NpcType == NPCID.Mechanic)
			{
				// Adding an item to a vanilla NPC is easy:
				// This item sells for the normal price.
				shop.InsertAfter(ModContent.ItemType<Content.Items.Tools.MechanicHammer>());
				nextSlot++; // Don't forget this line, it is essential.

				shop.item[nextSlot].SetDefaults(ModContent.ItemType<Content.Items.Placeables.Machines.Logic.Button>());
				nextSlot++;
			}
		}
	}
}
*/
