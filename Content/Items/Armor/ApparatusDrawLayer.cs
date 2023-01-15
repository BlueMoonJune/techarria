using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System;
using Techarria.Content.Items.Armor.Apparatus;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Techarria.Content.Items.Armor
{
    public abstract class ApparatusDrawLayer : PlayerDrawLayer
    {

        public Texture2D texture = null;
        public int itemType = 0;
        public override Position GetDefaultPosition()
        {
            System.Console.WriteLine("Techarria: GetDefaultPosition");
            return new AfterParent(PlayerDrawLayers.Torso);
        }

        public abstract void SetData();

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {


            if (texture == null || itemType == 0)
            {
            }
            SetData();

            Player drawPlayer = drawInfo.drawPlayer;
            Vector2 pos = drawPlayer.bodyPosition + drawInfo.bodyVect + new Vector2((int)(drawInfo.Position.X - Main.screenPosition.X - (float)drawPlayer.bodyFrame.Width / 2f + (float)drawPlayer.width / 2f), (int)(drawInfo.Position.Y - Main.screenPosition.Y + (float)drawPlayer.height - (float)drawPlayer.bodyFrame.Height + 4f));
            int frames = drawPlayer.GetModPlayer<PowerArmorPlayer>().frames;

            SpriteEffects effects = SpriteEffects.None;
            if (drawPlayer.direction == -1)
            {
                effects |= SpriteEffects.FlipHorizontally;
            }
            if (drawPlayer.gravDir == -1f)
            {
                effects |= SpriteEffects.FlipVertically;
            }

            for (int i = 0; i < 3; i++)
            {
                if (drawPlayer.armor[i].type == itemType)
                {

                    Color color = new Color(0, 0, 0, 0);
                    if (drawPlayer.armor[i].ModItem is ChargableItem item && item.charge > 0)
                    {
                        color = Color.White;
                        if (i > 0 && drawPlayer.armor[0].ModItem is PowerArmor helm)
                        {
                            if (helm.damageClass == 1)
                                color = Color.Orange;
                            if (helm.damageClass == 2)
                                color = new Color(0f, 1f, 0.75f);
                            if (helm.damageClass == 3)
                                color = new Color(0.9f, 0f, 1f);
                            if (helm.damageClass == 4)
                                color = new Color(0f, 0.8f, 1f);
                        } else
                        {
                            if (this is RadiatorApparatusOverlay)
                            {
                                color *= (MathF.Sin(frames / 10f) * 0.1f + 0.9f);
                            }
                            if (this is ControllingApparatusOverlay && frames % 60 >= 30)
                            {
                                return;
                            }
                        }

                    }
                    
                    DrawData data = new DrawData(texture, pos, drawPlayer.bodyFrame, color, drawPlayer.bodyRotation, drawInfo.bodyVect, 1f, effects, 0);
                    drawInfo.DrawDataCache.Add(data);
                    return;
                }
            }

        }
    }
}
