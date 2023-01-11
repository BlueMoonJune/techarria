using Microsoft.Xna.Framework.Graphics;
using Techarria.Content.Items.Armor.Apparatus;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using Terraria.UI;

namespace Techarria.Content.Items.Armor
{
    internal abstract class PowerArmor : ChargableItem
    {
        public static int ApparatusMaxCharge = 200;
    }

    public class PowerArmorPlayer : ModPlayer
    {
        UserInterface EnergyUI = new UserInterface();

        bool hasMechJump = false;
        bool canMechJump = false;
        bool waitMechJump = true;
        bool performingMechJump = false;

        int combatTimer = 0;
        int helmetDepleteTimer = 0;

        public override void Hurt(bool pvp, bool quiet, double damage, int hitDirection, bool crit, int cooldown)
        {
            if (Player.armor[1].ModItem is BreastplateApparatus chestplate)
            {
                chestplate.Deplete(5);
            }
        }

        public override void ModifyHitByNPC(NPC npc, ref int damage, ref bool crit)
        {
            if (Player.armor[1].ModItem is BreastplateApparatus chestplate && chestplate.charge > 0)
                Player.endurance += 0.05f;
        }

        public override void ModifyHitByProjectile(Projectile proj, ref int damage, ref bool crit)
        {
            if (Player.armor[1].ModItem is BreastplateApparatus chestplate && chestplate.charge > 0)
                Player.endurance += 0.05f;
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
            if (Player.armor[0].ModItem is PowerArmor helmet && combatTimer > 0)
            {
                if (helmetDepleteTimer == 0)
                {
                    helmetDepleteTimer = 20;
                    helmet.Deplete(1);
                    combatTimer--;
                } else
                {
                    helmetDepleteTimer--;
                    combatTimer--;
                }
            }

            if ((Player.velocity.Y == 0f || Player.sliding || (Player.autoJump && Player.justJumped)) && hasMechJump)
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
            Main.NewText(Player.armor);
            if (Player.armor[0].ModItem is RadiatorApparatus helmet && helmet.IsArmorSet(Player.armor[0], Player.armor[1], Player.armor[2]))
            {
                for (int i = 0; i < 3; i++)
                {
                    Item item = Player.armor[i];
                    if (item.ModItem is PowerArmor armor && 1 != armor.Deplete(1))
                    {
                        Player.meleeEnchant = 0;
                    }
                }
            }
        }
    }
}
