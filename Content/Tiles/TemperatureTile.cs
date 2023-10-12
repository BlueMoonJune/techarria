using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Techarria.Content.Items.RecipeItems;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Techarria.Content.Tiles
{
    public interface ITemperatureTE
    {
        public static Dictionary<ITemperatureTE, float> teTemps = new();

        public float Temp
        {
            get
            {
                if (teTemps.ContainsKey(this))
                {
                    return teTemps[this];
                }
                teTemps.Add(this, 0);
                return 0;
            }
        }
    }

    public interface ITemperatureTile
    {
        public float GetTemp(int i, int j);
    }

    public class TemperatureTile<T> : EntityTile<T>, ITemperatureTile where T : ModTileEntity, ITemperatureTE
    {
        public float GetTemp(int i, int j)
        {
            return GetTileEntity(i, j).Temp;
        }
    }
}
