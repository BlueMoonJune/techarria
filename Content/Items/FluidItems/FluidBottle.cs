using Terraria.ModLoader;
using Techarria.Content.Tiles.FluidTransfer;
using Terraria;
using Terraria.ID;
using System.IO;
using Terraria.ModLoader.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria.GameContent;

namespace Techarria.Content.Items.FluidItems
{
    public class FluidBottle : ModItem
    {
        public int storedItem;

        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = FluidTank.maxStorage;
        }

        public override void SaveData(TagCompound tag)
        {
            tag["storedItem"] = storedItem;
        }

        public override void LoadData(TagCompound tag)
        {
            storedItem = tag.Get<int>("storedItem");
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(storedItem);
        }

        public override void NetReceive(BinaryReader reader)
        {
            storedItem = reader.ReadInt32();
        }

		public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale) {
			Texture2D texture = TextureAssets.Item[storedItem].Value;

			Rectangle sourceRect = new Rectangle(0, 0, texture.Width, texture.Height);
			if (Main.itemAnimationsRegistered.Contains(storedItem)) {
				sourceRect = Main.itemAnimations[storedItem].GetFrame(texture);
			}

			spriteBatch.Draw(TextureAssets.Item[storedItem].Value, position - new Vector2(8), sourceRect, drawColor.MultiplyRGBA(drawColor));
			return true;
		}

		public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI) {
			Texture2D texture = TextureAssets.Item[storedItem].Value;

			Rectangle sourceRect = new Rectangle(0, 0, texture.Width, texture.Height);
			if (Main.itemAnimationsRegistered.Contains(storedItem)) {
				sourceRect = Main.itemAnimations[storedItem].GetFrame(texture);
			}

			spriteBatch.Draw(TextureAssets.Item[storedItem].Value, Main.item[whoAmI].Center - new Vector2(8), sourceRect, lightColor.MultiplyRGBA(alphaColor), rotation, Vector2.Zero, scale, SpriteEffects.None, 0);
			return true;
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (storedItem != 0)
            {
                tooltips.Add(new TooltipLine(Mod, "StoredLiquid", "Storing: " + Lang.GetItemNameValue(storedItem)));
            } else
            {
                tooltips.Add(new TooltipLine(Mod, "StoredLiquid", "Storing: nothing"));
            }
            
            Player player = Main.player[Main.myPlayer];

            base.ModifyTooltips(tooltips);
        }
    }
}
