﻿using Techarria.Content.Items.Armor.Apparatus;
using Techarria.Content.Items.Materials.Molten;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Techarria.Content.Items.Armor
{
	public abstract class PowerArmor : ChargableItem
    {
        public int damageClass = 0;
        public static int ApparatusMaxCharge = 500;

    }

    public class PowerArmorPlayer : ModPlayer
    {
        public int visorCooldown = 0;
        public int frames = 0;

        public bool hasMechJump = false;
		public bool canMechJump = false;
		public bool waitMechJump = true;

		public int combatTimer = 0;
		public int helmetDepleteTimer = 0;

		public override void ModifyHurt(ref Player.HurtModifiers modifiers)
        {
            if (Player.armor[1].ModItem is BreastplateApparatus chestplate)
            {
                chestplate.Deplete(5);
            }
        }

		public override void ModifyHitByNPC(NPC npc, ref Player.HurtModifiers modifiers)
        {
            if (Player.armor[1].ModItem is BreastplateApparatus chestplate && chestplate.charge > 0)
                Player.endurance += 0.10f;
        }

		public override void ModifyHitByProjectile(Projectile proj, ref Player.HurtModifiers modifiers)
        {
            if (Player.armor[1].ModItem is BreastplateApparatus chestplate && chestplate.charge > 0)
                Player.endurance += 0.10f;
        }

        public override void UpdateEquips()
        {
            base.UpdateEquips();

            if (Player.armor[2].ModItem is LeggingsApparatus leggings && leggings.charge > 0)
            {
                hasMechJump = true;
                return;
            }
            else
            {
                hasMechJump = false;
                canMechJump = false;
            }
        }

        public override void PostUpdate()
        {

            frames++;
            if (visorCooldown > 0)
                visorCooldown--;
            if (Player.armor[0].ModItem is PowerArmor helmet && combatTimer > 0)
            {
                if (helmetDepleteTimer == 0)
                {
                    helmetDepleteTimer = 60;
                    helmet.Deplete(1);
                    combatTimer--;
                } else
                {
                    helmetDepleteTimer--;
                    combatTimer--;
                }
            }

            if ((Player.velocity.Y == 0f || Player.sliding || (Player.autoJump && Player.justJumped) || Player.grappling[0] >= 0) && hasMechJump)
            {
                canMechJump = true;
                waitMechJump = true;
            }
            else
            {
                if (canMechJump && Player.armor[2].ModItem is LeggingsApparatus leggings && Player.controlJump && !waitMechJump)
                {
                    if (Player.jump > 0)
                    {
                        waitMechJump = true;
                        return ;
                    }
                    Player.velocity.Y = (0f - Player.jumpSpeed) * Player.gravDir;
                    Player.jump = (int)(Player.jumpHeight);
                    canMechJump = false;
                    leggings.Deplete(1);
                    UpdateEquips();
                    SoundEngine.PlaySound(new SoundStyle("Techarria/Content/Sounds/Boing"), Player.position);
                    for (int i = 0; i < 20; i++)
                        Dust.NewDust(Player.TopLeft, Player.width, Player.height, Terraria.ID.DustID.TreasureSparkle);
                }
                waitMechJump = Player.controlJump;
            }

        }

        public override void OnHitAnything(float x, float y, Entity victim)
        {
            combatTimer = 120;
            if (Player.armor[0].ModItem is RadiatorApparatus helmet && helmet.IsArmorSet(Player.armor[0], Player.armor[1], Player.armor[2]) && victim is NPC npc)
            {
                npc.AddBuff(Terraria.ID.BuffID.OnFire, 600);
                for (int i = 0; i < 3; i++)
                {
                    Item item = Player.armor[i];
                    if (item.ModItem is PowerArmor armor && 1 != armor.Deplete(1))
                    {
                    }
                }
            }

        }

        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            foreach (Item item in Player.inventory)
            {
                if (item.type == ModContent.ItemType<MoltenSpikeSteel>())
                {
                    if (Player.dead)
                    {
                        WorldGen.PlaceLiquid((int)Player.Center.X / 16, (int)Player.Center.Y / 16, (byte)LiquidID.Lava, 255);
                    }
                    item.TurnToAir();
                }
            }
        }
    }
}
