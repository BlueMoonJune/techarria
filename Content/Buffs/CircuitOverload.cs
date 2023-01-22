using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Techarria.Content.Buffs
{
	internal class CircuitOverload : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Circuit Overload");
            Description.SetDefault("Your armor is fried! You can't use your arms!");
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.cursed = true;
            player.noItems = true;
            Rectangle rectangle = player.getRect();
            Dust.NewDust(rectangle.TopLeft(), rectangle.Width, rectangle.Height, DustID.MartianSaucerSpark);
        }
    }
}
