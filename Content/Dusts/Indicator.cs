using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Techarria.Content.Dusts
{
	/// <summary>
	/// Particle for when a transfer detector activates
	/// </summary>
    public class Indicator : ModDust
	{
		public override void OnSpawn(Dust dust)
		{
			dust.velocity *= 0.0f;
			dust.noGravity = true;
			dust.noLight = true;
			dust.scale = 1f;
		}

		public override bool Update(Dust dust)
		{
			dust.alpha += 16;

			float light = (255 - dust.alpha) / 256f;

			Lighting.AddLight(dust.position, 0f, light, 0f);

			if (dust.alpha >= 255)
			{
				dust.active = false;
			}

			return false;
		}
	}
}
