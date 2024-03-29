﻿using MagicStorage.UI;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Techarria.Content.Dusts;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;

using TE = Techarria.Content.Tiles.Machines.GelatinousTurbineTE;

namespace Techarria.Content.Tiles.Machines
{
    public class GelatinousTurbineTE : InventoryTileEntity
	{
		public Item item = new();
		public int burnTime = 0;
		public float pulseFraction = 0;
		public int frame = 0;
		public static Rectangle particleRect = new(4, 22, 16, 6);

        public override Item[] ExtractableItems => new Item[] {item};

        public static Dictionary<int, int> fuelItems = new Dictionary<int, int>()
		{
			{ItemID.Gel, 600}, //10 seconds
            {ItemID.PinkGel, 3600}, //1 minute
            {ItemID.RoyalGel, 216000}, // 1 hour
			{ItemID.VolatileGelatin, 438000} // 2 hours
        };

        public override bool InsertItem(Item item)
        {
            if (this.item.IsAir)
            {
                this.item = item.Clone();
				this.item.stack = 1;
            }
            if (this.item.type != item.type) return false;
            if (this.item.stack >= this.item.maxStack) return false;
            this.item.stack++;
            return true;
        }

        public override void Update() {


			if (burnTime > 0) {
				frame = burnTime / 15 % 4;
				for (int i = 0; i < 3; i++) {
					for (int j = 0; j < 2; j++) {
						Tile tile = Main.tile[Position.X + i, Position.Y + j];
						tile.TileFrameX = (short)(54 + 18 * i);
						tile.TileFrameY = (short)(frame * 36 + j * 18);
					}
				}
			}

			if (burnTime == 0) {
				for (int i = 0; i < 3; i++) {
					for (int j = 0; j < 2; j++) {
						Tile tile = Main.tile[Position.X + i, Position.Y + j];
						tile.TileFrameX = (short)(i * 18);
						tile.TileFrameY = (short)(j * 18);
					}
				}
			}
			else {

				if (new Random().Next(10) == 0) {
					var dust = Dust.NewDustDirect(new Vector2(Position.X, Position.Y) * 16 + particleRect.TopLeft(), particleRect.Width, particleRect.Height, DustID.Flare);
					//dust.velocity = new Vector2(0, -1);
					//dust.alpha = 127;
				}
				if (new Random().Next(3) == 0) {
					var dust = Dust.NewDustDirect(new Vector2(Position.X, Position.Y) * 16 + particleRect.TopLeft(), particleRect.Width, particleRect.Height, DustID.Smoke);
					dust.velocity = new Vector2(0, -1);
					dust.alpha = 127;
				}
			}

			Dust.NewDust(new Vector2(Position.X, Position.Y) * 16, 0, 0, ModContent.DustType<TransferDust>());
			if (item == null) {
				item = new Item();
			}

            if (burnTime <= 0)
            {
                if (fuelItems.Keys.Contains(item.type))
                {
                    burnTime += fuelItems[item.type];
                    item.stack--;
                    if (item.stack <= 0)
                    {
                        item.TurnToAir();
                    }
                }
            }

            if (burnTime > 0) {
				for (int i = 0; i < (int)Techarria.GenerationMultiplier; i++) {
					Wiring.TripWire(Position.X, Position.Y, 3, 2);
					Power.TransferCharge(3, Position.X, Position.Y, 3, 2);
				}
				pulseFraction += (3 * Techarria.GenerationMultiplier) % 1f;
				if (pulseFraction >= 1) {
					pulseFraction -= 1;
					Power.TransferCharge(1, Position.X, Position.Y, 3, 2);
				}
				burnTime--;
			}
			
		}

		public override void SaveData(TagCompound tag) {
			tag.Add("item", item);
			tag.Add("burnTime", burnTime);
			base.SaveData(tag);
		}

		public override void LoadData(TagCompound tag) {
			item = tag.Get<Item>("item");
			burnTime = tag.GetInt("burnTime");
			base.LoadData(tag);
		}
	}

	// Where the TE ends and the Tile starts
	public class GelatinousTurbine : EntityTile<TE>
	{
		public override void PreStaticDefaults() {
			Main.tileLavaDeath[Type] = false;
			Main.tileNoAttach[Type] = true;
			Main.tileFrameImportant[Type] = true;
			TileID.Sets.DisableSmartCursor[Type] = true;

			DustType = ModContent.DustType<Spikesteel>();

			width = 3;
			height = 2;

			AddMapEntry(new Color(200, 200, 200), Language.GetText("Gelatinous Turbine"));
		}

		public override bool RightClick(int i, int j) {
			TE tileEntity = GetTileEntity(i, j);
			Item item = tileEntity.item;
			Item playerItem;
			if (!Main.mouseItem.IsAir) {
				playerItem = Main.mouseItem;
			}
			else {
				playerItem = Main.player[Main.myPlayer].HeldItem;
			}

			if (item.IsAir && TE.fuelItems.ContainsKey(item.type)) {
				item = playerItem.Clone();
				item.stack = 1;
				tileEntity.item = item;
				playerItem.stack--;
				if (playerItem.stack <= 0) {
					playerItem.TurnToAir();
				}
				return true;
			}
			if (!item.IsAir && playerItem.type == item.type && item.stack < item.maxStack) {
				item.stack++;
				playerItem.stack--;
				if (playerItem.stack <= 0) {
					playerItem.TurnToAir();
				}
				return true;
			}
			if (!item.IsAir) {
				item.stack--;
				Item.NewItem(new EntitySource_TileInteraction(Main.player[Main.myPlayer], i, j), i * 16, j * 16, 32, 32, item.type);
				if (item.stack <= 0) {
					item.TurnToAir();
				}
				return true;
			}
			return false;
		}
		public override void MouseOver(int i, int j) {
            TE tileEntity = GetTileEntity(i, j);
			Item item = tileEntity.item;
			Player player = Main.LocalPlayer;
			player.noThrow = 2;
			if (item != null && !item.IsAir) {
				player.cursorItemIconEnabled = true;
				player.cursorItemIconText = "" + item.stack;
				player.cursorItemIconID = item.type;
			}
		}
	}
}
