using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace Techarria.Common.Configs
{
    public class TecharriaServerConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;

        [Header("Dev Items")]
        // do not newline these
        [Label("Enable Dev items")]
        [Tooltip("Highly experimental/unbalanced, use at your own risk\n" +
            "Affected items:\n" +
            "Update Hammer (functional, unnobtainable)\n" +
            "Infinity Crate (functional, obtainable)\n" +
            "Power Block (functional, unnobtainable)")]
        [DefaultValue(false)]
        [ReloadRequired]
        public bool DevItemsEnabled;
    }
    /*
     * client side (unused)
     * 
    public class TecharriaClientConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;
    }
    */
}
