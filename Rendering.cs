using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using System;
using Terraria.DataStructures;
using Techarria.Content.Tiles;
using Techarria.Content.Tiles.Machines;
using System.Collections.Generic;
using Techarria.Common.Global;
using Techarria.Content.Items.Tools.Adhesive;
using Techarria.Content.Dusts;
using Techarria.Content.Entities;
using Techarria.Content.Tiles.Machines.Logic;
using Terraria.UI.Chat;
using ReLogic.Graphics;
using Terraria.GameContent.Creative;

namespace Techarria
{
	internal class Rendering : ModSystem
    {
		static GraphicsDevice graphicsDevice;

		static BasicEffect basicEffect;

		static Effect effect;

		Texture2D texture;

		public Rectangle getGlueFraming(int i, int j, int c) {
			Rectangle value = new Rectangle(0, 0, 16, 16);
			if (Main.tile[i + 1, j].Get<Glue>().GetChannel(c))
				value.X += 16;
			if (Main.tile[i, j + 1].Get<Glue>().GetChannel(c))
				value.X += 32;
			if (Main.tile[i - 1, j].Get<Glue>().GetChannel(c))
				value.Y += 16;
			if (Main.tile[i, j - 1].Get<Glue>().GetChannel(c))
				value.Y += 32;
			return value;
		}

		public void DrawJourneyCrate(JourneyCrateTE JourneyStorage, Point16 p) {
			Texture2D texture = TextureAssets.Item[JourneyStorage.item.type].Value;

			float scale = 10f / Math.Max(texture.Width, texture.Height);


			Matrix offset = Matrix.Identity;
			offset.Translation = new Vector3(p.X * 16 + 16 - Main.screenPosition.X, p.Y * 16 + 11 - Main.screenPosition.Y, 0);
			Matrix transform = Main.Transform;
			transform = offset * transform;

			transform = Matrix.CreateScale(scale) * transform;

			Main.spriteBatch.Begin(
				SpriteSortMode.Deferred,
				BlendState.AlphaBlend,
				Main.DefaultSamplerState,
				DepthStencilState.None,
				Main.Rasterizer,
				null,
				transform
			);

			Color color = JourneyStorage.item.color;
			if (JourneyStorage.item.color == new Color()) {
				color = Color.White;
			}

			Main.spriteBatch.Draw(texture, -new Vector2(texture.Width / 2, texture.Height / 2), color);
			DynamicSpriteFont font = FontAssets.MouseText.Value;
			Item item = JourneyStorage.item;
			int ResearchRequirement = 0;
			if (item == null || item.IsAir) {
				Main.spriteBatch.End();
				return;
			}
			string text;
			if (!CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId.TryGetValue(item.type, out ResearchRequirement)) {
				text = "X";
			} else if (item.stack > ResearchRequirement) {
				text = "∞";
			} else {text = $"{item.stack}/{ResearchRequirement}";
			}
			Vector2 size = font.MeasureString(text);
			Vector2 textScale = Vector2.One / scale / Math.Max(size.X / 16, size.Y / 16);
			ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, font, text, -size/2 * textScale + new Vector2(0, 14)/scale, Color.White, 0, Vector2.Zero, textScale);

			Main.spriteBatch.End();
		}

		public void DrawStorageCrate(StorageCrateTE storage, Point16 p) {

			Texture2D texture = TextureAssets.Item[storage.item.type].Value;

			float scale = 16f / Math.Max(texture.Width, texture.Height);


			Matrix offset = Matrix.Identity;
			offset.Translation = new Vector3(p.X * 16 + 16 - Main.screenPosition.X, p.Y * 16 + 16 - Main.screenPosition.Y, 0);
			Matrix transform = Main.Transform;
			transform = offset * transform;

			transform = Matrix.CreateScale(scale) * transform;

			Main.spriteBatch.Begin(
				SpriteSortMode.Deferred,
				BlendState.AlphaBlend,
				Main.DefaultSamplerState,
				DepthStencilState.None,
				Main.Rasterizer,
				null,
				transform
			);

			Color color = storage.item.color;
			if (storage.item.color == new Color()) {
				color = Color.White;
			}

			Main.spriteBatch.Draw(texture, -new Vector2(texture.Width / 2, texture.Height / 2), color);

			Main.spriteBatch.End();
		}

