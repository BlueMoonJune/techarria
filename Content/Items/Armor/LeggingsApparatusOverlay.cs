using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Techarria.Content.Items.Armor.Apparatus;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Techarria.Content.Items.Armor
{
    public class LeggingsApparatusOverlay : ApparatusDrawLayer
    {
        public override void SetData()
        {
            texture = ModContent.Request<Texture2D>("Techarria/Content/Items/Armor/Apparatus/LeggingsApparatus_Legs_Overlay").Value;
            itemType = ModContent.ItemType<LeggingsApparatus>();
        }

        public override Position GetDefaultPosition()
        {
            System.Console.WriteLine("Techarria: GetDefaultPosition");
            return new AfterParent(PlayerDrawLayers.Leggings);
        }
    }
}
