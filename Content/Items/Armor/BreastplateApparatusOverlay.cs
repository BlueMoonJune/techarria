using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Techarria.Content.Items.Armor.Apparatus;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Techarria.Content.Items.Armor
{
    internal class BreastplateApparatusOverlay : ApparatusDrawLayer
    {
        public override void SetData()
        {
            texture = ModContent.Request<Texture2D>("Techarria/Content/Items/Armor/Apparatus/BreastplateApparatus_Body_Overlay").Value;
            itemType = ModContent.ItemType<BreastplateApparatus>();
        }

        public override Position GetDefaultPosition()
        {
            System.Console.WriteLine("Techarria: GetDefaultPosition");
            return new AfterParent(PlayerDrawLayers.ArmOverItem);
        }
    }
}
