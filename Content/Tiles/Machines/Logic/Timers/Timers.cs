using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Techarria.Content.Dusts;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Techarria.Content.Tiles.Machines.Logic.Timers
{
	public class TimerTE : ModTileEntity
    {
        public bool justToggled = false;
        public bool active;
        public int timer;
        public int duration;

        public override bool IsTileValidForEntity(int x, int y)
        {
            return (ModContent.GetModTile(Main.tile[x, y].TileType) is Timer);
        }

        public void Toggle(int dur)
        {
            if (justToggled)
            {
                return;
            }

            justToggled = true;
            active = !active;
            if (!active)
            {
                Main.tile[Position.X, Position.Y].TileFrameY = 0;
                return;
            }
            Main.tile[Position.X, Position.Y].TileFrameY = 16;
            timer = dur;
            duration = dur;
        }

        public override void Update()
        {
            justToggled = false;
            if (!active) return;

            if (timer > 0)
            {
                timer--;
                return;
            }
            timer = duration - 1;
            Wiring.TripWire(Position.X, Position.Y, 1, 1);

        }

        public override void SaveData(TagCompound tag)
        {
            tag.Add("active", active);
            tag.Add("duration", duration);
            tag.Add("timer", timer);
            base.SaveData(tag);
        }

        public override void LoadData(TagCompound tag)
        {
            active = tag.GetBool("active");
            duration = tag.GetInt("duration");
            timer = tag.GetInt("timer");
            base.LoadData(tag);
        }
    }

    public abstract class Timer : ModTile
    {
        public int duration;

        public override void SetStaticDefaults()
        {

            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = false;
            Main.tileSolid[Type] = false;

            AddMapEntry(new Color(60/255f, 50/255f, 44/255f), CreateMapEntryName());

			DustType = ModContent.DustType<Spikesteel>();

			HitSound = SoundID.Tink;
        }

        public override void PlaceInWorld(int i, int j, Item item)
        {
            ModContent.GetInstance<TimerTE>().Place(i, j);
        }

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (fail || effectOnly) return;
            ModContent.GetInstance<TimerTE>().Kill(i, j);
        }

        public override bool RightClick(int i, int j)
        {
            ((TimerTE)TileEntity.ByPosition[new Point16(i, j)]).Toggle(duration);
            return true;
        }

        public override void HitWire(int i, int j)
        {
            ((TimerTE)TileEntity.ByPosition[new Point16(i, j)]).Toggle(duration);
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {

            TimerTE timerTE = ((TimerTE)TileEntity.ByPosition[new Point16(i, j)]);
            if (!timerTE.active) return;

            Vector2 TileOffset = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
            Vector2 pos = new Vector2(i, j) * 16 - Main.screenPosition + TileOffset;

            float progress = 1 - timerTE.timer / (float)timerTE.duration;
            spriteBatch.Draw(
                ModContent.Request<Texture2D>("Techarria/Content/Tiles/Machines/Logic/Timers/TimerIndicator").Value,
                pos,
                new Color(progress, progress, progress)
            );
        }
    }
    public class FiveSecondTimer : Timer
    {
        public override void SetStaticDefaults()
        {
            duration = 300;
            base.SetStaticDefaults();

            ItemDrop = ModContent.ItemType<Items.Placeables.Timers.FiveSecondTimer>();
        }
        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;

            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ModContent.ItemType<Items.Placeables.Timers.FiveSecondTimer>();
        }
    }
    public class ThreeSecondTimer : Timer
    {
        public override void SetStaticDefaults()
        {
            duration = 180;
            base.SetStaticDefaults();

            ItemDrop = ModContent.ItemType<Items.Placeables.Timers.ThreeSecondTimer>();
        }
        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;

            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ModContent.ItemType<Items.Placeables.Timers.ThreeSecondTimer>();
        }
    }
    public class OneSecondTimer : Timer
    {
        public override void SetStaticDefaults()
        {
            duration = 60;
            base.SetStaticDefaults();

            ItemDrop = ModContent.ItemType<Items.Placeables.Timers.OneSecondTimer>();
        }
        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;

            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ModContent.ItemType<Items.Placeables.Timers.OneSecondTimer>();
        }
    }
    public class HalfSecondTimer : Timer
    {
        public override void SetStaticDefaults()
        {
            duration = 30;
            base.SetStaticDefaults();

            ItemDrop = ModContent.ItemType<Items.Placeables.Timers.HalfSecondTimer>();
        }
        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;

            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ModContent.ItemType<Items.Placeables.Timers.HalfSecondTimer>();
        }
    }

    public class QuarterSecondTimer : Timer
    {
        public override void SetStaticDefaults()
        {
            duration = 15;
            base.SetStaticDefaults();

            ItemDrop = ModContent.ItemType<Items.Placeables.Timers.QuarterSecondTimer>();
        }
        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;

            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ModContent.ItemType<Items.Placeables.Timers.QuarterSecondTimer>();
        }
    }
}
