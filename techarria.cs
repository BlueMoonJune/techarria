using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using System;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using ReLogic.Content;
using System.Collections.Generic;
using Techarria.Content.Tiles;

namespace Techarria
{

	public class Techarria : Mod
    {
        public static float GenerationMultiplier = 1;
        public static float CapacityMultiplier = 1;
        public static float UsageMultiplier = 1;

        public static bool BlockDusts = false;

		public static Dictionary<int, NPC> moddedBannerToNPC = new();

        public static bool Intersects(Rectangle rect, LineSegment line)
        {
            Rectangle aabb = new Rectangle();
            aabb.X = (int)Math.Min(line.Start.X, (int)line.End.X);
            aabb.Y = (int)Math.Min(line.Start.Y, (int)line.End.Y);
            aabb.Width = (int)Math.Max(line.Start.X, (int)line.End.X) - aabb.X;
            aabb.Height = (int)Math.Max(line.Start.Y, (int)line.End.Y) - aabb.Y;
            if (!aabb.Intersects(rect))
            {
                return false;
            }

            float m = (line.Start.Y - line.End.Y) / (line.Start.X - line.End.X);

            Vector2 slideTR = new Vector2(rect.Left, rect.Top - rect.Width * m);
            Vector2 slideBR = new Vector2(rect.Left, slideTR.Y + rect.Width);

            float topBound = Math.Max(rect.Top, rect.Bottom);
            topBound = Math.Max(topBound, slideTR.Y);
            topBound = Math.Max(topBound, slideBR.Y);
            float bottomBound = Math.Min(rect.Top, rect.Bottom);
            bottomBound = Math.Min(bottomBound, slideTR.Y);
            bottomBound = Math.Min(bottomBound, slideBR.Y);

            float b = m * (rect.Left - line.Start.X) + line.Start.Y;

            return b <= topBound && b >= bottomBound;
        }

        public static void print(object obj)
        {
            System.Console.WriteLine(obj);
        }

        /// <summary>Whether or not this type is an item transfer tile</summary>
        public static bool[] tileIsTransferDuct = new bool[TileLoader.TileCount];
        /// <summary>Whether or not transfer ducts should connect to this type of tile</summary>
		public static bool[] tileConnectToPipe = new bool[TileLoader.TileCount];

        /// <summary>
        /// Updates the texture of the transfer duct at the specified coords
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void TransferDuctFrame(int x, int y)
        {
            if (tileIsTransferDuct[Main.tile[x, y].TileType])
            {
                ModTile tileInstance = TileLoader.GetTile(Main.tile[x, y].TileType);
                bool idc = false;
                tileInstance.TileFrame(x, y, ref idc, ref idc);
            }
        }


        public override void Load()
        {
            Terraria.On_WorldGen.paintTile += WorldGen_paintTile;
            Terraria.On_Dust.NewDust += DustDetour;
            Terraria.On_Main.DrawWires += DrawPowerTransfer;
			Terraria.On_Player.Update += On_Player_Update;
		}

		int biorepulsionFieldType;
		int invertedBiorepulsionFieldType;
		private void On_Player_Update(On_Player.orig_Update orig, Player self, int i) {
			Main.tileSolid[biorepulsionFieldType] = true;
			Main.tileSolid[invertedBiorepulsionFieldType] = false;
			orig(self, i);
			Main.tileSolid[biorepulsionFieldType] = false;
			Main.tileSolid[invertedBiorepulsionFieldType] = true;
		}

		public override void PostSetupContent() {
			Console.WriteLine("Techaria: Linking Banners...");
			foreach (Mod mod in ModLoader.Mods) {
				foreach (ModNPC npc in mod.GetContent<ModNPC>()) {
					if (npc.BannerItem != 0) {
						moddedBannerToNPC.TryAdd(npc.BannerItem, npc.NPC);
						Console.WriteLine($"Linked banner with item type \"{npc.BannerItem}\" to \"{npc.DisplayName.Value}\"");
					}
				}
			}
			biorepulsionFieldType = ModContent.TileType<BiorepulsionField>();
			invertedBiorepulsionFieldType = ModContent.TileType<InvertedBiorepulsionField>();
		}

		private void DrawPowerTransfer(Terraria.On_Main.orig_DrawWires orig, Main self)
        {
            orig(self);
            foreach (Wire wire in Power.DisplayInfos.Keys)
            {
                Color color;
                switch (wire.C)
                {
                    case 0:
                        color = new Color(218 / 255f, 2 / 255f, 5 / 255f);
                        break;
                    case 1:
                        color = new Color(2 / 255f, 124 / 255f, 218 / 255f);
                        break;
                    case 2:
                        color = new Color(2 / 255f, 218 / 255f, 91 / 255f);
                        break;
                    default:
                        color = new Color(218 / 255f, 194 / 255f, 2 / 255f);
                        break;
                }
                int j = wire.X;
                int i = wire.Y;
                PowerDisplayInfo displayInfo = Power.DisplayInfos[wire];
                Main.spriteBatch.Draw(
                    ModContent.Request<Texture2D>("Techarria/Content/PowerTransferDisplay").Value,
                    new Rectangle(j * 16 - (int)Main.screenPosition.X, i * 16 - (int)Main.screenPosition.Y, 16, 16),
                    new Rectangle(0, 0, 16, 16),
                    color * (1 - displayInfo.age / 30f)
                );
                Main.spriteBatch.Draw(
                    ModContent.Request<Texture2D>("Techarria/Content/PowerTransferDisplay_TheSecond").Value,
                    new Rectangle(j * 16 - (int)Main.screenPosition.X, i * 16 - (int)Main.screenPosition.Y, 16, 16),
                    new Rectangle(0, 0, 16, 16),
                    Color.White * (1 - displayInfo.age / 30f)
                );
                if (++displayInfo.age >= 30)
                    Power.DisplayInfos.Remove(wire);
            }
        }

		
        private int DustDetour(Terraria.On_Dust.orig_NewDust orig, Vector2 Position, int Width, int Height, int Type, float SpeedX, float SpeedY, int Alpha, Color newColor, float Scale)
        {
            if (BlockDusts)
            {
                return Main.maxDust;
            }
            return orig(Position, Width, Height, Type, SpeedX, SpeedY, Alpha, newColor, Scale);
        }

        public override void Unload()
        {
            Terraria.On_WorldGen.paintTile -= WorldGen_paintTile;
			Terraria.On_Dust.NewDust -= DustDetour;
			Terraria.On_Main.DrawWires -= DrawPowerTransfer;
			Terraria.On_Player.Update -= On_Player_Update;
		}

        private bool WorldGen_paintTile(Terraria.On_WorldGen.orig_paintTile orig, int x, int y, byte color, bool broadCast)
        {
            bool result = orig.Invoke(x, y, color, broadCast);
            TransferDuctFrame(x, y);
            TransferDuctFrame(x+1, y);
            TransferDuctFrame(x, y+1);
            TransferDuctFrame(x-1, y);
            TransferDuctFrame(x, y-1);

            return result;
        }
	}
}