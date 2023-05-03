using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Techarria.Content.Dusts;
using Techarria.Content.Items.FluidItems;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;

namespace Techarria.Content.Tiles.FluidTransfer
{
	public class FluidTankTE : ModTileEntity {
		private Item _fluid = new Item();
		public Item fluid {
			get {
				return getOriginEntity()._fluid;
			}
			set {
				getOriginEntity()._fluid = value;
			}
		}
		public Point? multiblockOrigin = null;
		public int multiblockSize = 1;
		public int multiblockIndexY = 0;

		public int maxCapacity { get => getOriginEntity().multiblockSize * FluidTank.maxStorage; }

        public override bool IsTileValidForEntity(int x, int y)
        {
            return Main.tile[x, y].TileType == ModContent.TileType<FluidTank>();
        }

		public FluidTankTE getOriginEntity() {
			if (multiblockOrigin is Point p && multiblockOrigin!=Position.ToPoint()) {
				if (ByPosition.TryGetValue(new(p.X, p.Y), out TileEntity te))
					return te as FluidTankTE;
			}
			multiblockOrigin = Position.ToPoint();
			return this;
		}

		public override void Update() {
			Point offset = new Point();
			if (TileEntity.ByPosition.TryGetValue(Position - new Point16(0, 2), out TileEntity te)) {
				multiblockOrigin = (te as FluidTankTE).multiblockOrigin;
				multiblockIndexY = (te as FluidTankTE).multiblockIndexY+1;
				multiblockSize = (te as FluidTankTE).multiblockSize;
				offset.Y = 36;
			} else {
				multiblockOrigin = Position.ToPoint();
				multiblockIndexY = 0;
			}
			if (TileEntity.ByPosition.TryGetValue(Position + new Point16(0, 2), out te)) {
				offset.X = 36;
			}

			if (multiblockOrigin != Position.ToPoint()) {
				if (_fluid.type == fluid.type) {
					Main.NewText(_fluid.stack);
					getOriginEntity()._fluid.stack += _fluid.stack;
				}
				_fluid.TurnToAir();
			}

			for (int i = 0; i < 2; i++) {
				for (int j = 0; j < 2; j++) {
					Tile tile = Main.tile[Position.ToPoint() + new Point(i, j)];
					tile.TileFrameX = (short)(18 * i + offset.X);
					tile.TileFrameY = (short)(18 * j + offset.Y);
				}
			}

			if (multiblockOrigin == Position.ToPoint()) {
				multiblockSize = 1;
				while (TileEntity.ByPosition.TryGetValue(Position + new Point16(0, 2 * multiblockSize), out te)) {
					multiblockSize++;
				}
			}

			Dust.NewDust(new Vector2(Position.X, Position.Y) * 16, 0, 0, ModContent.DustType<TransferDust>());
		}

		public override void SaveData(TagCompound tag)
        {
            tag.Add("fluid", _fluid);
            base.SaveData(tag);
        }

        public override void LoadData(TagCompound tag)
        {
            _fluid = tag.Get<Item>("fluid");
            base.LoadData(tag);
        }
    }
    public class FluidTank : ModTile
    {
        public static int maxStorage = 2550;
        public override void SetStaticDefaults()
        {
            // Spelunker
            Main.tileSpelunker[Type] = true;

            // Properties
            Main.tileLavaDeath[Type] = false;
            Main.tileFrameImportant[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;
            TileID.Sets.IgnoredByNpcStepUp[Type] = true;
            TileID.Sets.PreventsTileRemovalIfOnTopOfIt[Type] = true;

            Main.tileFrameImportant[Type] = true;
            Main.tileSolidTop[Type] = true;
            Main.tileTable[Type] = true;

            // placement
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.Origin = new Point16(0, 1);
            TileObjectData.newTile.CoordinateHeights = new[] { 16, 16 };
            TileObjectData.newTile.AnchorInvalidTiles = new int[] { TileID.MagicalIceBlock };
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide | AnchorType.Table, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.addTile(Type);
        }
        public virtual FluidTankTE GetTileEntity(int i, int j)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            i -= tile.TileFrameX / 18 % 2;
            j -= tile.TileFrameY / 18 % 2;
            TileEntity.ByPosition.TryGetValue(new Point16(i, j), out TileEntity te);
            return te as FluidTankTE;
        }
        public override void PlaceInWorld(int i, int j, Item item)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            i -= tile.TileFrameX / 18 % 2;
            j -= tile.TileFrameY / 18 % 2;
			ModContent.GetInstance<FluidTankTE>().Place(i, j);
		}

