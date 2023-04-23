using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Techarria.Content.Items.Tools.Hooks
{
	internal class BiofusingHook : ModItem
	{
		public override void SetDefaults() {
			// Copy values from the Amethyst Hook
			Item.CloneDefaults(ItemID.AmethystHook);
			Item.shootSpeed = 18f; // This defines how quickly the hook is shot.
			Item.shoot = ModContent.ProjectileType<BiofusingHookProjectile>(); // Makes the item shoot the hook's projectile when used.
		}
	}

	internal class BiofusingHookProjectile : ModProjectile
	{
		private static Asset<Texture2D> chainTexture;
		public float grappleDist = 0;
		public bool hooked = false;
		public Entity hookedNPC = null;
		public bool tileHooked = false;
		public int grabTileCounter = 0;

		public override void Load() { // This is called once on mod (re)load when this piece of content is being loaded.
									  // This is the path to the texture that we'll use for the hook's chain. Make sure to update it.
			chainTexture = ModContent.Request<Texture2D>("Techarria/Content/Items/Tools/Hooks/BiofusingHookChain");
		}

		public override void Unload() { // This is called once on mod reload when this piece of content is being unloaded.
										// It's currently pretty important to unload your static fields like this, to avoid having parts of your mod remain in memory when it's been unloaded.
			chainTexture = null;
		}

		/*
		public override void SetStaticDefaults() {
			// If you wish for your hook projectile to have ONE copy of it PER player, uncomment this section.
			ProjectileID.Sets.SingleGrappleHook[Type] = true;
		}
		*/

		public override void SetDefaults() {
			Projectile.CloneDefaults(ProjectileID.GemHookAmethyst); // Copies the attributes of the Amethyst hook's projectile.
		}

		// Use this hook for hooks that can have multiple hooks mid-flight: Dual Hook, Web Slinger, Fish Hook, Static Hook, Lunar Hook.
		public override bool? CanUseGrapple(Player player) {
			int hooksOut = 0;
			for (int l = 0; l < 1000; l++) {
				if (Main.projectile[l].active && Main.projectile[l].owner == Main.myPlayer && Main.projectile[l].type == Projectile.type) {
					hooksOut++;
				}
			}

			return hooksOut <= 2;
		}

		// Use this to kill oldest hook. For hooks that kill the oldest when shot, not when the newest latches on: Like SkeletronHand
		// You can also change the projectile like: Dual Hook, Lunar Hook
		// public override void UseGrapple(Player player, ref int type)
		// {
		//	int hooksOut = 0;
		//	int oldestHookIndex = -1;
		//	int oldestHookTimeLeft = 100000;
		//	for (int i = 0; i < 1000; i++)
		//	{
		//		if (Main.projectile[i].active && Main.projectile[i].owner == projectile.whoAmI && Main.projectile[i].type == projectile.type)
		//		{
		//			hooksOut++;
		//			if (Main.projectile[i].timeLeft < oldestHookTimeLeft)
		//			{
		//				oldestHookIndex = i;
		//				oldestHookTimeLeft = Main.projectile[i].timeLeft;
		//			}
		//		}
		//	}
		//	if (hooksOut > 1)
		//	{
		//		Main.projectile[oldestHookIndex].Kill();
		//	}
		// }

		public override void AI() {
			if (!hooked) {
				Player player = Main.player[Projectile.owner];
				if (tileHooked && player.controlDown) {
					grabTileCounter++;
					if (grabTileCounter >= 30) {
						player.PickTile((int)Projectile.Center.X / 16, (int)Projectile.Center.Y / 16, player.GetBestPickaxe().pick);
						grabTileCounter = 0;
					}
				}
				foreach (NPC npc in Main.npc) {
					if (npc.active && Projectile.getRect().Intersects(npc.getRect())) {
						Projectile.Center = npc.Center;
						hooked = true;
						hookedNPC = npc;
						break;
					}
				}
				foreach (Item npc in Main.item) {
					if (npc.active && Projectile.getRect().Intersects(npc.getRect())) {
						Projectile.Center = npc.Center;
						hooked = true;
						hookedNPC = npc;
						break;
					}
				}
			}
			else {
				Projectile.Center = hookedNPC.Center;
				if (!hookedNPC.active) {
					hooked = false;
				}
				Player player = Main.player[Projectile.owner];
				float entityMass = CalculateMass(hookedNPC);
				float playerMass = CalculateMass(player);

				hookedNPC.velocity = hookedNPC.DirectionTo(player.Center) * 15 * playerMass / (float)(entityMass + playerMass);
				if (hookedNPC.Distance(player.Center) < 64)
					hooked = false;
			}

		}

		public static float CalculateMass(Entity entity) {
			if (entity is Player player) {
				Rectangle rect = player.getRect();
				return rect.Width * rect.Height * (player.statDefense + 1) * (player.controlDown ? 10 : 1);
			}
			if (entity is NPC npc) {
				Rectangle rect = npc.getRect();
				return rect.Width * rect.Height * (npc.defense + 1);
			}
			return 0;
		}

		// Amethyst Hook is 300, Static Hook is 600.
		public override float GrappleRange() {
			return 400f;
		}

		public override void NumGrappleHooks(Player player, ref int numHooks) {
			numHooks = 1; // The amount of hooks that can be shot out
		}

		// default is 11, Lunar is 24
		public override void GrappleRetreatSpeed(Player player, ref float speed) {
			speed = 20f; // How fast the grapple returns to you after meeting its max shoot distance
		}

		public override void GrapplePullSpeed(Player player, ref float speed) {
			if (hookedNPC != null) {
				float entityMass = CalculateMass(hookedNPC);
				float playerMass = CalculateMass(player);
				speed = 15 * entityMass / (float)(entityMass+playerMass);
			}
			else {
				tileHooked = true;
				if (player.controlDown) {
					speed = 1;
				}
			}
		}

		// Adjusts the position that the player will be pulled towards. This will make them hang 50 pixels away from the tile being grappled.
		public override void GrappleTargetPoint(Player player, ref float grappleX, ref float grappleY) {
			if (player.controlDown) {
				float speed = 0;
				GrapplePullSpeed(player, ref speed);
				grappleY = player.Center.Y + 10 + player.DirectionTo(Projectile.Center).Y * speed;
			}
		}

		// Can customize what tiles this hook can latch onto, or force/prevent latching alltogether, like Squirrel Hook also latching to trees
		public override bool? GrappleCanLatchOnTo(Player player, int x, int y) {
			if (hooked)
				return true;

			// In any other case, behave like a normal hook
			return null;
		}

		// Draws the grappling hook's chain.
		public override bool PreDrawExtras() {
			Vector2 playerCenter = Main.player[Projectile.owner].MountedCenter;
			Vector2 center = Projectile.Center;
			Vector2 directionToPlayer = playerCenter - Projectile.Center;
			float chainRotation = directionToPlayer.ToRotation() - MathHelper.PiOver2;
			float distanceToPlayer = directionToPlayer.Length();
			float dist = directionToPlayer.Length();

			while (distanceToPlayer > 20f && !float.IsNaN(distanceToPlayer)) {
				directionToPlayer /= distanceToPlayer; // get unit vector
				directionToPlayer *= chainTexture.Height(); // multiply by chain link length

				center += directionToPlayer; // update draw position
				directionToPlayer = playerCenter - center; // update distance
				distanceToPlayer = directionToPlayer.Length();

				Color drawColor = Color.White;

				// Draw chain
				Main.EntitySpriteDraw(chainTexture.Value, center - Main.screenPosition,
					chainTexture.Value.Bounds, drawColor, chainRotation,
					chainTexture.Size() * 0.5f, new Vector2(grappleDist == 0 ? 1 : grappleDist / dist, 1), SpriteEffects.None, 0);
			}
			// Stop vanilla from drawing the default chain.
			return false;
		}
	}
}