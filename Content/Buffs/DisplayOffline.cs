using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Techarria.Content.Buffs
{
	internal class MotorMalfunction : ModBuff
    {
        public override void SetStaticDefaults()
        {
			// DisplayName.SetDefault("Display Offline");
			// Description.SetDefault("Your armor is fried! You can't see!");
			Main.debuff[Type] = true;
		}

        public override void Update(Player player, ref int buffIndex)
        {
            player.headcovered = true;
            Rectangle rectangle = player.getRect();
            Dust.NewDust(rectangle.TopLeft(), rectangle.Width, rectangle.Height, DustID.MartianSaucerSpark);
        }
    }
}
