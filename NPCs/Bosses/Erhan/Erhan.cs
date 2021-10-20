using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.IO;
using Redemption.Items.Weapons.PreHM.Melee;
using Redemption.Items.Weapons.PreHM.Ranged;
using Redemption.Items.Armor.Vanity;
using Redemption.Items.Placeable.Trophies;
using Redemption.Items.Usable;
using Redemption.Globals;
using Terraria.GameContent;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using System.Collections.Generic;
using Terraria.GameContent.ItemDropRules;
using Terraria.Audio;
using Redemption.Base;
using Terraria.Graphics.Shaders;
using Redemption.Items.Accessories.PreHM;
using Redemption.Items.Materials.PreHM;
using System.Linq;
using Redemption.Items.Weapons.PreHM.Magic;
using Redemption.Dusts;
using Redemption.NPCs.Friendly;

namespace Redemption.NPCs.Bosses.Erhan
{
    [AutoloadBossHead]
    public class Erhan : ModNPC
    {
        public enum ActionState
        {
            Begin,
            Idle,
            Attacks,
            Death
        }

        public ActionState AIState
        {
            get => (ActionState)NPC.ai[0];
            set => NPC.ai[0] = (int)value;
        }

        public ref float AITimer => ref NPC.ai[1];

        public ref float TimerRand => ref NPC.ai[2];

        public float[] oldrot = new float[5];
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Erhan, Anglonic High Priest");
            Main.npcFrameCount[NPC.type] = 6;
            NPCID.Sets.TrailCacheLength[NPC.type] = 5;
            NPCID.Sets.TrailingMode[NPC.type] = 1;

            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0)
            {
                Position = new Vector2(0, 36),
                PortraitPositionYOverride = 8
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            NPC.lifeMax = 2100;
            NPC.damage = 21;
            NPC.defense = 6;
            NPC.knockBackResist = 0f;
            NPC.width = 34;
            NPC.height = 60;
            NPC.npcSlots = 10f;
            NPC.SpawnWithHigherTime(30);
            NPC.boss = true;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.netAlways = true;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.dontTakeDamage = true;
            if (!Main.dedServ)
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/BossForest1");
            //BossBag = ModContent.ItemType<KeeperBag>();
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public override bool? CanHitNPC(NPC target) => false;

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.6f * bossLifeScale);
            NPC.damage = (int)(NPC.damage * 0.6f);
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.DayTime,

