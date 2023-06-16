using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace Redemption.Tiles.Tiles
{
    public class LabTubeTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = false;
            Main.tileLighted[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;
            DustType = DustID.Glass;
            MinPick = 1000;
            MineResist = 3f;
            HitSound = SoundID.Tink;
            AddMapEntry(new Color(54, 193, 59));
        }
        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.0f;
            g = 0.2f;
            b = 0.0f;
        }
        public override bool CanExplode(int i, int j) => false;
    }
    public class LabTubeTile2 : LabTubeTile
    {
        public override string Texture => "Redemption/Tiles/Tiles/LabTubeTile";
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            TileID.Sets.DisableSmartCursor[Type] = false;
            MinPick = 200;
        }
    }
}