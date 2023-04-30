using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Techarria.Content.Buffs
{
	internal class DisplayOffline : ModBuff
    {
        public override void SetStaticDefaults()
        {
			// DisplayName.SetDefault("Motor Malfunction");
			// Description.SetDefault("Your armor is fried! You can't move!");
			Main.debuff[Type] = true;
		}

        public override void Update(Player player, ref int buffIndex)
        {
            player.mount.Dismount(player);
            player.moveSpeed = 0;
            if (player.velocity.Y < 0)
            {
                player.velocity.Y = 0;
            }
            player.velocity.X = 0;
            Rectangle rectangle = player.getRect();
            Dust.NewDust(rectangle.TopLeft(), rectangle.Width, rectangle.Height, DustID.MartianSaucerSpark);
        }
    }
}
