using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Techarria.Content.Items.Armor
{
    internal class ApparatusDrawLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition()
        {
            System.Console.WriteLine("Techarria: GetDefaultPosition");
            return new AfterParent(PlayerDrawLayers.Torso);
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Player drawPlayer = drawInfo.drawPlayer;
            Vector2 pos = drawPlayer.bodyPosition + drawInfo.bodyVect + new Vector2((int)(drawInfo.Position.X - Main.screenPosition.X - (float)drawPlayer.bodyFrame.Width / 2f + (float)drawPlayer.width / 2f), (int)(drawInfo.Position.Y - Main.screenPosition.Y + (float)drawPlayer.height - (float)drawPlayer.bodyFrame.Height + 4f));

            SpriteEffects effects = SpriteEffects.None;
            if (drawPlayer.direction == -1)
            {
                effects |= SpriteEffects.FlipHorizontally;
            }
            if (drawPlayer.gravDir == -1f)
            {
                effects |= SpriteEffects.FlipVertically;
            }

            DrawData data = new DrawData(ModContent.Request<Texture2D>("Techarria/Content/Items/Armor/Apparatus/BreastplateApparatus_Body_Overlay").Value, pos, drawPlayer.bodyFrame, Color.White, drawPlayer.bodyRotation, drawInfo.bodyVect, 1f, effects, 0);
            drawInfo.DrawDataCache.Add(data);

        }
    }
}
