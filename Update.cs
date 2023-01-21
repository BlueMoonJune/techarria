﻿using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.ObjectData;
using Techarria.Content.Dusts;
using Terraria.Audio;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using System;
using Terraria.DataStructures;
using Techarria.Content.Tiles;
using System.Collections.Generic;
using Techarria.Content.Items.Materials.Molten;

namespace Techarria
{
    internal class Update : ModSystem
    {
        public override void PostUpdateEverything()
        {
            bool logged = false;
            foreach (Chest chest in Main.chest)
            {
                if (chest != null)
                {
                    bool hasMolten = false;
                    foreach (Item item in chest.item)
                    {
                        if (item.ModItem is MoltenBlob)
                        {
                            hasMolten = true; 
                            break;
                        }
                    }
                    if (hasMolten)
                    {
                        for (int i = 0; i < chest.item.Length; i++)
                        {
                            Item item = chest.item[i];
                            if (!logged)
                            {
                                Main.NewText($"{chest.x} {chest.y} {item.Name}");
                                logged = true;
                            }
                            //if (!item.IsAir)
                            //{
                            Item.NewItem(new EntitySource_TileBreak(chest.x, chest.y), new Rectangle(chest.x * 16, chest.y * 16, 32, 32), item);
                            chest.item[i] = null;
                        }
                        WorldGen.KillTile(chest.x, chest.y, noItem: true);
                    }
                }
            }
        }
    }
}
