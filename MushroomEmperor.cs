using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Wild.Common.AuxiliaryMeans;
using Wild.Common.UITools;
using Wild.Common;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Wild.Asset;
using Wild.Common.DrawTools;
using Wild.Common.HcGlobalNPC;
using System.Collections.Generic;
using System;
using Wild.Content.NPCs.FungusDevourers;
using Wild.Content.Projectiles;
using Wild.Content.Dusts;

namespace Wild.Content.NPCs.MushroomEmperors
{
    public class MushroomEmperor : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 3;
            NPCID.Sets.BossBestiaryPriority.Add(Type);//将其分组为其他Boss
            NPCID.Sets.CantTakeLunchMoney[Type] = true;//并不希望该实体拾取钱币
            NPCID.Sets.MustAlwaysDraw[Type] = true;//该NPC将始终绘制，即使离开屏幕

            //设置其免疫的DeBuff
            NPCDebuffImmunityData debuffData = new NPCDebuffImmunityData
            {
                SpecificallyImmuneTo = new int[] {
                    BuffID.Poisoned,
                    BuffID.Confused,
                }
            };
            NPCID.Sets.DebuffImmunitySets.Add(Type, debuffData);
        }

        public override void SetDefaults()
        {
            NPC.width = 60;
            NPC.height = 60;
            NPC.scale = 1.5f;

            NPC.damage = 322;
            NPC.defense = 66;
            NPC.lifeMax = 72000;

            NPC.noGravity = true;//使其不受到重力
            NPC.lavaImmune = true;//不会受到岩浆烫伤
            NPC.noTileCollide = true;//可穿墙
            NPC.knockBackResist = 0f;//不受击退

            NPC.boss = true;
            NPC.value = Item.buyPrice(gold: 12);
            NPC.npcSlots = 25f;
            NPC.aiStyle = -1;

            NPC.BossBar = ModContent.GetInstance<NullBossBar>();

            //Music = MusicLoader.GetMusicSlot(Mod, WdConstant.Sounds + "FightingTheNight");
        }

        public override string Texture => WdConstant.NPC_FDers + "MushroomEmperor";

        public static int normalIconIndex = -1;

        public override void BossHeadSlot(ref int index)
        {
            index = ModContent.GetModBossHeadSlot(WdConstant.NPC_FDers + "MushroomEmperor_Map");
        }

        public int BodyIndex
        {
            set => NPC.ai[0] = value;
            get => (int)NPC.ai[0];
        }

        public int HeadIndex
        {
            set => NPC.ai[3] = value;
            get => (int)NPC.ai[3];
        }

        public int Status
        {
            set => NPC.ai[1] = value;
            get => (int)NPC.ai[1];
        }

        public int ThisTimeVlue
        {
            set => NPC.ai[2] = value;
            get => (int)NPC.ai[2];
        }

        public int ThisNPCwhoAmI = -1;

        public void OnSpanAction()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient) return;
            AiBehavior.LoadList(ref NPCwhoAmI.MushroomEmperorList, NPC, ref ThisNPCwhoAmI);
        }

        public void OnKillAction()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient) return;
            AiBehavior.UnLoadList(ref NPCwhoAmI.MushroomEmperorList, NPC, ref ThisNPCwhoAmI);
        }

        public override bool CheckDead()
        {
            return base.CheckDead();
        }

        public override void OnKill()
        {
            OnKillAction();
            base.OnKill();
        }

        public override void AI()
        {
            if (Status == 0)
            {
                OnSpanAction();
                Status = 1;
            }

            if (BodyIndex < 0 || BodyIndex >= Main.npc.Length)
            {
                NPC.life = 0;
                NPC.active = false;
            }
            ThisTimeVlue++;
            NPC.timeLeft = 10;
            NPC body = Main.npc[BodyIndex];
            Player target = AiBehavior.FindingTarget(NPC, -1);

            if (AiBehavior.NPCAlive(body) == false || AiBehavior.PlayerAlive(target) == false)
            {
                NPC.life = 0;
                NPC.active = false;
            }

            if (Status == 1)
            {
                Vector2 bodyVer = body.velocity;
                Vector2 haedPos = body.Center + (HeadIndex * MathHelper.ToRadians(180 / body.ai[3]) + MathHelper.ToRadians(-170)).ToRotationVector2() * 780f + new Vector2(0, (float)Math.Sin(ThisTimeVlue * 0.1f) * 35f);
                Vector2 chasingRvr = (HeadIndex * MathHelper.ToRadians(180 / body.ai[3])).ToRotationVector2() * 360f;
                float chasMaxDg = 380 * 380;

                if ((NPC.Center - target.Center - chasingRvr).LengthSquared() <= chasMaxDg)
                {
                    NPC.velocity = AiBehavior.GetChasingVelocity(NPC, target.Center + chasingRvr, 7f, 16) + bodyVer;
                }
                else
                {
                    NPC.velocity = AiBehavior.GetChasingVelocity(NPC, haedPos, 7f, 16) + bodyVer;
                }



                Vector2 ToTargVr = target.Center - NPC.Center;

                if (ThisTimeVlue > 120 + HeadIndex * 120)
                {
                    if (Main.fpsCount % 5 == 0)
                    {
                        for (int i = 0; i < 13; i++)
                        {
                            Vector2 vr = (ToTargVr.ToRotation() + MathHelper.ToRadians(HcMath.HcRandom.Next(-5, 5))).ToRotationVector2() * (15f + HcMath.HcRandom.Next(-5, 15));
                            Projectile.NewProjectile(AiBehavior.GetEntitySource_Parent(NPC), NPC.Center, vr, ModContent.ProjectileType<MushroomSpores>(), 545, 0f, Main.myPlayer);
                            Dust.NewDust(NPC.Center, 16, 16, ModContent.DustType<MushroomPowder>(), vr.X * 3f, vr.Y * 3f);
                        }
                    }                   
                }
                if (ThisTimeVlue > 180 + +HeadIndex * 120)
                {
                    ThisTimeVlue = 0;
                }
            }
        }

        // 计算九头蛇头部的移动速度
        public static Vector2 GetSnakeHeadVelocity(Vector2 headPosition, Vector2 targetPosition, float speed, float minDistance, float maxDistance)
        {
            Vector2 toTarget = targetPosition - headPosition;

            // 如果距离小于最小距离，头部停止移动
            if (toTarget.LengthSquared() < minDistance * minDistance)
            {
                return Vector2.Zero;
            }

            // 如果距离大于最大距离，头部以最大速度朝向目标移动
            if (toTarget.LengthSquared() > maxDistance * maxDistance)
            {
                toTarget.Normalize();
                return toTarget * speed;
            }

            // 距离在最小距离和最大距离之间时，头部以渐进速度朝向目标移动
            float distanceFactor = MathHelper.Clamp((toTarget.Length() - minDistance) / (maxDistance - minDistance), 0f, 1f);
            toTarget.Normalize();
            Vector2 velocity = toTarget * speed * distanceFactor;

            return velocity;
        }

        List<Vector2> Points = new List<Vector2>();
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            DrawUtils.ClockFrame(ref NPC.frameCounter, 15, 2);

            Color recombinationColor = new Color(drawColor.ToVector4() * 0.5f + Color.White.ToVector4() * 0.5f);

            Vector2 strpos = NPC.Center + new Vector2(0, 75);
            Vector2 endpos = Main.npc[BodyIndex].Center + new Vector2(0, -75);
            Vector2 controlPoint = (strpos + endpos) / 2f;
            Vector2 toEndVr = strpos - endpos;
            float Vrleng = toEndVr.Length();
            float connectionSacle = 1.52f;
            int pointNum = (int)(2 * Vrleng / (WdTexture.Mushroom_connection.Height * connectionSacle));

            HcMath.GenerateCurve2(ref Points, strpos, endpos, pointNum, controlPoint, 0.3f);

            if (Points.Count > pointNum) Points = new List<Vector2>();
            for (int i = 0; i < Points.Count - 2; i++)
            {
                Vector2 vr1 = Points[i];
                Vector2 vr2 = Points[i + 1];
                Vector2 toVr = vr2 - vr1;
                float rot = toVr.ToRotation();
                float leng = toVr.Length();

                Main.EntitySpriteDraw(
                WdTexture.Mushroom_connection,
                DrawUtils.WDEpos(vr1),
                DrawUtils.GetRec(WdTexture.Mushroom_connection),
                Color.White,
                rot + MathHelper.PiOver2,
                DrawUtils.GetOrig(WdTexture.Mushroom_connection),
                connectionSacle,
                SpriteEffects.None,
                0
                );
            }

            Main.EntitySpriteDraw(
                WdTexture.MushroomEmperor,
                DrawUtils.WDEpos(NPC),
                DrawUtils.GetRec(WdTexture.MushroomEmperor, (int)NPC.frameCounter, 3),
                recombinationColor,
                NPC.rotation,
                DrawUtils.GetOrig(WdTexture.MushroomEmperor, (int)NPC.frameCounter, 3),
                NPC.scale,
                SpriteEffects.None,
                0
                );

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

            FungusDevourerHead.BlueGlowShader(WdTexture.MushroomEmperor_Men, 10, new Vector4(0, 0, 1, 1));
            Main.EntitySpriteDraw(
                WdTexture.MushroomEmperor_Men,
                DrawUtils.WDEpos(NPC),
                DrawUtils.GetRec(WdTexture.MushroomEmperor_Men, (int)NPC.frameCounter, 3),
                drawColor,
                NPC.rotation,
                DrawUtils.GetOrig(WdTexture.MushroomEmperor_Men, (int)NPC.frameCounter, 3),
                NPC.scale,
                SpriteEffects.None,
                0
                );

            Main.spriteBatch.ResetBlendState();
            return false;
        }
    }
}
