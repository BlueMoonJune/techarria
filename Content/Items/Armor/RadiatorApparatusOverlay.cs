using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Techarria.Content.Items.Armor.Apparatus;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Techarria.Content.Items.Armor
{
    public class RadiatorApparatusOverlay : ApparatusDrawLayer
    {
        public override void SetData()
        {
            texture = ModContent.Request<Texture2D>("Techarria/Content/Items/Armor/Apparatus/RadiatorApparatus_Head_Overlay").Value;
            itemType = ModContent.ItemType<RadiatorApparatus>();
        }

        public override Position GetDefaultPosition()
        {
            System.Console.WriteLine("Techarria: GetDefaultPosition");
            return new AfterParent(PlayerDrawLayers.Head);
        }
    }
}