		public void DrawRotaryAssembler(RotaryAssemblerTE assembler, Point16 p) {

			Matrix offset = Matrix.Identity;
			offset.Translation = new Vector3(p.X * 16 + 24 - Main.screenPosition.X, p.Y * 16 + 24 - Main.screenPosition.Y, 0);
			Matrix transform = Main.Transform;
			transform = offset * transform;
			transform = Matrix.CreateRotationZ(assembler.degrees / 180f * MathF.PI) * transform;

			transform = Matrix.CreateScale(0.5f) * transform;


			int i = 0;
			foreach (List<Item> segment in assembler.items) {
				if (segment != null) {
					Matrix segmentTransform = Matrix.CreateRotationZ((i - 2) * MathHelper.PiOver4) * transform;

					Main.spriteBatch.Begin(
						SpriteSortMode.Deferred,
						BlendState.AlphaBlend,
						Main.DefaultSamplerState,
						DepthStencilState.None,
						Main.Rasterizer,
						null,
						segmentTransform
					);

					int j = 1;
					foreach (Item item in segment) {
						Texture2D texture1 = TextureAssets.Item[item.type].Value;
						Main.spriteBatch.Draw(texture1, -new Vector2(texture1.Width / 2, texture1.Height / 2) + new Vector2(0, 16 * j), Color.White);
						j++;
					}

					Main.spriteBatch.End();
				}
				i++;
			}

			Texture2D texture = TextureAssets.Item[assembler.seed.type].Value;

			Main.spriteBatch.Begin(
				SpriteSortMode.Deferred,
				BlendState.AlphaBlend,
				Main.DefaultSamplerState,
				DepthStencilState.None,
				Main.Rasterizer,
				null,
				transform
			);

			Color color = assembler.seed.color;
			if (assembler.seed.color == new Color()) {
				color = Color.White;
			}

			Main.spriteBatch.Draw(texture, -new Vector2(texture.Width / 2, texture.Height / 2), Color.White);

			Main.spriteBatch.End();
		}

		public void DrawGlue() {
			GlueLensPlayer p = Main.LocalPlayer.GetModPlayer<GlueLensPlayer>();

			if (!(p.glueForced || Main.LocalPlayer.HeldItem.ModItem is Adhesive || Main.LocalPlayer.HeldItem.ModItem is MultiAdhesive)) {
				return;
			}

			Texture2D glueTexture = ModContent.Request<Texture2D>("Techarria/Content/Glue").Value;

			_ = Main.gfxQuality;
			Vector2 zero2 = Vector2.Zero;
			if (Main.drawToScreen) {
				zero2 = Vector2.Zero;
			}
			int num12 = (int)((Main.screenPosition.X - zero2.X) / 16f - 1f);
			int num13 = (int)((Main.screenPosition.X + (float)Main.screenWidth + zero2.X) / 16f) + 2;
			int num14 = (int)((Main.screenPosition.Y - zero2.Y) / 16f - 1f);
			int num15 = (int)((Main.screenPosition.Y + (float)Main.screenHeight + zero2.Y) / 16f) + 5;
			if (num12 < 0) {
				num12 = 0;
			}
			if (num13 > Main.maxTilesX) {
				num13 = Main.maxTilesX;
			}
			if (num14 < 0) {
				num14 = 0;
			}
			if (num15 > Main.maxTilesY) {
				num15 = Main.maxTilesY;
			}
			Point screenOverdrawOffset = Main.GetScreenOverdrawOffset();

			Color[] colors = new Color[4] {
				new Color(0.1f, 0.4f, 1),
				new Color(0.2f, 0.7f, 0.7f),
				new Color(1f, 0.4f, 0.6f),
				new Color(1f, 0.7f, 0.2f)
			};
			for (int i = 0; i < 4; i++)
				if (p.modes[i] == 0)
					colors[i] = colors[i].MultiplyRGBA(new Color(0.5f, 0.5f, 0.5f, 0.5f));

			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);

