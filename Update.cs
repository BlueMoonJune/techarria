using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Techarria.Content.Items.Materials.Molten;
using Techarria.Content.Entities;
using System.Collections.Generic;
using Techarria.Structures;
using Techarria.Content.Tiles;
using Techarria.Content.Dusts;

namespace Techarria
{
	internal class Update : ModSystem
    {
		int CheckStructuresTimer = 0;

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
			CheckStructuresTimer++;
			if (CheckStructuresTimer > 60) {
				for (int i = 0; i < Greenhouse.greenhouses.Count;) {
					if (Greenhouse.greenhouses[i].CheckStructure()) {
						i++;
					}
				}
				CheckStructuresTimer = 0;
			}
        }
    }
}
