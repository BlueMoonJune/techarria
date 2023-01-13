using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Techarria.Drills
{

    public abstract class BaseDrill : ModProjectile
    {
        // Cooldown between drill sound in ticks
        private const int DrillSoundCooldown = 27;

        private int _drillTimer;
        private int _drillSoundTimer;

        // Sound to play periodically whilst drilling
        protected virtual SoundStyle DrillSound { get; }

        // Tier of the drill (pickaxe tier)
        protected abstract int DrillTier { get; }

        // Cooldown between drill 'picks' in ticks
        protected abstract int DrillCooldown { get; }

        // Drill is enabled and should be visible
        protected bool IsDrillEnabled
        {
            get => Projectile.ai[0] == 1f;
            set => Projectile.ai[0] = value ? 1f : 0f;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Background");
        }

        public override void SetDefaults()
        {
            Projectile.width = 38;
            Projectile.height = 38;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.light = 0.8f;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 1;

            // Trail?
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 35;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override void AI()
        {
            Projectile.timeLeft = 2;
            // Handle enabling/disabling the drill function
            if (Projectile.owner == Main.myPlayer)
            {
                bool shouldBeActive = Main.mouseRight;
                if (IsDrillEnabled && !shouldBeActive)
                {
                    Main.NewText("disabled" + Projectile.whoAmI);
                    // Disable the drill
                    IsDrillEnabled = false;
                    _drillTimer = DrillCooldown;
                    _drillSoundTimer = DrillSoundCooldown;
                    Projectile.netUpdate = true;
                }
                else if (!IsDrillEnabled && shouldBeActive)
                {
                    Main.NewText("enabled" + Projectile.whoAmI);
                    // Enable the drill
                    IsDrillEnabled = true;
                    Projectile.netUpdate = true;
                }
            }

            // Find the parent yoyo
            Projectile proj = Main.projectile[(int)Projectile.ai[1]];
            if (!proj.active || proj.owner != Projectile.owner || proj.aiStyle != 99)
            {
                Projectile.Kill();
                return;
            }

            // Follow the parent yoyo around
            Projectile.Center = proj.Center;

            if (!IsDrillEnabled)
            {
                // Do not update if the drill is disabled
                return;
            }



            Projectile.rotation -= 0.60f;

            // Play the drill sound periodically
            if (Main.netMode != NetmodeID.Server && !string.IsNullOrEmpty(DrillSound.SoundPath) && _drillSoundTimer > 0)
            {
                _drillSoundTimer--;
                if (_drillSoundTimer == 0)
                {
                    SoundEngine.PlaySound(DrillSound, Projectile.position);
                    _drillSoundTimer = DrillSoundCooldown;
                }
            }

            // Prevent the projectile from 'picking' a tile until this timer has depleted
            if (_drillTimer > 0)
            {
                _drillTimer--;
            }
            else
            {
                // Attempt to 'pick' any nearby solid tiles
                Player player = Main.player[Projectile.owner];
                bool success = false;
                void TryPick(int cX, int cY)
                {
                    int x = (int)((proj.Center.X + (cX * proj.width * 0.5f + 8 * cX)) / 16);
                    int y = (int)((proj.Center.Y + (cY * proj.height * 0.5f + 8 * cY)) / 16);
                    
                    if (!Main.tile[x, y].HasTile) // TODO check tile exists + check solid
                    {
                        return;
                    }

                    player.PickTile(x, y, DrillTier);
                    if (!Main.tile[x, y].HasTile)
                    {
                        success = true;
                    }
                }

                TryPick(-1, 0);
                TryPick(0, 1);
                TryPick(1, 0);
                TryPick(0, -1);
                TryPick(-1, -1);
                TryPick(1, -1);
                TryPick(-1, 1);
                TryPick(1, 1);

                // Reset the cooldown if successful
                if (success)
                {
                    _drillTimer = DrillCooldown;
                }
            }   
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return IsDrillEnabled;
        }
    }
}