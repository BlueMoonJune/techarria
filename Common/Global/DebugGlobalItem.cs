using Microsoft.Xna.Framework;
using Techarria.Content.Items.Armor.Apparatus;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;


namespace techarria.Common.Global
{
	internal class DebugGlobalProjectile : GlobalProjectile
	{
		public override bool AppliesToEntity(Projectile entity, bool lateInstantiation) {
			return true;
		}

		public override void OnSpawn(Projectile projectile, IEntitySource source) {
			Main.NewText($"Created Projectile with type {projectile.type}");
		}
	}
}