                new FlavorTextBestiaryInfoElement("A high priest of Fairwood, tasked himself to purify the forest's curse after it consumed it's warden.")
            });
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            //npcLoot.Add(ItemDropRule.BossBag(BossBag));
            //npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<KeeperTrophy>(), 10));

            //npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ModContent.ItemType<OcciesCollar>(), 4));

            //LeadingConditionRule notExpertRule = new(new Conditions.NotExpert());

            //notExpertRule.OnSuccess(ItemDropRule.OneFromOptions(1,
            //    ModContent.ItemType<SoulScepter>(), ModContent.ItemType<KeepersClaw>(), ModContent.ItemType<FanOShivs>()));
            //notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<GrimShard>(), 1, 2, 4));

            //npcLoot.Add(notExpertRule);
        }

        public override void OnKill()
        {
            if (!RedeBossDowned.downedErhan)
            {
                RedeWorld.alignment--;
                for (int p = 0; p < Main.maxPlayers; p++)
                {
                    Player player = Main.player[p];
                    if (!player.active)
                        continue;

                    CombatText.NewText(player.getRect(), Color.Gold, "-1", true, false);

                    if (!player.HasItem(ModContent.ItemType<AlignmentTeller>()))
                        continue;

                    if (!Main.dedServ)
                        RedeSystem.Instance.ChaliceUIElement.DisplayDialogue("Attempting to summon a demon and fighting a priest... Are you alright in the head?", 240, 30, 0, Color.DarkGoldenrod);

                }
            }
            NPC.SetEventFlagCleared(ref RedeBossDowned.downedErhan, -1);
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            if (Main.netMode == NetmodeID.Server || Main.dedServ)
            {
                writer.Write(ID);
            }
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                ID = reader.ReadInt32();
            }
        }

        void AttackChoice()
        {
            int attempts = 0;
            while (attempts == 0)
            {
                if (CopyList == null || CopyList.Count == 0)
                    CopyList = new List<int>(AttackList);
                ID = CopyList[Main.rand.Next(0, CopyList.Count)];
                CopyList.Remove(ID);
                NPC.netUpdate = true;

                attempts++;
            }
        }

        public List<int> AttackList = new() { 0, 1, 2 };
        public List<int> CopyList = null;

        private float move;
        private float speed = 6;
        private Vector2 origin;
        private bool floatTimer;

        public int ID { get => (int)NPC.ai[3]; set => NPC.ai[3] = value; }

        public override void AI()
        {
            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();

            Player player = Main.player[NPC.target];

            DespawnHandler();

            if (!floatTimer)
            {
                NPC.velocity.Y += 0.03f;
                if (NPC.velocity.Y > .5f)
                {
                    floatTimer = true;
                    NPC.netUpdate = true;
                }
            }
            else if (floatTimer)
            {
                NPC.velocity.Y -= 0.03f;
                if (NPC.velocity.Y < -.5f)
                {
                    floatTimer = false;
                    NPC.netUpdate = true;
                }
            }

            NPC.LookAtEntity(player);

            switch (AIState)
            {
                case ActionState.Begin:
                    switch (TimerRand)
                    {
                        case 0:
                            if (!Main.dedServ)
                                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/silence");

                            SoundEngine.PlaySound(SoundID.Item68, NPC.position);
                            player.GetModPlayer<ScreenPlayer>().ScreenShakeIntensity = 14;
                            HolyFlare = true;
                            TeleGlow = true;
                            TimerRand = 1;
                            NPC.netUpdate = true;
                            break;
                        case 1:
                            if (!Main.dedServ)
                            {
                                if (RedeBossDowned.erhanDeath <= 0)
                                {
                                    if (AITimer++ == 0)
                                    {
                                        ArmType = 2;
                                        RedeSystem.Instance.DialogueUIElement.DisplayDialogue("Great heavens!!", 120, 1, 0.6f, "Erhan:", 2f, Color.LightGoldenrodYellow, null, null, NPC.Center, sound: true);
                                    }
                                    if (AITimer >= 120)
                                    {
                                        player.GetModPlayer<ScreenPlayer>().ScreenFocusPosition = NPC.Center;
                                        player.GetModPlayer<ScreenPlayer>().lockScreen = true;
                                    }
                                    if (AITimer == 120)
                                    {
                                        ArmType = 0;
                                        RedeSystem.Instance.DialogueUIElement.DisplayDialogue("Doth thine brain be stuck in a well!?", 240, 1, 0.6f, "Erhan:", 1f, Color.LightGoldenrodYellow, null, null, NPC.Center, sound: true);
                                    }
                                    if (AITimer == 360)
                                        RedeSystem.Instance.DialogueUIElement.DisplayDialogue("To summon a demon, so close to my land... 'Tis heresy!", 240, 1, 0.6f, "Erhan:", 1f, Color.LightGoldenrodYellow, null, null, NPC.Center, sound: true);
                                    if (AITimer == 600)
                                    {
                                        ArmType = 2;
                                        HeadFrameY = 1;
                                        RedeSystem.Instance.DialogueUIElement.DisplayDialogue("Repent! Repent for thy sins!", 200, 1, 0.6f, "Erhan:", 1f, Color.LightGoldenrodYellow, null, null, NPC.Center, sound: true);
                                    }
                                    if (AITimer == 800)
                                    {
                                        ArmType = 0;
                                        HeadFrameY = 0;
                                        RedeSystem.Instance.DialogueUIElement.DisplayDialogue("Lest I smack'eth thine buttocks with the Hand of Judgement!", 240, 1, 0.6f, "Erhan:", 1f, Color.LightGoldenrodYellow, null, null, NPC.Center, sound: true);
                                    }
                                    if (AITimer >= 1040)
                                    {
                                        if (!Main.dedServ)
                                        {
                                            RedeSystem.Instance.TitleCardUIElement.DisplayTitle("Erhan", 60, 90, 0.8f, 0, Color.Goldenrod,
                                                "Anglonic High Priest");
                                            Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/BossForest1");
                                        }
                                        TimerRand = 0;
                                        AITimer = 0;
                                        NPC.dontTakeDamage = false;
                                        AIState = ActionState.Idle;
                                        NPC.netUpdate = true;
                                    }
                                }
                                else
                                {
                                    if (AITimer++ == 0)
                                        RedeSystem.Instance.DialogueUIElement.DisplayDialogue("CEASE!", 120, 1, 0.6f, "Erhan:", 2f, Color.LightGoldenrodYellow, null, null, NPC.Center, sound: true);

                                    if (AITimer >= 120)
                                    {
                                        if (!Main.dedServ)
                                        {
                                            RedeSystem.Instance.TitleCardUIElement.DisplayTitle("Erhan", 60, 90, 0.8f, 0, Color.Goldenrod,
                                                "Anglonic High Priest");
                                            Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/BossForest1");
                                        }
                                        TimerRand = 0;
                                        AITimer = 0;
                                        NPC.dontTakeDamage = false;
                                        AIState = ActionState.Idle;
                                        NPC.netUpdate = true;
                                    }
                                }
                            }
                            break;
                    }
                    break;
                case ActionState.Idle:
                    if (AITimer++ == 0)
                    {
                        move = NPC.Center.X;
                        speed = 9;
                    }
                    NPC.Move(new Vector2(move, player.Center.Y - 250), speed, 50, false);
                    MoveClamp();
                    if (NPC.DistanceSQ(player.Center) > 800 * 800)
                        speed *= 1.03f;
                    else if (NPC.velocity.Length() > 9 && NPC.DistanceSQ(player.Center) <= 800 * 800)
                        speed *= 0.96f;

                    if (AITimer > 80)
                    {
                        AttackChoice();
                        AITimer = 0;
                        AIState = ActionState.Attacks;
                        NPC.netUpdate = true;
                    }
                    break;
                case ActionState.Attacks:
                    switch (ID)
                    {
                        #region Lightmass
                        case 0:
                            AITimer++;
                            if (AITimer < 60)
                                NPC.Move(new Vector2(player.Center.X + (40 * NPC.spriteDirection), player.Center.Y - 250), 10, 40, false);
                            else
                                NPC.velocity *= 0.96f;

                            if (AITimer == 80)
                                ArmType = 1;
                            if (AITimer == 100 || (Main.rand.NextBool(2) ? AITimer == 120 : AITimer == -1))
                            {
                                TeleGlow = true;
                                TeleGlowTimer = 0;
                                for (int i = 0; i < Main.rand.Next(4, 7); i++)
                                    NPC.Shoot(NPC.Center, ModContent.ProjectileType<Erhan_Lightmass>(), NPC.damage,
                                        new Vector2(Main.rand.NextFloat(-3, 3), Main.rand.NextFloat(-9, -5)), false, SoundID.Item101);
                            }
                            if (AITimer == 140)
                                ArmType = 0;

                            if (AITimer >= 200)
                            {
                                AITimer = 0;
                                AIState = ActionState.Idle;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

                        #region Scorching Rays
                        case 1:
                            if (AITimer++ == 0)
                            {
                                move = NPC.Center.X;
                                speed = 7;
                            }
                            NPC.Move(new Vector2(move, player.Center.Y - 250), speed, 50, false);
                            MoveClamp();
                            if (NPC.DistanceSQ(player.Center) > 800 * 800)
                                speed *= 1.03f;
                            else if (NPC.velocity.Length() > 9 && NPC.DistanceSQ(player.Center) <= 800 * 800)
                                speed *= 0.96f;

                            if (AITimer == 20)
                            {
                                HeadFrameY = 1;
                                ArmType = 2;
                            }
                            if (AITimer >= 40 && AITimer % 30 == 0 && AITimer <= 220)
                            {
                                NPC.Shoot(new Vector2(player.Center.X + Main.rand.Next(-600, 600), player.Center.Y - 600),
                                    ModContent.ProjectileType<ScorchingRay>(), (int)(NPC.damage * 1.5f),
                                    new Vector2(Main.rand.NextFloat(-1, 1), 10), false, SoundID.Item162);
                            }
                            if (AITimer == 340)
                            {
                                HeadFrameY = 0;
                                ArmType = 0;
                            }

                            if (AITimer >= 350)
                            {
                                AITimer = 0;
                                AIState = ActionState.Idle;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

                        #region Holy Spears
                        case 2:
                            AITimer++;
                            if (AITimer < 80)
                                NPC.Move(new Vector2(player.Center.X + (40 * NPC.spriteDirection), player.Center.Y - 250), 10, 40, false);
                            else
                                NPC.velocity *= 0.5f;

                            if (AITimer == 80)
                                ArmType = 1;
                            if (AITimer >= 90 && AITimer % 5 == 0 && AITimer <= 130)
                            {
                                player.GetModPlayer<ScreenPlayer>().ScreenShakeIntensity = 4;
                                TimerRand += (float)Math.PI / 15;
                                if (TimerRand > (float)Math.PI)
                                {
                                    TimerRand -= (float)Math.PI * 2;
                                }
                                NPC.Shoot(NPC.Center, ModContent.ProjectileType<HolySpear_Proj>(), NPC.damage,
                                    new Vector2(0.1f, 0).RotatedBy(TimerRand + Math.PI / 2), false, SoundID.Item125);
                                NPC.Shoot(NPC.Center, ModContent.ProjectileType<HolySpear_Proj>(), NPC.damage,
                                    new Vector2(0.1f, 0).RotatedBy(-TimerRand + Math.PI / 2), false, SoundID.Item125);
                            }
                            if (AITimer == 150)
                                ArmType = 0;

                            if (AITimer >= 160)
                            {
                                TimerRand = 0;
                                AITimer = 0;
                                AIState = ActionState.Idle;
                                NPC.netUpdate = true;
                            }
                            break;
                            #endregion
                    }
                    break;
                case ActionState.Death:
                    NPC.dontTakeDamage = true;
                    NPC.velocity *= 0;
                    break;
            }
        }

        public override bool CheckDead()
        {
            if (AIState is ActionState.Death)
                return true;
            else
            {
                NPC.dontTakeDamage = true;
                NPC.velocity *= 0;
                NPC.alpha = 0;
                NPC.life = 1;
                AITimer = 0;
                AIState = ActionState.Death;
                return false;
            }
        }

        private int ArmFrameY;
        private int ArmType;
        private int HeadFrameY;
        private bool HolyFlare;
        private int HolyFlareTimer;
        private bool TeleGlow;
        private int TeleGlowTimer;
        public override void FindFrame(int frameHeight)
        {
            for (int k = NPC.oldPos.Length - 1; k > 0; k--)
            {
                oldrot[k] = oldrot[k - 1];
            }
            oldrot[0] = NPC.rotation;

            if (HolyFlare)
            {
                HolyFlareTimer++;
                if (HolyFlareTimer > 60)
                {
                    HolyFlare = false;
                    HolyFlareTimer = 0;
                }
            }
            if (TeleGlow)
            {
                TeleGlowTimer += 3;
                if (TeleGlowTimer > 60)
                {
                    TeleGlow = false;
                    TeleGlowTimer = 0;
                }
            }

            ArmFrameY = (NPC.frame.Y / frameHeight) + (6 * ArmType);

            if (++NPC.frameCounter >= 5)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y > 5 * frameHeight)
                    NPC.frame.Y = 0 * frameHeight;
            }
        }

        public void MoveClamp()
        {
            Player player = Main.player[NPC.target];
            int xFar = 400;
            if (NPC.Center.X < player.Center.X)
            {
                if (move < player.Center.X - xFar)
                {
                    move = player.Center.X - xFar;
                }
                else if (move > player.Center.X - 200)
                {
                    move = player.Center.X - 200;
                }
            }
            else
            {
                if (move > player.Center.X + xFar)
                {
                    move = player.Center.X + xFar;
                }
                else if (move < player.Center.X + 200)
                {
                    move = player.Center.X + 200;
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D ArmsTex = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_Arms").Value;
            Texture2D HeadTex = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_Head").Value;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            int shader = ContentSamples.CommonlyUsedContentSamples.ColorOnlyShaderIndex;
            Color shaderColor = BaseUtility.MultiLerpColor(Main.LocalPlayer.miscCounter % 100 / 100f, Color.Yellow, Color.Goldenrod * 0.7f, Color.Yellow);

            if (!NPC.IsABestiaryIconDummy)
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
                GameShaders.Armor.ApplySecondary(shader, Main.player[Main.myPlayer], null);

                for (int i = 0; i < NPCID.Sets.TrailCacheLength[NPC.type]; i++)
                {
                    Vector2 oldPos = NPC.oldPos[i];
                    Main.spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, oldPos + NPC.Size / 2f - screenPos + new Vector2(0, NPC.gfxOffY), NPC.frame, NPC.GetAlpha(shaderColor) * 0.5f, oldrot[i], NPC.frame.Size() / 2, NPC.scale + 0.1f, effects, 0);
                }

                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            }

            spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

            int heightHead = HeadTex.Height / 3;
            int yHead = heightHead * HeadFrameY;
            Rectangle rectHead = new(0, yHead, HeadTex.Width, heightHead);
            Vector2 originHead = new(HeadTex.Width / 2f, heightHead / 2f);
            Main.spriteBatch.Draw(HeadTex, NPC.Center - screenPos - new Vector2(-2 * NPC.spriteDirection, 33), new Rectangle?(rectHead), NPC.GetAlpha(drawColor), NPC.rotation, originHead, NPC.scale, effects, 0);

            int heightArms = ArmsTex.Height / 18;
            int yArms = heightArms * ArmFrameY;
            Rectangle rectArms = new(0, yArms, ArmsTex.Width, heightArms);
            Vector2 originArms = new(ArmsTex.Width / 2f, heightArms / 2f);
            Main.spriteBatch.Draw(ArmsTex, NPC.Center - screenPos + new Vector2(-2 * NPC.spriteDirection, -10), new Rectangle?(rectArms), NPC.GetAlpha(drawColor), NPC.rotation, originArms, NPC.scale, effects, 0);

            return false;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D flare = ModContent.Request<Texture2D>("Redemption/Textures/HolyGlow2").Value;
            Rectangle rect = new(0, 0, flare.Width, flare.Height);
            Vector2 origin = new(flare.Width / 2, flare.Height / 2);
            Vector2 position = NPC.Center - screenPos;
            Color colour = Color.Lerp(Color.White, Color.White, 1f / HolyFlareTimer * 10f) * (1f / HolyFlareTimer * 10f);
            if (HolyFlare)
            {
                spriteBatch.Draw(flare, position, new Rectangle?(rect), colour, NPC.rotation, origin, 3f, SpriteEffects.None, 0);
                spriteBatch.Draw(flare, position, new Rectangle?(rect), colour * 0.4f, NPC.rotation, origin, 2.5f, SpriteEffects.None, 0);
            }

            Texture2D teleportGlow = ModContent.Request<Texture2D>("Redemption/Textures/HolyGlow3").Value;
            Rectangle rect2 = new(0, 0, teleportGlow.Width, teleportGlow.Height);
            Vector2 origin2 = new(teleportGlow.Width / 2, teleportGlow.Height / 2);
            Vector2 position2 = NPC.Center - screenPos;
            Color colour2 = Color.Lerp(Color.White, Color.White, 1f / TeleGlowTimer * 10f) * (1f / TeleGlowTimer * 10f);
            if (TeleGlow)
            {
                spriteBatch.Draw(teleportGlow, position2, new Rectangle?(rect2), colour2, NPC.rotation, origin2, 2f, SpriteEffects.None, 0);
                spriteBatch.Draw(teleportGlow, position2, new Rectangle?(rect2), colour2 * 0.4f, NPC.rotation, origin2, 2f, SpriteEffects.None, 0);
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
        }

        public override void ModifyHitByItem(Player player, Item item, ref int damage, ref float knockback, ref bool crit)
        {
            if (ItemTags.Celestial.Has(item.type) || ItemTags.Psychic.Has(item.type))
                damage = (int)(damage * 0.9f);

            if (ItemTags.Holy.Has(item.type))
                damage = (int)(damage * 0.5f);

            if (ItemTags.Shadow.Has(item.type))
                damage = (int)(damage * 1.25f);
        }
        public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (ProjectileTags.Celestial.Has(projectile.type) || ProjectileTags.Psychic.Has(projectile.type))
                damage = (int)(damage * 0.9f);

            if (ProjectileTags.Holy.Has(projectile.type))
                damage = (int)(damage * 0.5f);

            if (ProjectileTags.Shadow.Has(projectile.type))
                damage = (int)(damage * 1.25f);
        }

        private void DespawnHandler()
        {
            Player player = Main.player[NPC.target];
            if (!player.active || player.dead)
            {
                NPC.TargetClosest(false);
                player = Main.player[NPC.target];
                if (!player.active || player.dead)
                {
                    NPC.alpha += 2;
                    if (NPC.alpha >= 255)
                        NPC.active = false;
                    if (NPC.timeLeft > 10)
                        NPC.timeLeft = 10;
                    return;
                }
            }
        }
    }
}