using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Techarria.Content.Items.Materials.Molten;
using Techarria.Content.Entities;
using System.Collections.Generic;

namespace Techarria
{
	internal class Update : ModSystem
    {
		public void DestroyChestsWithMoltenItems() {
			bool logged = false;
			foreach (Chest chest in Main.chest) {
				if (chest != null) {
					bool hasMolten = false;
					foreach (Item item in chest.item) {
						if (item.ModItem is MoltenBlob) {
							hasMolten = true;
							break;
						}
					}
					if (hasMolten) {
						for (int i = 0; i < chest.item.Length; i++) {
							Item item = chest.item[i];
							if (!logged) {
								logged = true;
							}
							Item.NewItem(new EntitySource_TileBreak(chest.x, chest.y), new Rectangle(chest.x * 16, chest.y * 16, 32, 32), item);
							chest.item[i] = null;
						}
						WorldGen.KillTile(chest.x, chest.y, noItem: true);
					}
				}
			}
		}

		public void UpdateDrones() {
			Drone[] tempDrones = Drone.drones.ToArray();
			foreach (Drone drone in tempDrones) {
				drone.Update();
			}
		}

        public override void PostUpdateEverything()
        {
			DestroyChestsWithMoltenItems();
			UpdateDrones();
        }
    }
}
