using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Steamworks;
using Techarria;

namespace techarria.Content.Items.Tools
{
    internal class Skullotron : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.value = Item.buyPrice(gold: 3, silver: 25);
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = false;
        }

        public override void UseAnimation(Player player)
        {
            player.GetModPlayer<SkullotronPlayer>().Activate(20);
        }
    }

    public class SkullotronPlayer : ModPlayer
    {
        public Vector2 velocity = new Vector2();
        public int time = 0;
        public int duration = 0;
        public Projectile skull;
        public bool slow = false;
        public Vector2 exitVelocity = new Vector2();

        public static float speed = 25;
        public static float accel = 0.1f;
        public static float decel = 1;
        public static float exitSpeed = 5;

        public void Activate(int duration)
        {
            this.duration = duration;
            time = duration;
            //Vector2 moveVec = new Vector2(Player.controlRight.Int() - Player.controlLeft.Int(), Player.controlDown.Int() - (Player.controlUp || Player.controlJump).Int());
            velocity = /*moveVec != Vector2.Zero ? moveVec : */Player.velocity;
            velocity.SafeNormalized(Vector2.Zero);
            slow = false;
        }

        public override void PreUpdateMovement()
        {
            Vector2 moveVec = new Vector2(Player.controlRight.Int() - Player.controlLeft.Int(), Player.controlDown.Int() - (Player.controlUp || Player.controlJump).Int());
            if (moveVec != Vector2.Zero)
            {
                moveVec.Normalize();
            }
            if (time > 0) {
                Player.noFallDmg = true;
                velocity = velocity.MoveTowards(moveVec, accel);
                velocity.SafeNormalized(Vector2.Zero);
                Player.velocity = velocity * speed;
                time--;
                if (time == 0)
                {
                    slow = true;
                }
                return;
            }
            if (slow)
            {
                Player.noFallDmg = true;
                Player.velocity = Player.velocity.MoveTowards(moveVec * exitSpeed, decel);
                if (Player.velocity == moveVec * exitSpeed)
                    slow = false;
            }
        }
    }
}
