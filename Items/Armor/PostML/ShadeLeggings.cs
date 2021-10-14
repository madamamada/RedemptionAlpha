using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent.Creative;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Redemption.Globals.Player;
using Redemption.Items.Materials.PostML;
using System;
using Redemption.Rarities;
using Redemption.DamageClasses;

namespace Redemption.Items.Armor.PostML
{
    [AutoloadEquip(EquipType.Legs)]
    public class ShadeLeggings : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shade Greaves");
            Tooltip.SetDefault("8% increased ritual damage\n" +
                "15% increased ritual critical strike chance\n" +
                "30% increased movement speed");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 24;
            Item.sellPrice(gold: 5);
            Item.rare = ModContent.RarityType<SoullessRarity>();
            Item.defense = 24;
        }

        public override void UpdateEquip(Player player)
        {
            player.moveSpeed *= 1.4f;
            player.GetDamage<RitualistClass>() += .8f;
            player.GetCritChance<RitualistClass>() += 15;
        }


        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<Shadesoul>(), 3)
            .AddTile(TileID.LunarCraftingStation)
            .Register();
        }
    }
}