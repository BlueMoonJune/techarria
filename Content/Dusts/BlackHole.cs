using Microsoft.Xna.Framework;
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
	/// Primary particle for transmitting wormholes
	/// </summary>
    internal class BlackHole : ModDust
	{
		public override void OnSpawn(Dust dust)
		{
			dust.velocity *= 0.0f;
			dust.noGravity = true;
			dust.noLight = true;
			dust.scale = 0f;
			dust.frame.Y = 0;
			dust.customData = 0f;
		}

		public override bool Update(Dust dust)
		{
			dust.scale = (float) Math.Sin((float) dust.customData * Math.PI) * 4;
			dust.customData = 1f / 60f + (float) dust.customData;
			dust.rotation += 0.05f * (5 - dust.scale);

			if ((float) dust.customData >= 0.95)
			{
				dust.active = false;
			}

			return false;
		}
	}
}
