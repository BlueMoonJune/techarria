using System;
using Techarria.Content.Items.FluidItems;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;

namespace Techarria.Content.Tiles.FluidTransfer
{
    public class FluidTankTE : ModTileEntity
    {
        public Item fluid = new Item();
        public override bool IsTileValidForEntity(int x, int y)
        {
            return Main.tile[x, y].TileType == ModContent.TileType<FluidTank>();
        }

        public override void SaveData(TagCompound tag)
        {
            tag.Add("fluid", fluid);
            base.SaveData(tag);
        }

        public override void LoadData(TagCompound tag)
        {
            fluid = tag.Get<Item>("fluid");
            base.LoadData(tag);
        }
    }
    public class FluidTank : ModTile
    {
        public static int maxStorage = 100_000;
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
            TileObjectData.newTile.CoordinateHeights = new[] { 16, 18 };
            TileObjectData.newTile.AnchorInvalidTiles = new int[] { TileID.MagicalIceBlock };
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
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

            if (fluid.IsAir && ModContent.GetModItem(playerItem.type) is FluidItem)
            {
                fluid = playerItem.Clone();
                fluid.stack = playerItem.stack;
                tileEntity.fluid = fluid;
                playerItem.stack -= playerItem.stack;
                // overflow prevention
                if (fluid.stack > maxStorage)
                {
                    int overflow = fluid.stack - maxStorage;
                    playerItem.stack += overflow;
                    fluid.stack -= overflow;
                }

                if (playerItem.stack <= 0)
                {
                    playerItem.TurnToAir();
                }
                return true;
            }
            if (!fluid.IsAir && playerItem.type == fluid.type && fluid.stack < maxStorage)
            {
                fluid.stack += playerItem.stack;
                playerItem.stack -= playerItem.stack;
                // overflow prevention
                if (fluid.stack > maxStorage)
                {
                    int overflow = fluid.stack - maxStorage;
                    playerItem.stack += overflow;
                    fluid.stack -= overflow;
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
                player.cursorItemIconText = fluid.stack.ToString("#,# mb /") + maxStorage.ToString(" #,# mb");
                player.cursorItemIconID = fluid.type;
            }
        }
    }
}
