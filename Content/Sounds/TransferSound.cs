using Microsoft.Xna.Framework;
using ReLogic.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Techarria.Content.Sounds
{
    public struct TransferSoundInfo
    {
        public int timer = 0;
        public bool isLooping = false;
        public SlotId slot;

        public TransferSoundInfo()
        {
        }
    }

    public class TransferSound : ModSystem
    {
        public static Dictionary<Point16, TransferSoundInfo> soundValues = new();
        public static int soundLength;
        public static SoundStyle initSound;
        public static SoundStyle loopSound;

        public override void SetupContent()
        {
            initSound = new("Techarria/Content/Sounds/Transfer", SoundType.Sound);
            loopSound = new("Techarria/Content/Sounds/TransferLoop", SoundType.Sound);
            loopSound.IsLooped = true;
            soundLength = (int)(initSound.GetRandomSound().Duration.TotalSeconds*60);
        }

        public override void PostUpdateEverything()
        {
            List<Point16> toRemove = new();
            foreach (var (p, i) in soundValues)
            {
                if (SoundEngine.TryGetActiveSound(i.slot, out ActiveSound result))
                {
                    result.Volume = (1-((float)i.timer / soundLength)) * 0.5f;
                }
                if (i.timer > soundLength)
                {
                    if (result != null)
                        result.Stop();
                    toRemove.Add(p);
                    continue;
                }
                TransferSoundInfo info = i;
                info.timer++;
                soundValues[p] = info;
            }

            foreach (Point16 p in toRemove)
                soundValues.Remove(p);
        }

        public static bool PlaySound(Point16 p)
        {
            if (soundValues.ContainsKey(p))
            {
                TransferSoundInfo info = soundValues[p];
                if (!info.isLooping)
                {
                    info.slot = SoundEngine.PlaySound(loopSound, new Vector2(p.X * 16 + 8, p.Y * 16 + 8));
                    info.isLooping = true;
                }
                info.timer = 0;
                soundValues[p] = info;
                return false;
            }
            initSound.Volume = 0.2f;
            SoundEngine.PlaySound(initSound, new Vector2(p.X * 16 + 8, p.Y * 16 + 8));
            soundValues.Add(p, new TransferSoundInfo());
            return true;
        }
    }
}
