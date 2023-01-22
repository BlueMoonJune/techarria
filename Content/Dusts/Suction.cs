using Terraria;
using Terraria.ModLoader;

namespace Techarria.Content.Dusts
{
	/// <summary>
	/// Particles for extracting (or attempting to extract) an item from a container
	/// </summary>
	public class Suction : ModDust
	{
		public override void OnSpawn(Dust dust)
		{
			dust.velocity *= 0.0f;
			dust.noGravity = true;
			dust.noLight = true;
			dust.scale = 2f;
		}

		public override bool Update(Dust dust)
		{
			dust.position += dust.velocity;
			dust.velocity *= 0.94f;
			dust.rotation = dust.velocity.ToRotation();
			dust.alpha += 16;

			if (dust.velocity.Length() < 0.1f)
			{
				dust.active = false;
			}

			return false;
		}
	}
}
