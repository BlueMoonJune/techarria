using Microsoft.Xna.Framework;
using System.Collections.Generic;
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
			dust.customData = new List<Vector2>() { new(0, 0), new(0, 1) };
		}

		public override bool Update(Dust dust)
		{
			Vector2 l1 = ((List<Vector2>)dust.customData)[0];
			Vector2 l2 = ((List<Vector2>)dust.customData)[1];
			Vector2 a = dust.position.ClosestPointOnLine(l1, l2);
			dust.position += 0.1f*(a - dust.position) + dust.velocity;
			dust.scale -= 0.02f;

			if (dust.scale <= 0)
			{
				dust.active = false;
			}

			return false;
		}
	}
}
