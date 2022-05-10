using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Enums;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Redemption.Dusts;
using Redemption.Items.Placeable.Furniture.Shade;
using Redemption.Dusts.Tiles;

namespace Redemption.Tiles.Furniture.Shade
{
    public class ShadestonePianoTile : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
			TileObjectData.newTile.Origin = new Point16(1, 1);
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.addTile(Type);

			ModTranslation name = CreateMapEntryName();
			name.SetDefault("Shadestone Piano");
			AddMapEntry(new Color(59, 61, 87), name);

			AdjTiles = new int[] { TileID.Pianos };
			DustType = ModContent.DustType<ShadestoneDust>();
		}
		public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
		public override void KillMultiTile(int i, int j, int frameX, int frameY) => Item.NewItem(new EntitySource_TileBreak(i, j), new Vector2(i, j) * 16f, ModContent.ItemType<ShadestonePiano>());
	}
}
