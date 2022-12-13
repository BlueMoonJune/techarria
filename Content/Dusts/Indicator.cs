using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Techarria.Content.Dusts
{
    internal class Indicator : ModDust
	{
		public override void OnSpawn(Dust dust)
		{
			dust.velocity *= 0.0f; // Multiply the dust's start velocity by 0.4, slowing it down
			dust.noGravity = true; // Makes the dust have no gravity.
			dust.noLight = true; // Makes the dust emit no light.
			dust.scale = 1f; // Multiplies the dust's initial scale by 1.5.
		}

		public override bool Update(Dust dust)
		{ // Calls every frame the dust is active
			dust.alpha += 16;

			float light = (255 - dust.alpha) / 256f;

			Lighting.AddLight(dust.position, 0f, light, 0f);

			if (dust.alpha >= 255)
			{
				dust.active = false;
			}

			return false; // Return false to prevent vanilla behavior.
		}
	}
}
