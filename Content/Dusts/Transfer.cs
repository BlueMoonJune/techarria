using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Techarria.Content.Dusts
{
    internal class Transfer : ModDust
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
			dust.position += dust.velocity;
			dust.rotation += (float) (dust.velocity.Length() / Math.PI);
			dust.scale = dust.velocity.Length();
			dust.velocity *= 0.94f;

			float light = 1f * dust.scale;

			Lighting.AddLight(dust.position, light, light, light);

			if (dust.velocity.Length() < 0.1f)
			{
				dust.active = false;
			}

			return false; // Return false to prevent vanilla behavior.
		}
	}
}
