using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Techarria.Content.Items.Tools.Adhesive;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Techarria.Common.UI.MultiAdhesiveUI
{
	public enum MultiAdhesiveButtonType : byte
	{
		Mode = 0,
		Slimy = 1,
		Frigid = 2,
		Elastic = 3,
		Sweet = 4
	}

	public class MultiAdhesiveButton : UIElement
	{
		public static string[] textures = new string[6] { "SlimyIcon", "FrigidIcon", "ElasticIcon", "SweetIcon", "AdhesiveIcon", "SolventIcon" };

		public static bool mouseOverAny = false;

		public Vector2 position = new();
		public MultiAdhesiveButtonType type = 0;

		public bool mouseOver = false;
		public bool pressed = false;

		public MultiAdhesiveButton(Vector2 position, MultiAdhesiveButtonType type) {
            if (Main.netMode == NetmodeID.Server) return;
            this.position = position;
			this.type = type;

			Width = new StyleDimension(TextureAssets.WireUi[0].Value.Width, 1);
			Height = new StyleDimension(TextureAssets.WireUi[0].Value.Height, 1);

			IgnoresMouseInteraction = false;
		}

		public override void Draw(SpriteBatch spriteBatch) {

			MultiAdhesivePlayer p = Main.LocalPlayer.GetModPlayer<MultiAdhesivePlayer>();
			if (p.UIPos == new Vector2(float.MinValue)) {
				p.UIPos = Main.MouseScreen;
			}
			mouseOver = (position + p.UIPos - Main.MouseScreen).Length() <= 20;
			int modeIndex = p.removing ? 8 : 0;
			if (mouseOver) {
				mouseOverAny = true;
				modeIndex++;
			}
			Texture2D modeTexture = TextureAssets.WireUi[modeIndex].Value;


			Color color;
			if (((int)MathF.Pow(2, (int)type - 1) & (int)p.GlueTypes) == (int)MathF.Pow(2, (int)type - 1)) {
				color = new(1f, 1f, 1f);
			}
			else if (mouseOver) {
				color = new(0.75f, 0.75f, 0.75f);
			}
			else {
				color = new(0.5f, 0.5f, 0.5f);
			}

			spriteBatch.Draw(modeTexture, position + p.UIPos - modeTexture.Size() / 2, color);

			Texture2D icon;

			int index = (int)type - 1;

			switch (type) {
				case 0:
					index = p.removing ? 5 : 4;
					break;
			}

			icon = ModContent.Request<Texture2D>("Techarria/Common/UI/MultiAdhesiveUI/" + textures[index]).Value;

			spriteBatch.Draw(icon, position + p.UIPos - icon.Size() / 2, color);

			if (Main.mouseLeft && mouseOver) {
				if (!pressed) {
					if (type == 0) {
						p.removing = !p.removing;
					}
					else {
						p.GlueTypes = (GlueTypes)((byte)p.GlueTypes ^ (byte)MathF.Pow(2, (int)type - 1));
					}
					pressed = true;
				}
			}
			else {
				pressed = false;
			}


		}
	}

	public class MultiAdhesiveUI : UIState
	{
		public override void OnInitialize() {
			Append(new MultiAdhesiveButton(new(0), 0));
			Append(new MultiAdhesiveButton(new(-30, -30), MultiAdhesiveButtonType.Slimy));
			Append(new MultiAdhesiveButton(new(-30, 30), MultiAdhesiveButtonType.Frigid));
			Append(new MultiAdhesiveButton(new(30, 30), MultiAdhesiveButtonType.Elastic));
			Append(new MultiAdhesiveButton(new(30, -30), MultiAdhesiveButtonType.Sweet));
		}
	}

	public class MultiAdhesiveSystem : ModSystem
	{
		internal MultiAdhesiveUI MultiAdhesiveUI;
		private UserInterface _multiAdhesiveUI;

		public override void Load() {
			MultiAdhesiveUI = new();
			MultiAdhesiveUI.Activate();
			_multiAdhesiveUI = new();
			_multiAdhesiveUI.SetState(MultiAdhesiveUI);
		}

		public override void UpdateUI(GameTime gameTime) {
			_multiAdhesiveUI?.Update(gameTime);
		}

		public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers) {
			MultiAdhesivePlayer p = Main.LocalPlayer.GetModPlayer<MultiAdhesivePlayer>();
			int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
			if (mouseTextIndex != -1 && p.UIOpen) {
				layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
					"Techarria: MultiAdhesive UI",
					delegate {
						MultiAdhesiveButton.mouseOverAny = false;
						_multiAdhesiveUI.Draw(Main.spriteBatch, new GameTime());
						return true;
					},
					InterfaceScaleType.UI)
				);
			}
		}
	}
}
