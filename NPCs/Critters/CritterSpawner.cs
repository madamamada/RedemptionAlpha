﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.Globals;
using Terraria.ModLoader.Utilities;
using Terraria.Utilities;
using Microsoft.Xna.Framework;
using Redemption.Tiles.Tiles;
using System.Linq;
using Terraria.DataStructures;

namespace Redemption.NPCs.Critters
{
    public class ChickenSpawner : ModNPC
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Hide = true };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            NPC.width = 26;
            NPC.height = 22;
            NPC.lifeMax = 1;
            NPC.aiStyle = -1;
        }
        public enum SpawnType
        {
            Single, Small, Big
        }
        public override bool PreAI()
        {
            WeightedRandom<SpawnType> SpawnChoice = new(Main.rand);
            SpawnChoice.Add(SpawnType.Single, 10);
            SpawnChoice.Add(SpawnType.Small, 5);
            SpawnChoice.Add(SpawnType.Big, 1);

            Vector2 pos = Vector2.Zero;
            switch ((SpawnType)SpawnChoice)
            {
                case SpawnType.Single:
                    NPC.SetDefaults(ModContent.NPCType<Chicken>());
                    break;
                case SpawnType.Small:
                    for (int i = 0; i < 2; i++)
                    {
                        pos = RedeHelper.FindGround(NPC, 6);
                        RedeHelper.SpawnNPC(new EntitySource_SpawnNPC(), (int)pos.X * 16, (int)pos.Y * 16, ModContent.NPCType<Chicken>());
                    }
                    NPC.active = false;
                    break;
                case SpawnType.Big:
                    for (int i = 0; i < Main.rand.Next(3, 5); i++)
                    {
                        pos = RedeHelper.FindGround(NPC, 8);
                        RedeHelper.SpawnNPC(new EntitySource_SpawnNPC(), (int)pos.X * 16, (int)pos.Y * 16, ModContent.NPCType<Chicken>());
                    }
                    NPC.active = false;
                    break;
            }
            return true;
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            float baseChance = SpawnCondition.OverworldDay.Chance;
            float multiplier = Main.tile[spawnInfo.spawnTileX, spawnInfo.spawnTileY].TileType == TileID.Grass ? 0.2f : 0f;

            return baseChance * multiplier;
        }
    }
    public class KabucraSpawner : ModNPC
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Hide = true };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            NPC.width = 20;
            NPC.height = 22;
            NPC.lifeMax = 1;
            NPC.aiStyle = -1;
        }
        public enum SpawnType
        {
            Single, Small, Big
        }
        public override bool PreAI()
        {
            WeightedRandom<SpawnType> SpawnChoice = new(Main.rand);
            SpawnChoice.Add(SpawnType.Single, 10);
            SpawnChoice.Add(SpawnType.Small, 5);
            SpawnChoice.Add(SpawnType.Big, 1);

            Vector2 pos = Vector2.Zero;
            switch ((SpawnType)SpawnChoice)
            {
                case SpawnType.Single:
                    NPC.SetDefaults(ModContent.NPCType<Kabucra>());
                    break;
                case SpawnType.Small:
                    for (int i = 0; i < 2; i++)
                    {
                        pos = RedeHelper.FindGround(NPC, 5);
                        RedeHelper.SpawnNPC(new EntitySource_SpawnNPC(), (int)pos.X * 16, (int)pos.Y * 16, ModContent.NPCType<Kabucra>());
                    }
                    NPC.active = false;
                    break;
                case SpawnType.Big:
                    for (int i = 0; i < Main.rand.Next(3, 5); i++)
                    {
                        pos = RedeHelper.FindGround(NPC, 8);
                        RedeHelper.SpawnNPC(new EntitySource_SpawnNPC(), (int)pos.X * 16, (int)pos.Y * 16, ModContent.NPCType<Kabucra>());
                    }
                    NPC.active = false;
                    break;
            }
            return true;
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            float baseChance = SpawnCondition.Overworld.Chance;
            float multiplier = Main.tile[spawnInfo.spawnTileX, spawnInfo.spawnTileY].TileType == TileID.Sand && !spawnInfo.water && spawnInfo.player.ZoneBeach ? 3f : 0;

            return baseChance * multiplier;
        }
    }
}