			for (int j = num14 + screenOverdrawOffset.Y; j < num15 - screenOverdrawOffset.Y; j++) {
				for (int i = num12 + screenOverdrawOffset.X; i < num13 - screenOverdrawOffset.X; i++) {

					for (int c = 0; c < 4; c++) {
						if (Main.tile[i, j].Get<Glue>().GetChannel(c)) {
							Main.NewText($"{c}, {i}, {j}");
							Rectangle value = getGlueFraming(i, j, c);
							Main.spriteBatch.Draw(glueTexture, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y), value, Color.Black.MultiplyRGBA(colors[c]));
						}
					}
				}
			}

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);

			for (int j = num14 + screenOverdrawOffset.Y; j < num15 - screenOverdrawOffset.Y; j++) {
				for (int i = num12 + screenOverdrawOffset.X; i < num13 - screenOverdrawOffset.X; i++) {


					for (int c = 0; c < 4; c++) {
						if (Main.tile[i, j].Get<Glue>().GetChannel(c)) {
							Rectangle value = getGlueFraming(i, j, c);
							Main.spriteBatch.Draw(glueTexture, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y), value, colors[c].MultiplyRGB(p.modes[c] <= 1 ? Lighting.GetColor(i, j) : Color.White));
						}
					}
				}
			}
			Main.spriteBatch.End();
		}

		public Vector2 BezierPoint(Vector2 start, Vector2 end, Vector2 startControl, Vector2 endControl, float t) {
			Vector2 A = Vector2.Lerp(start, startControl, t);
			Vector2 B = Vector2.Lerp(startControl, endControl, t);
			Vector2 C = Vector2.Lerp(endControl, end, t);
			Vector2 AB = Vector2.Lerp(A, B, t);
			Vector2 BC = Vector2.Lerp(B, C, t);
			return Vector2.Lerp(AB, BC, t);

		}

		public List<Vector2> BezierCurve(Vector2 start, Vector2 end, Vector2 startControl, Vector2 endControl, int segmentCount = 10, bool relativeControlPostions = true) {
			if (relativeControlPostions) {
				startControl += start;
				endControl += end;
			}
			List<Vector2> points = new List<Vector2>();
			for (int i = 0; i < segmentCount; i++) {
				float t1 = i / (float)segmentCount;
				float t2 = (i + 1) / (float)segmentCount;
				points.Add(BezierPoint(start, end, startControl, endControl, t1));
				points.Add(BezierPoint(start, end, startControl, endControl, t2));
			}

			return points;
		}

		public void DrawCables() {

			if (graphicsDevice == null)
				graphicsDevice = Main.graphics.GraphicsDevice;
			if (basicEffect == null)
				basicEffect = new BasicEffect(graphicsDevice);


			VertexBuffer vertexBuffer;

			List<VertexPositionColor> vertices = new();

			foreach (var (p, te) in TileEntity.ByPosition) {
				if (te is CableConnectorTE connectorTE) {
					if (connectorTE.ID < connectorTE.connectedID && connectorTE.isConnected) {
						Point16 mPos = connectorTE.Position;
						Vector2 mVec = new Vector2(mPos.X * 16, mPos.Y * 16) + Vector2.One * 8;
						if (TileEntity.ByID.TryGetValue(connectorTE.connectedID, out TileEntity temp) && temp is CableConnectorTE connectingTE) {
							Point16 cPos = connectingTE.Position;
							Vector2 cVec = new Vector2(cPos.X * 16, cPos.Y * 16) + Vector2.One * 8;
							List<Vector2> points = BezierCurve(mVec, cVec, new Vector2(16, 16), new Vector2(-16, 16));
							foreach (Vector2 point in points) {
								vertices.Add(new VertexPositionColor(new(point, 0), new Color(218 / 255f, 98 / 255f, 82 / 255f)));
								Dust.NewDust(point - new Vector2(4), 0, 0, ModContent.DustType<TransferDust>());
							}
							Main.NewText(vertices);
						}
					}
				}
			}

			CableConnectorPlayer connectorPlayer = Main.LocalPlayer.GetModPlayer<CableConnectorPlayer>();
			if (connectorPlayer.isConnecting) {
				CableConnectorTE connectingTE = TileEntity.ByID[connectorPlayer.connectingID] as CableConnectorTE;
				Point16 cPos = connectingTE.Position;
				Vector2 cVec = new(cPos.X * 16, cPos.Y * 16);
				vertices.Add(new VertexPositionColor(new(Main.MouseWorld, 0), new Color(218 / 255f, 98 / 255f, 82 / 255f)));
				vertices.Add(new VertexPositionColor(new(cVec + Vector2.One * 8, 0), new Color(218 / 255f, 98 / 255f, 82 / 255f)));
			}

			vertexBuffer = new VertexBuffer(graphicsDevice, typeof(VertexPositionColor), 5, BufferUsage.WriteOnly);
			vertexBuffer.SetData(vertices.ToArray());

			basicEffect.World = Matrix.CreateTranslation(-new Vector3(Main.screenPosition.X, Main.screenPosition.Y, 0));
			basicEffect.View = Main.GameViewMatrix.TransformationMatrix;
			basicEffect.Projection = Matrix.CreateOrthographicOffCenter(0, Main.instance.GraphicsDevice.Viewport.Width, Main.instance.GraphicsDevice.Viewport.Height, 0, -1, 1);
			basicEffect.VertexColorEnabled = true;

			graphicsDevice.SetVertexBuffer(vertexBuffer);

			RasterizerState rasterizerState = new RasterizerState();
			rasterizerState.CullMode = CullMode.None;
			graphicsDevice.RasterizerState = rasterizerState;
			graphicsDevice.BlendState = BlendState.AlphaBlend;

			foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes) {
				pass.Apply();
				graphicsDevice.DrawPrimitives(PrimitiveType.LineStrip, 0, vertices.Count);
			}
		}

		public void DrawDrones() {
			foreach (Drone drone in Drone.drones) {
				drone.Draw();
			}
		}
		public void DrawChargingRack(ChargingRackTE storage, Point16 p) {

			Texture2D texture = TextureAssets.Item[storage.item.type].Value;

			float scale = 16f / Math.Max(texture.Width, texture.Height);


			Matrix offset = Matrix.Identity;
			offset.Translation = new Vector3(p.X * 16 + 24 - Main.screenPosition.X, p.Y * 16 + 16 - Main.screenPosition.Y, 0);
			Matrix transform = Main.Transform;
			transform = offset * transform;

			transform = Matrix.CreateScale(scale) * transform;

			Main.spriteBatch.Begin(
				SpriteSortMode.Deferred,
				BlendState.AlphaBlend,
				Main.DefaultSamplerState,
				DepthStencilState.None,
				Main.Rasterizer,
				null,
				transform
			);

			Color color = storage.item.color;
			if (storage.item.color == new Color()) {
				color = Color.White;
			}

			Main.spriteBatch.Draw(texture, -new Vector2(texture.Width / 2, texture.Height), color);

			Main.spriteBatch.End();
		}

		public override void PostDrawTiles() {
			foreach (var (point, te) in TileEntity.ByPosition) {
				if (te is JourneyCrateTE JourneyStorage) {
					DrawJourneyCrate(JourneyStorage, point);
				}
				else if (te is StorageCrateTE storage) {
					DrawStorageCrate(storage, point);
				}
				else if (te is RotaryAssemblerTE assembler) {
					DrawRotaryAssembler(assembler, point);
				}
				else if (te is ChargingRackTE chargingRack) {
					DrawChargingRack(chargingRack, point);
				}
			}

			DrawGlue();

			DrawCables();

			DrawDrones();


			if (texture == null) {
				texture = new(Main.graphics.GraphicsDevice, 320, 16);
			}

			foreach (var (modtile, effect) in BeamDrill.effects) {
				Vector2 TileOffset = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange) - Main.screenPosition;
				Main.spriteBatch.Begin(
					SpriteSortMode.Immediate,
					BlendState.AlphaBlend,
					Main.DefaultSamplerState,
					DepthStencilState.None,
					Main.Rasterizer,
					effect,
					Matrix.CreateTranslation(TileOffset.X, TileOffset.Y, 0) * Main.Transform
				);

				effect.Parameters["myParam"].SetValue(Main.GlobalTimeWrappedHourly);
				effect.Parameters["uImageSize0"].SetValue(new Vector2(320, 16));
				effect.Parameters["waveSize"].SetValue(-100);
				effect.Parameters["waveFrequency"].SetValue(20);
				effect.Parameters["pixelate"].SetValue(true);
				effect.Parameters["pixelScale"].SetValue(2);

				foreach (var (p, te) in TileEntity.ByPosition) {
					if (te is BeamDrillTE drill && drill.modtile == modtile) {

						if (drill.range > 0.5f) {
							effect.Parameters["thickness"].SetValue(8);
							effect.Parameters["beamLength"].SetValue(drill.range * 16 - 8);
							effect.Parameters["indent"].SetValue(1.5f);
							effect.Parameters["waveAmplitude"].SetValue(0.75f);
						}
						else {
							effect.Parameters["thickness"].SetValue(drill.range * 16);
							effect.Parameters["beamLength"].SetValue(0);
							effect.Parameters["indent"].SetValue(0);
							effect.Parameters["waveAmplitude"].SetValue(0);
						}
						Main.spriteBatch.Draw(texture, new Vector2(drill.Position.X * 16 - 200 + 16, drill.Position.Y * 16 - 200 + 16), null, Color.White, drill.dir * MathHelper.PiOver2, new Vector2(0, 8), 1f, SpriteEffects.None, 0);
					}
				}

				Main.spriteBatch.End();
			}
		}
    }
}
