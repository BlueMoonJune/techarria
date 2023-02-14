using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Techarria.Common.UI.MultiAdhesiveUI;
using Terraria.GameContent.Creative;

namespace Techarria.Content.Items.Tools.Adhesive
{
	internal class MultiAdhesive : ModItem
	{

		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Hot Glue Gun");
			Tooltip.SetDefault("Right click while holding to edit adhesive settings");

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults() {
			Item.width = 28;
			Item.height = 24;
			Item.maxStack = 1;
			Item.value = Item.buyPrice(0, 0, 1, 0);

			Item.useStyle = ItemUseStyleID.Shoot;
			Item.useTurn = true;
			Item.useAnimation = 5;
			Item.useTime = 5;
			Item.autoReuse = true;
		}

		public override bool? CanChooseAmmo(Item ammo, Player player) {
			if (ammo.type == ItemID.Gel || ammo.type == ItemID.PinkGel || ammo.type == ItemID.HoneyBlock) {
				ammo.stack--;
				if (ammo.stack <= 0) {
					ammo.TurnToAir();
				}
				return true;
			}
			return false;
		}

		public override bool AltFunctionUse(Player player) {
			MultiAdhesivePlayer p = player.GetModPlayer<MultiAdhesivePlayer>();
			p.UIOpen = !p.UIOpen;
			if (p.UIOpen == false) {
				p.CloseUI();
			}
			return true;
		}

		public override void UpdateInventory(Player player) {
			MultiAdhesivePlayer p = player.GetModPlayer<MultiAdhesivePlayer>();
			if (player.HeldItem.type != ModContent.ItemType<MultiAdhesive>() && p.UIOpen) {
				p.CloseUI();
			}

		}

		public override bool CanUseItem(Player player) {

			if (MultiAdhesiveButton.mouseOverAny) {
				return false;
			}

			MultiAdhesivePlayer p = player.GetModPlayer<MultiAdhesivePlayer>();
			Vector2 vec = Main.MouseWorld / 16;
			Point pos = new((int)vec.X, (int)vec.Y);
			Tile tile = Main.tile[pos];

			byte tTypes = (byte)tile.Get<Glue>().types;
			byte pTypes = (byte)p.GlueTypes;
			byte newTypes;
			if (p.removing) {
				newTypes = (byte)(tTypes & ~pTypes);
			} else {
				newTypes = (byte)(tTypes | pTypes);
			}

			if (newTypes == tTypes) {
				return false;
			}
			return true;
		}

		public override void UseAnimation(Player player) {

			if (player.altFunctionUse == 2) {
				return;
			}

			MultiAdhesivePlayer p = player.GetModPlayer<MultiAdhesivePlayer>();
			if (player.altFunctionUse != 2) {
				p.CloseUI();
			}

			Vector2 vec = Main.MouseWorld / 16;
			Point pos = new((int)vec.X, (int)vec.Y);
			Tile tile = Main.tile[pos];

			byte tTypes = (byte)tile.Get<Glue>().types;
			byte pTypes = (byte)p.GlueTypes;
			byte newTypes;
			if (p.removing) {
				newTypes = (byte)(tTypes & ~pTypes);
			}
			else {
				newTypes = (byte)(tTypes | pTypes);
			}

			tile.Get<Glue>().types = (GlueTypes)newTypes;
			SoundEngine.PlaySound(SoundID.NPCHit25, Main.MouseWorld);
		}
	}

	public class MultiAdhesivePlayer : ModPlayer 
	{
		public GlueTypes GlueTypes = 0;
		public bool removing = false;
		public bool UIOpen = false;
		public Vector2 UIPos = new(float.MinValue);

		public void CloseUI() {
			UIOpen = false;
			UIPos = new Vector2(float.MinValue);
			MultiAdhesiveButton.mouseOverAny = false;
		}
	}
}
