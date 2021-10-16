using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PostML.Melee
{
    public class NanoAxe2 : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("CREATIVE PICKAXE OF RECOLOURED DOOOOOOOM!!!!");
            Tooltip.SetDefault("Developer/builder tool.");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 555;
            Item.DamageType = DamageClass.Melee;
            Item.width = 52;
            Item.height = 58;
            Item.useTime = 2;
            Item.useAnimation = 8;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.pick = 5000;
            Item.knockBack = 4;
            Item.value = Item.sellPrice(0, 0, 0, 0);
            Item.rare = ItemRarityID.Purple;
            Item.UseSound = SoundID.Item15;
            Item.tileBoost += 6;
            Item.autoReuse = true;
            if (!Main.dedServ)
            {
                Item.GetGlobalItem<ItemUseGlow>().glowTexture = ModContent.Request<Texture2D>(Item.ModItem.Texture + "_Glow").Value;
            }
        }
    }
}