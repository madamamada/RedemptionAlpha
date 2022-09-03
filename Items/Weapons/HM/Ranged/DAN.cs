using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.BaseExtension;
using Redemption.Items.Materials.HM;
using Redemption.Projectiles.Ranged;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace Redemption.Items.Weapons.HM.Ranged
{
    public class DAN : ModItem
	{
		public override void SetStaticDefaults()
		{
            DisplayName.SetDefault("D.A.N");
            Tooltip.SetDefault("Fires two blasts of bullets per use\n" +
                "Continuing to hold left-click will spin the weapon while firing, creating a spiral of bullets\n" +
                "\nContinuing to hold left-click while aiming downwards will charge a purple beam that'll cause eruptions on impact");
            ItemID.Sets.SkipsInitialUseSound[Item.type] = true;
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 35;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 110;
            Item.height = 44;
            Item.useTime = 50;
            Item.useAnimation = 50;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.knockBack = 2;
            Item.value = Item.sellPrice(0, 10, 0, 0);
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = CustomSounds.ShotgunBlast1;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 10;
            Item.useAmmo = AmmoID.Bullet;
            if (!Main.dedServ)
                Item.RedemptionGlow().glowTexture = ModContent.Request<Texture2D>(Item.ModItem.Texture + "_Glow").Value;
        }
        public override bool CanConsumeAmmo(Item ammo, Player player) => false;
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            type = ModContent.ProjectileType<DAN_Proj>();
        }
    }
}
