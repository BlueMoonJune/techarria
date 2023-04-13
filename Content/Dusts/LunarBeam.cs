using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace Techarria.Content.Dusts
{
	internal class LunarBeam : ModDust
	{
		public override void OnSpawn(Dust dust) {
			dust.velocity *= 0.0f;
			dust.noGravity = true;
			dust.noLight = true;
			dust.scale = 1f;
		}

		public override bool Update(Dust dust) {
			dust.position += dust.velocity;
			dust.scale -= 1/60f;

			if (dust.scale <= 0) {
				dust.active = false;
			}

			return false;
		}
	}
}
