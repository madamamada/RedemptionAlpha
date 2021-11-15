using Redemption.Globals;
using Redemption.Items.Materials.HM;
using Redemption.Items.Placeable.Tiles;
using Redemption.Tiles.Containers;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Containers
{
    public class LabChest2 : ModItem
	{
		public override void SetStaticDefaults()
		{
            DisplayName.SetDefault("Special Laboratory Chest");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<LabChestTileLocked2>(), 0);
			Item.width = 32;
			Item.height = 30;
			Item.maxStack = 99;
			Item.value = 5000;
			Item.rare = ItemRarityID.LightPurple;
		}
    }
}