using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Techarria.Content.Tiles;
using Techarria.Content.Tiles.Machines;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Techarria.Content.Items.Tools.Hooks
{
	internal class AetherealHook : ModItem
	{
		public override void SetDefaults() {
			// Copy values from the Amethyst Hook
			Item.CloneDefaults(ItemID.AmethystHook);
			Item.shootSpeed = 18f; // This defines how quickly the hook is shot.
			Item.shoot = ModContent.ProjectileType<AetherealHookProjectile>(); // Makes the item shoot the hook's projectile when used.
		}

		public override void AddRecipes() {
			AssemblyRecipe.recipes.Add(new AssemblyRecipe(new Item(Type), new Item(ModContent.ItemType<SuperconductingHook>()))
				.AddItem(new Item(ItemID.FragmentSolar), 0)
				.AddItem(new Item(ItemID.FragmentVortex), 2)
				.AddItem(new Item(ItemID.FragmentNebula), 4)
				.AddItem(new Item(ItemID.FragmentStardust), 6)
				.AddItem(new Item(ItemID.LunarOre), 1)
				.AddItem(new Item(ItemID.LunarOre), 3)
				.AddItem(new Item(ItemID.LunarOre), 5)
				.AddItem(new Item(ItemID.LunarOre), 7)
				.AddItem(new Item(ItemID.ShimmerBlock), 0)
				.AddItem(new Item(ItemID.ShimmerBlock), 1)
				.AddItem(new Item(ItemID.ShimmerBlock), 2)
				.AddItem(new Item(ItemID.ShimmerBlock), 3)
				.AddItem(new Item(ItemID.ShimmerBlock), 4)
				.AddItem(new Item(ItemID.ShimmerBlock), 5)
				.AddItem(new Item(ItemID.ShimmerBlock), 6)
				.AddItem(new Item(ItemID.ShimmerBlock), 7)
				);
			AssemblyRecipe.recipes.Add(new AssemblyRecipe(new Item(Type), new Item(ModContent.ItemType<SuperconductingHook>()))
				.AddItem(new Item(ItemID.FragmentSolar), 0)
				.AddItem(new Item(ItemID.FragmentVortex), 6)
				.AddItem(new Item(ItemID.FragmentNebula), 4)
				.AddItem(new Item(ItemID.FragmentStardust), 2)
				.AddItem(new Item(ItemID.LunarOre), 1)
				.AddItem(new Item(ItemID.LunarOre), 3)
				.AddItem(new Item(ItemID.LunarOre), 5)
				.AddItem(new Item(ItemID.LunarOre), 7)
				.AddItem(new Item(ItemID.ShimmerBlock), 0)
				.AddItem(new Item(ItemID.ShimmerBlock), 1)
				.AddItem(new Item(ItemID.ShimmerBlock), 2)
				.AddItem(new Item(ItemID.ShimmerBlock), 3)
				.AddItem(new Item(ItemID.ShimmerBlock), 4)
				.AddItem(new Item(ItemID.ShimmerBlock), 5)
				.AddItem(new Item(ItemID.ShimmerBlock), 6)
				.AddItem(new Item(ItemID.ShimmerBlock), 7)
				);
		}
	}

	internal class AetherealHookProjectile : ModProjectile
	{
		private static Asset<Texture2D> chainTexture;
		public float grappleDist = 0;
		public bool hooked = false;
		public Entity hookedNPC = null;

		public override void Load() { // This is called once on mod (re)load when this piece of content is being loaded.
									  // This is the path to the texture that we'll use for the hook's chain. Make sure to update it.
			chainTexture = ModContent.Request<Texture2D>("Techarria/Content/Items/Tools/Hooks/AetherealHookChain");
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

		public override void UseGrapple(Player player, ref int type) {
			Vector2 pos = Main.MouseWorld;
			WorldGen.PlaceTile((int)pos.X / 16, (int)pos.Y / 16, ModContent.TileType<AetherealAnchor>());
		}

		public override void AI() {
			Lighting.AddLight(Projectile.position, new Vector3(0.75f, 0.5f, 1));

		}

		// Amethyst Hook is 300, Static Hook is 600.
		public override float GrappleRange() {
			return 600f;
		}

		public override void NumGrappleHooks(Player player, ref int numHooks) {
			numHooks = 1; // The amount of hooks that can be shot out
		}

		// default is 11, Lunar is 24
		public override void GrappleRetreatSpeed(Player player, ref float speed) {
			speed = 20f; // How fast the grapple returns to you after meeting its max shoot distance
		}

		public override void GrapplePullSpeed(Player player, ref float speed) {
			speed = 30000; // How fast you get pulled to the grappling hook projectile's landing position
		}

		// Adjusts the position that the player will be pulled towards. This will make them hang 50 pixels away from the tile being grappled.
		public override void GrappleTargetPoint(Player player, ref float grappleX, ref float grappleY) {
			if (grappleDist == 0) {
				grappleDist = Projectile.Distance(player.Center);
			}
			Vector2 pos = player.Center + player.position - player.oldPosition + new Vector2(0, player.gravity);
			Vector2 dir = Projectile.DirectionTo(pos);
			float dist = Projectile.Distance(pos);
			if (dist - grappleDist > 10) {
				grappleDist = dist - 10;
			}
			grappleX += dir.X * grappleDist;
			grappleY += dir.Y * grappleDist;
		}

		// Can customize what tiles this hook can latch onto, or force/prevent latching alltogether, like Squirrel Hook also latching to trees
		public override bool? GrappleCanLatchOnTo(Player player, int x, int y) {
			if (Main.tile[x, y].TileType == ModContent.TileType<AetherealAnchor>())
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