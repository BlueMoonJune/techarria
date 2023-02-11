using Microsoft.Xna.Framework;
using Techarria.Common.UI.GlueLensUI;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Techarria.Common.Global
{
	internal class MechanicalLens : GlobalItem
	{
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) {
			return entity.type == ItemID.MechanicalLens;
		}

		public override void SetDefaults(Item item) {

			item.useStyle = ItemUseStyleID.Swing;
			item.useTurn = true;
			item.useAnimation = 30;
			item.useTime = 30;
			item.autoReuse = true;
		}

		public override bool CanUseItem(Item item, Player player) {
			return !GlueLensButton.mouseOverAny;
		}

		public override bool AltFunctionUse(Item item, Player player) {
			GlueLensPlayer p = player.GetModPlayer<GlueLensPlayer>();
			p.UIOpen = !p.UIOpen;
			if (p.UIOpen == false) {
				p.CloseUI();
			}
			return true;
		}

		public override void UpdateInventory(Item item, Player player) {
			GlueLensPlayer p = player.GetModPlayer<GlueLensPlayer>();
			if (player.HeldItem.type != ItemID.MechanicalLens && p.UIOpen) {
				p.CloseUI();
			}
		}
	}

	public class GlueLensPlayer : ModPlayer {

		public bool glueForced = false;
		public int[] modes = new int[4] {0, 0, 0, 0}; 
		public bool UIOpen = false;
		public Vector2 UIPos = new Vector2(float.MinValue);

		public void CloseUI() {
			UIOpen = false;
			UIPos = new Vector2(float.MinValue);
		}

	}
}
