using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Techarria.Content.Tiles.Machines;

namespace Techarria.Content.Items.Materials.Molten
{
	internal class AmalurityBlob : MoltenBlob
	{
		public override void SetDefaults() {
			temp = 2000;
			Item.width = 16;
			Item.height = 16;
			Item.maxStack = 999;
		}

		public static List<int> evilBlocks = new List<int>() { 23, 199, 25, 203, 112, 234, 163, 200, 400, 401, 398, 399, 661, 662 };
		public static List<int> pureBlocks = new List<int>() { 2, 1, 53, 161, 396, 397, 60 };
		public static List<int> hallowedBlocks = new List<int>() { TileID.HallowedGrass, TileID.HallowedIce, TileID.HallowHardenedSand, TileID.HallowSandstone, TileID.Pearlsand, TileID.Pearlstone };

		public override void Update(ref float gravity, ref float maxFallSpeed) {
			int amount = 60;
			if (Item.velocity.Y == 0) {
				Tile tile = Main.tile[(Item.Bottom / 16 + new Vector2(0, 0.5f)).ToPoint()];
				if (evilBlocks.Contains(tile.TileType)) {
					for (int i = 0; i < amount; i++) {
						Projectile.NewProjectile(new EntitySource_DropAsItem(Item), Item.Center, new Vector2(4 + Main.rand.NextFloat() * 2, 0).RotatedBy(i / (float)amount * MathHelper.TwoPi), ProjectileID.PureSpray, 0, 0);
					}
					Item.TurnToAir();
					return;
				}
				if (pureBlocks.Contains(tile.TileType)) {
					for (int i = 0; i < amount; i++) {
						Projectile.NewProjectile(new EntitySource_DropAsItem(Item), Item.Center, new Vector2(4 + Main.rand.NextFloat() * 2, 0).RotatedBy(i / (float)amount * MathHelper.TwoPi), ProjectileID.HallowSpray, 0, 0);
					}
					Item.TurnToAir();
					return;
				}
				if (hallowedBlocks.Contains(tile.TileType)) {
					int sprayType = WorldGen.crimson ? ProjectileID.CrimsonSpray : ProjectileID.CorruptSpray;
					for (int i = 0; i < amount; i++) {
						Projectile.NewProjectile(new EntitySource_DropAsItem(Item), Item.Center, new Vector2(4+Main.rand.NextFloat()*2, 0).RotatedBy(i / (float)amount * MathHelper.TwoPi), sprayType, 0, 0);
					}
					Item.TurnToAir();
					return;
				}
				int[] sprayTypes = new int[3] { ProjectileID.PureSpray, ProjectileID.HallowSpray, WorldGen.crimson ? ProjectileID.CrimsonSpray : ProjectileID.CorruptSpray };
				for (int i = 0; i < amount; i++) {
					Projectile.NewProjectile(new EntitySource_DropAsItem(Item), Item.Center, new Vector2(4 + Main.rand.NextFloat() * 2, 0).RotatedBy(i / (float)amount * MathHelper.TwoPi), Main.rand.Next(sprayTypes), 0, 0);
				}
				Item.TurnToAir();
			}
		}

		public override void AddRecipes() {
			new ArcFurnaceRecipe(new Item(Type), 1000, new() {
				new(new Item(ItemID.HallowedBar), 3),
				new(new Item(ItemID.SoulofLight), 3),
			}).Register();
		}
	}
}
