using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using Techarria.Structures;
using Techarria.Content.NPCs;

namespace Techarria.Content.Items.Tools
{
	public class UpdateHammer : ModItem
	{
        public override bool IsLoadingEnabled(Mod mod)
        {
			return ModContent.GetInstance<Common.Configs.TecharriaServerConfig>().DevItemsEnabled;
		}
        public override void SetStaticDefaults()
		{
			//Tooltip.SetDefault("Use this to update the textures of blocks");

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.width = 52;
			Item.height = 52;
			Item.useTime = 1;
			Item.useAnimation = 5;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.value = 10000;
			Item.rare = ItemRarityID.Blue;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true; // Automatically re-swing/re-use this item after its swinging animation is over.
		}

        public override bool? UseItem(Player player)
        {
			ModContent.GetInstance<Tiles.Machines.PlayerInterface>().SetStaticDefaults();
			if (player.whoAmI == Main.myPlayer)
			{
				bool idc = false;
				Point pos = Main.MouseWorld.ToTileCoordinates();
				Tile t = Main.tile[pos];

				Main.tileSolid[t.TileType] = true;
				Main.NewText(Main.tileSolid[t.TileType]);
				ModTile tile = TileLoader.GetTile(Main.tile[pos.X, pos.Y].TileType);
				if (tile != null) {
					tile.TileFrame(pos.X, pos.Y, ref idc, ref idc);
					return true;
				}
			}
			return false;
		}
    }
}
