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

namespace Techarria
{
	internal class Rendering : ModSystem
    {
        /*
        public override void PostDrawTiles()
        {
            SpriteBatch spriteBatch = Main.spriteBatch;


            Matrix matrix = Main.Transform;

            matrix = Matrix.CreateRotationZ((float)(System.DateTime.Now.Millisecond / 500f * Math.PI)) * matrix;


            Vector2 translation = Main.LocalPlayer.Center - Main.screenPosition;
            matrix.Translation = new Vector3 (translation.X, translation.Y, 0);

            spriteBatch.Begin(
                SpriteSortMode.Immediate, 
                BlendState.AlphaBlend, 
                SamplerState.PointClamp, 
                DepthStencilState.Default, 
                RasterizerState.CullNone, 
                null, 
                matrix
            );

            Tile tile = Main.tile[100, 100];
            if (tile.HasTile)
                spriteBatch.Draw(TextureAssets.Tile[tile.TileType].Value, new Vector2(-8, -64), new Rectangle(tile.TileFrameX, tile.TileFrameY + tile.TileFrameNumber, 16, 16), Color.White);

            spriteBatch.End();
        }
        */

        public override void PostDrawTiles()
        {
            foreach (var (p, te) in TileEntity.ByPosition)
            {
                if ( te is JourneyCrateTE JourneyStorage)
                {
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
                    if (JourneyStorage.item.color == new Color())
                    {
                        color = Color.White;
                    }

                    Main.spriteBatch.Draw(texture, -new Vector2(texture.Width / 2, texture.Height / 2), color);

                    Main.spriteBatch.End();
                } 
				else if (te is StorageCrateTE storage)
                {
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
                    if (storage.item.color == new Color())
                    {
                        color = Color.White;
                    }

                    Main.spriteBatch.Draw(texture, -new Vector2(texture.Width / 2, texture.Height / 2), color);

                    Main.spriteBatch.End();
                }
				else if (te is RotaryAssemblerTE assembler) 
				{

					Matrix offset = Matrix.Identity;
					offset.Translation = new Vector3(p.X * 16 + 24 - Main.screenPosition.X, p.Y * 16 + 24 - Main.screenPosition.Y, 0);
					Matrix transform = Main.Transform;
					transform = offset * transform;
					transform = Matrix.CreateRotationZ(assembler.degrees / 180f * MathF.PI) * transform;

					transform = Matrix.CreateScale(0.5f) * transform;


					int i = 0;
					foreach (List<Item> segment in assembler.items) {
						if (segment != null) {
							Matrix segmentTransform = Matrix.CreateRotationZ(i * MathHelper.PiOver4) * transform;

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
								Main.spriteBatch.Draw(texture1, -new Vector2(texture1.Width / 2, texture1.Height / 2) + new Vector2(16 * j, 0), Color.White);
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
			}
        }
    }
}