		public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            FluidTankTE tileEntity = GetTileEntity(i, j);
            int itemIndex = Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 32, ModContent.ItemType<FluidBottle>(), tileEntity.fluid.stack);
            Item spawnedFluidBottle = Main.item[itemIndex];
            FluidBottle fluidBottle = spawnedFluidBottle.ModItem as FluidBottle;
            fluidBottle.storedItem = tileEntity.fluid.type;

            ModContent.GetInstance<FluidTankTE>().Kill(i, j);
        }

        public override bool RightClick(int i, int j)
        {
            FluidTankTE tileEntity = GetTileEntity(i, j);
            Item fluid = tileEntity.fluid;
            Item playerItem;

            if (!Main.mouseItem.IsAir)
            {
                playerItem = Main.mouseItem;
            }
            else
            {
                playerItem = Main.player[Main.myPlayer].HeldItem;
            }

			if (fluid.IsAir) {
				if (playerItem.ModItem is FluidItem) {
					fluid = playerItem.Clone();
					fluid.stack = playerItem.stack;
					playerItem.stack -= playerItem.stack;
					// overflow prevention
					if (fluid.stack > tileEntity.maxCapacity) {
						int overflow = fluid.stack - tileEntity.maxCapacity;
						playerItem.stack += overflow;
						fluid.stack = tileEntity.maxCapacity;
					}

					if (playerItem.stack <= 0) {
						playerItem.TurnToAir();
					}
					return true;
				}
				if (playerItem.ModItem is FluidBottle bottle) {
					fluid = new Item(bottle.storedItem, playerItem.stack);
					playerItem.stack = 0;
					if (fluid.stack > tileEntity.maxCapacity) {
						int overflow = fluid.stack - tileEntity.maxCapacity;
						playerItem.stack += overflow;
						fluid.stack = tileEntity.maxCapacity;
					}
					return true;
				}
			}

			if (!fluid.IsAir && playerItem.type == fluid.type && fluid.stack < tileEntity.maxCapacity)
            {
                fluid.stack += playerItem.stack;
                playerItem.stack -= playerItem.stack;
                // overflow prevention
                if (fluid.stack > tileEntity.maxCapacity)
                {
                    int overflow = fluid.stack - tileEntity.maxCapacity;
                    playerItem.stack += overflow;
                    fluid.stack = tileEntity.maxCapacity;
                }

                if (playerItem.stack <= 0)
                {
                    playerItem.TurnToAir();
                }
                return true;
            }
            return false;
        }


        public override void MouseOver(int i, int j)
        {
            FluidTankTE tileEntity = GetTileEntity(i, j);
            Item fluid = tileEntity.fluid;
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            if ((fluid != null) && (!fluid.IsAir))
            {
                player.cursorItemIconEnabled = true;
                player.cursorItemIconText = fluid.stack.ToString("#,# L /") + tileEntity.maxCapacity.ToString(" #,# L");
                player.cursorItemIconID = fluid.type;
			}
			if (tileEntity.multiblockOrigin is Point p)
			Dust.NewDust(p.ToVector2() * 16, 0, 0, ModContent.DustType<Indicator>());
			Main.NewText(tileEntity.getOriginEntity().multiblockSize - tileEntity.multiblockIndexY - 1);
		}

		public override void PostDraw(int i, int j, SpriteBatch spriteBatch) {
			FluidTankTE tileEntity = GetTileEntity(i, j);
			Item fluid = tileEntity.fluid;
			Point16 subTile = new Point16(i, j) - tileEntity.Position;
			if (subTile != new Point16(1, 1)) {
				return;
			}
			if (tileEntity.multiblockIndexY + 1 != tileEntity.multiblockSize) {
				return;
			}

			Texture2D texture = TextureAssets.Item[tileEntity.fluid.type].Value;

			Rectangle sourceRect = new Rectangle(0, 0, texture.Width, texture.Height);
			if (Main.itemAnimationsRegistered.Contains(tileEntity.fluid.type)) {
				sourceRect = Main.itemAnimations[tileEntity.fluid.type].GetFrame(texture);
			}

			float fillRatio = (float)tileEntity.fluid.stack / FluidTank.maxStorage;

			Color color = tileEntity.fluid.color;
			if (tileEntity.fluid.color == new Color()) {
				color = Color.White;
			}

			Vector2 TileOffset = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
			Vector2 pos = new Vector2(i, j) * 16 - Main.screenPosition + TileOffset;

			Vector2 offset = -new Vector2(8, (int)(fillRatio * 32 - 16));
			sourceRect.Height -= Math.Min((int)(16 - fillRatio * 32), 16);

			Main.spriteBatch.Draw(texture, offset + pos, sourceRect, color);

			if (fillRatio > 0.5) {

				sourceRect.Y += 14;
				sourceRect.Height = 1;
				offset.Y += 16;
				Main.spriteBatch.Draw(texture, offset + pos, sourceRect, color, 0, Vector2.Zero, new Vector2(1, (int)(fillRatio * 32 - 16)), SpriteEffects.None, 0);
			}

			return;
		}
	}
}
