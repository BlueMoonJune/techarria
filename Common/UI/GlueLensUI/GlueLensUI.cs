using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Techarria.Common.Global;
using Techarria.Content.Items.Tools.Adhesive;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.ID;

namespace Techarria.Common.UI.GlueLensUI
{
	public enum GlueLensButtonType : byte
	{
		Mode = 0,
		Slimy = 1,
		Frigid = 2,
		Elastic = 3,
		Sweet = 4
	}

	public class GlueLensButton : UIElement
	{
		public static string[] textures = new string[5] { "SlimyIcon", "FrigidIcon", "ElasticIcon", "SweetIcon", "ModeIcon" };

		public static bool mouseOverAny = false;

		public Vector2 position = new();
		public GlueLensButtonType type = 0;

		public bool mouseOver = false;
		public bool pressed = false;

		public GlueLensButton(Vector2 position, GlueLensButtonType type) {
			if (Main.netMode == NetmodeID.Server) return;
			this.position = position;
			this.type = type;

			Width = new StyleDimension(TextureAssets.WireUi[0].Value.Width, 1);
			Height = new StyleDimension(TextureAssets.WireUi[0].Value.Height, 1);

			IgnoresMouseInteraction = false;
		}

		public override void Draw(SpriteBatch spriteBatch) {

			GlueLensPlayer p = Main.LocalPlayer.GetModPlayer<GlueLensPlayer>();
			if (p.UIPos == new Vector2(float.MinValue)) {
				p.UIPos = Main.MouseScreen;
			}
			mouseOver = (position + p.UIPos - Main.MouseScreen).Length() <= 20;
			int modeIndex = 0;
			if (mouseOver) {
				mouseOverAny = true;
				modeIndex = 1;
			}

			int index = (int)type - 1;

			if (type == 0) {
				index = 4;
			}

			Color color;
			Texture2D modeTexture = TextureAssets.WireUi[modeIndex].Value;
			if (type == 0) {
				color = p.glueForced ? Color.White : Color.Gray;
			} else
			color = p.modes[(int)type - 1] switch {
				0 => Color.Gray,
				1 => Color.LightGray,
				_ => Color.White,
			};

			spriteBatch.Draw(modeTexture, position + p.UIPos - modeTexture.Size() / 2, color);

			Texture2D icon;

			icon = ModContent.Request<Texture2D>("Techarria/Common/UI/GlueLensUI/" + textures[index]).Value;

			spriteBatch.Draw(icon, position + p.UIPos - icon.Size() / 2, color);

			if (Main.mouseLeft && mouseOver) {
				if (!pressed) {
					if (type == 0) {
						p.glueForced = !p.glueForced;
					}
					else {
						p.modes[index]++;
						p.modes[index] %= 3;
					}
					pressed = true;
				}
			}
			else {
				pressed = false;
			}


		}
	}

	public class GlueLensUI : UIState
	{
		public override void OnInitialize() {
			Append(new GlueLensButton(new(0), GlueLensButtonType.Mode));
			Append(new GlueLensButton(new(-30, -30), GlueLensButtonType.Slimy));
			Append(new GlueLensButton(new(-30, 30), GlueLensButtonType.Frigid));
			Append(new GlueLensButton(new(30, 30), GlueLensButtonType.Elastic));
			Append(new GlueLensButton(new(30, -30), GlueLensButtonType.Sweet));
		}
	}

	public class GlueLensSystem : ModSystem
	{
		internal GlueLensUI GlueLensUI;
		private UserInterface _glueLensUI;

		public override void Load() {
			GlueLensUI = new();
			GlueLensUI.Activate();
			_glueLensUI = new();
			_glueLensUI.SetState(GlueLensUI);
		}

		public override void UpdateUI(GameTime gameTime) {
			_glueLensUI?.Update(gameTime);
		}

		public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers) {
			GlueLensPlayer p = Main.LocalPlayer.GetModPlayer<GlueLensPlayer>();
			int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
			if (mouseTextIndex != -1 && p.UIOpen) {
				layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
					"Techarria: MultiAdhesive UI",
					delegate {
						GlueLensButton.mouseOverAny = false;
						_glueLensUI.Draw(Main.spriteBatch, new GameTime());
						return true;
					},
					InterfaceScaleType.UI)
				);
			}
		}
	}
}
