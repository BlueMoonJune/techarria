using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Techarria;

namespace Techarria.Content.Items.Tools.Hooks
{
	public class DematerializationHook : ModItem
	{
		public override void SetDefaults() {
			// Copy values from the Amethyst Hook
			Item.CloneDefaults(ItemID.AmethystHook);
			Item.shootSpeed = 18f; // This defines how quickly the hook is shot.
			Item.shoot = ModContent.ProjectileType<DematerializationHookProjectile>(); // Makes the item shoot the hook's projectile when used.
		}
	}

	public class DematerializationHookProjectile : ModProjectile
	{
		private static Asset<Texture2D> chainTexture;

		public int frameCount = 0;
		private bool detach;

		public override void Load() { // This is called once on mod (re)load when this piece of content is being loaded.
									  // This is the path to the texture that we'll use for the hook's chain. Make sure to update it.
			chainTexture = ModContent.Request<Texture2D>("Techarria/Content/Items/Tools/Hooks/DematerializationHookChain");
		}

		public override void Unload() { // This is called once on mod reload when this piece of content is being unloaded.
										// It's currently pretty important to unload your static fields like this, to avoid having parts of your mod remain in memory when it's been unloaded.
			chainTexture = null;
		}

		public override void SetDefaults() {
			Projectile.CloneDefaults(ProjectileID.GemHookAmethyst); // Copies the attributes of the Amethyst hook's projectile.
		}

		// Use this hook for hooks that can have multiple hooks mid-flight: Dual Hook, Web Slinger, Fish Hook, Static Hook, Lunar Hook.
		public override bool? CanUseGrapple(Player player) {
			if(player.HasBuff<Dematerialized>()) {
				return false;
			}
			int hooksOut = 0;
			for (int l = 0; l < 1000; l++) {
				if (Main.projectile[l].active && Main.projectile[l].owner == Main.myPlayer && Main.projectile[l].type == Projectile.type) {
					hooksOut++;
				}
			}

			return hooksOut <= 2;
		}

		// Amethyst Hook is 300, Static Hook is 600.
		public override float GrappleRange() {
			return 200f;
		}

		public override void NumGrappleHooks(Player player, ref int numHooks) {
			numHooks = 1; // The amount of hooks that can be shot out
		}

		// default is 11, Lunar is 24
		public override void GrappleRetreatSpeed(Player player, ref float speed) {
			speed = 20f; // How fast the grapple returns to you after meeting its max shoot distance
		}

		public override void GrapplePullSpeed(Player player, ref float speed) {
			speed = 15f;
		}

		// Adjusts the position that the player will be pulled towards. This will make them hang 50 pixels away from the tile being grappled.
		public override void GrappleTargetPoint(Player player, ref float grappleX, ref float grappleY) {
			DematerializedPlayer p = player.GetModPlayer<DematerializedPlayer>();
			float speed = 0;
			GrapplePullSpeed(player, ref speed);
			if (p.velocity is Vector2 v && v.Dot(player.Center - Projectile.Center) > 0) {
				Projectile.Kill();
			}
			player.AddBuff(ModContent.BuffType<Dematerialized>(), 60);
		}

		// Can customize what tiles this hook can latch onto, or force/prevent latching alltogether, like Squirrel Hook also latching to trees
		public override bool? GrappleCanLatchOnTo(Player player, int x, int y) {

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
					chainTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0);
			}
			// Stop vanilla from drawing the default chain.
			return false;
		}
	}

	public class Dematerialized : ModBuff {

		public override void SetStaticDefaults() {
			Main.buffNoTimeDisplay[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex) {
			Dust.NewDust(player.position, player.width, player.height, DustID.ShimmerSpark);
			DematerializedPlayer p = player.GetModPlayer<DematerializedPlayer>();
			p.dematerialized = true;
			if (player.buffTime[buffIndex] <= 1) {
				p.dematerialized = false;
				if (p.startPosition is Vector2 pos)
					player.Teleport(pos, 1);
				if (p.velocity is Vector2 vel)
					player.velocity = vel * -1;
				SoundEngine.PlaySound(SoundID.Item8, player.Center);
				player.DelBuff(buffIndex);
				buffIndex--;
			}
			if (!Collision.SolidCollision(player.position, player.width, player.height)) {
				player.DelBuff(buffIndex);
				buffIndex--;
			}
			foreach (Point t in Collision.GetTilesIn(player.TopLeft, player.BottomRight)) {
				WorldGen.KillTile(t.X, t.Y, effectOnly: true);
			}
		}

		public override bool RightClick(int buffIndex) {
			return false;
		}
	}

	public class DematerializedPlayer : ModPlayer {

		public bool dematerialized = false;
		public Vector2? startPosition;
		public Vector2? velocity;

		public bool[] solidTiles;

		public override void SetStaticDefaults() {
			solidTiles = new bool[Main.tileSolid.Length];
			for (int i = 0; i < solidTiles.Length; i++) {
				solidTiles[i] = true;
			}
			solidTiles[TileID.LihzahrdBrick] = false;
		}

		public override void PostUpdateBuffs() {
			if (dematerialized)
				Player.shimmering = true;
		}

		public override void PostUpdate() {
			if (dematerialized) {
				if (velocity is Vector2 v) {
					Vector2 moveVec = new((Player.controlRight?1:0)-(Player.controlLeft?1:0), (Player.controlDown?1:0)-(Player.controlUp?1:0));
					v += moveVec;
					v.Normalize();
					v *= 15f;
					velocity = v;
					Player.velocity = v;
					Player.position += v;
				} else {
					velocity = Player.velocity;
				}
				if (startPosition == null)
					startPosition = Player.position;
			} else {
				velocity = null;
				startPosition = null;
			}
			dematerialized = false;
		}
	} 
}