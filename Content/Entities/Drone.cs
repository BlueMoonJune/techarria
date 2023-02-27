using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.Audio;
using Techarria.Content.Projectiles;

namespace Techarria.Content.Entities
{
	internal class Drone
	{
		public static List<Drone> drones = new();

		public Vector2 position;
		public Vector2 velocity;
		public Vector2 oldVelocity;
		public float rotation;
		public Item item;
		public Vector2 size = new(24, 24);
		public Vector2 Center {
			get { return position + size / 2; }
			set { position = value - size / 2; }
		}
		public float Width { 
			get { return size.X; }
			set { size.X = value; }
		}
		public float Height { 
			get { return size.Y; }
			set { size.Y = value; }
		}

		private Texture2D Texture { get { return ModContent.Request<Texture2D>("Techarria/Content/Entities/Drone").Value; } }

		public static Drone NewDrone(Vector2 pos) {
			Drone drone = new Drone();
			drone.Center = pos;
			Drone.drones.Add(drone);
			Main.NewText($"Created Drone #{drones.Count} at {pos.X}, {pos.Y}");
			return drone;
		}

		public void Kill() {
			for (int i = 0; i < 25; i++) {
				Dust dust = Dust.NewDustDirect(Center - Vector2.One * 16, 32, 32, DustID.Electric);
				dust.scale *= 2;
				dust.velocity *= 3;
			}
			Projectile.NewProjectile(new EntitySource_Misc("Drone Destruction"), Center, Vector2.Zero, ModContent.ProjectileType<EMPBlast>(), 0, 0);
			SoundEngine.PlaySound(SoundID.Item14, Center);
			Drone.drones.Remove(this);
		}

		public void Update() {
			position += velocity;

			Vector2 moveTarget = Main.MouseWorld;

			velocity += ((moveTarget - Center) - velocity / 0.05f) * 0.005f;
			if (velocity.Length() > 10) {
				velocity.Normalize();
				velocity *= 10;
			}

			Vector2 accel = velocity - oldVelocity;
			accel += new Vector2(-Main.windSpeedCurrent * Main.windPhysicsStrength, -Player.defaultGravity) + velocity * 0.01f;
			float angle = MathF.Atan2(accel.X, -accel.Y);
			rotation = (rotation - angle) * 0.9f + angle;

			if (Collision.SolidCollision(position, (int)Width, (int)Height)) {
				Kill();
			}
			
			oldVelocity = velocity;

			Lighting.AddLight((int)Center.X / 16, (int)Center.Y / 16, 0, 1f, 1);
		}

		public void Draw() {
			Point tile = new((int)Center.X / 16, (int)Center.Y / 16);

			Matrix offset = Matrix.Identity;
			offset.Translation = new Vector3(Center.X - Main.screenPosition.X, Center.Y - Main.screenPosition.Y, 0);
			Matrix transform = Main.Transform;
			transform = offset * transform;
			transform = Matrix.CreateRotationZ(rotation) * transform;

			Main.spriteBatch.Begin(
				SpriteSortMode.Deferred,
				BlendState.AlphaBlend,
				Main.DefaultSamplerState,
				DepthStencilState.None,
				Main.Rasterizer,
				null,
				transform
			);

			Main.spriteBatch.Draw(Texture, -Texture.Size()/2, Texture.Bounds, Lighting.GetColor(tile));
			Main.spriteBatch.End();
		}

	}
}
