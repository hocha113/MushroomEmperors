using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;
using Wild.Asset;
using Wild.Common;
using Wild.Common.AuxiliaryMeans;
using Wild.Common.DrawTools;
using Wild.Common.HcGlobalNPC;
using Wild.Common.HcGlobalProjectile;
using Wild.Common.SoundEffects;
using Wild.Common.UITools;
using Wild.Common.WorldGeneration;
using Wild.Content.NPCs.FungusDevourers;
using Wild.Content.Projectiles.FungusProjectile;

namespace Wild.Content.NPCs.MushroomEmperors
{
    public class MushroomEmperorBody : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 5;
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
            NPC.scale = 2.5f;

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

            Music = MusicLoader.GetMusicSlot(Mod, WdConstant.Sounds + "FightingTheNight"); 
        }

        public override string Texture => WdConstant.NPC_FDers + "Sporangium";
        public override string Name => Languages.Translation("真菌帝王", "Mushroom Emperor");

        public static int normalIconIndex = -1;

        public override void BossHeadSlot(ref int index)
        {
            index = ModContent.GetModBossHeadSlot(WdConstant.NPC_FDers + "Sporangium_Map");
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

        public int HaedNum
        {
            set => NPC.ai[3] = value;
            get => (int)NPC.ai[3];
        }

        public int ThisNPCwhoAmI = -1;

        public void OnSpanAction()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient) return;

            AiBehavior.LoadList(ref NPCwhoAmI.MushroomEmperorBodyList, NPC, ref ThisNPCwhoAmI);
        }

        public void OnKillAction()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient) return;

            AiBehavior.UnLoadList(ref NPCwhoAmI.MushroomEmperorBodyList, NPC, ref ThisNPCwhoAmI);
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

        /// <summary>
        /// 存储脚部的位置数据
        /// </summary>
        List<Vector2> PinCoordinate = Enumerable.Repeat(Vector2.Zero, 4).ToList();
        List<Vector2> ToPinCoordinate = Enumerable.Repeat(Vector2.Zero, 4).ToList();

        int PinCoordIndex = 0;
        public override void AI()
        {            
            if (Status == 0)
            {
                HaedNum = 9;
                for (int i = 0; i < HaedNum; i++)
                {
                    int npc = NPC.NewNPC(AiBehavior.GetEntitySource_Parent(NPC), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<MushroomEmperor>());
                    Main.npc[npc].ai[0] = NPC.whoAmI;
                    Main.npc[npc].ai[3] = i;
                }

                ToPinCoordinate = Enumerable.Repeat(Vector2.Zero, 4).ToList();
                for (int j = 0; j < 4; j++)
                {
                    Vector2 pinCoordinate = (j * MathHelper.Pi / 4f + MathHelper.Pi / 8f).ToRotationVector2() * 280f + NPC.Center;
                    PinCoordinate[j] = pinCoordinate;
                }

                OnSpanAction();
                Status = 1;
            }
            ThisTimeVlue++;
            NPC.timeLeft = 10;
            Player target = AiBehavior.FindingTarget(NPC, -1);

            if (AiBehavior.PlayerAlive(target) == false)
            {
                NPC.life = 0;
                NPC.active = false;
            }
            Music = 84;
            if (Status == 1)
            {
                AiBehavior.ChasingBehavior(NPC, target.Center, 9f, 16);

                if (ThisTimeVlue % 240 == 0 && ThisTimeVlue > 240)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        Vector2 spanPos = target.Center + HcMath.GetRandomVevtor(0, 360, HcMath.HcRandom.Next(360, 710));
                        Projectile.NewProjectile(AiBehavior.GetEntitySource_Parent(NPC), spanPos, Vector2.Zero, ModContent.ProjectileType<MushroomSwirl>(), 545, 0f, Main.myPlayer);
                    }                   
                }

                if (ThisTimeVlue % 280 == 0 && ProJwhoAmI.GiantFungusTreeBombLengList.GetIntListCount() == 0 && false)
                {
                    Vector2 vr = (target.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 15f;
                    Projectile.NewProjectile(AiBehavior.GetEntitySource_Parent(NPC), NPC.Center, vr, ModContent.ProjectileType<GiantFungusTreeBomb_Leng>(), 545, 0f, Main.myPlayer);
                }
            }

            TakeStep();

            //SoundEngine.PlaySound(ModSound.SoundEnumerator((int)ModSound.ID.Hcan) with { MaxInstances = 3, SoundLimitBehavior = SoundLimitBehavior.ReplaceOldest }, player.Center);
            

            if (!SkyManager.Instance["Wild:MushroomEmperorSky"].IsActive())
            {
                SkyManager.Instance.Activate("Wild:MushroomEmperorSky");
            }
        }

        public void TakeStep()
        {
            if (ThisTimeVlue % 120 == 0)
            {
                for (int i = 0; i < 160; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        Vector2 climbingPath = (NPC.velocity.ToRotation() + MathHelper.ToRadians(HcMath.HcRandom.Next(-60, 60))).ToRotationVector2() * i * 8 + NPC.Center;
                        Vector2 tilePosPath = AiBehavior.WEPosToTilePos(climbingPath);
                        Tile tile = Main.tile[(int)tilePosPath.X, (int)tilePosPath.Y];
                        if (tile.HasTile || tile.TopSlope())
                        {
                            ToPinCoordinate[j] = climbingPath;
                            continue;
                        }
                    }
                }
            }

            if (ToPinCoordinate[PinCoordIndex] != Vector2.Zero)
            {
                Vector2 ToPinVr = (ToPinCoordinate[PinCoordIndex] - PinCoordinate[PinCoordIndex]).SafeNormalize(Vector2.Zero) * 52f + NPC.velocity;
                PinCoordinate[PinCoordIndex] += ToPinVr;

                if ((ToPinCoordinate[PinCoordIndex] - PinCoordinate[PinCoordIndex]).LengthSquared() < 64 * 64)
                {
                    PinCoordinate[PinCoordIndex] = ToPinCoordinate[PinCoordIndex];
                    PinCoordIndex++;
                    if (PinCoordIndex > ToPinCoordinate.Count - 1) PinCoordIndex = 0;
                }
            }
        }

        List<Vector2> Points = new List<Vector2>();
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            DrawUtils.ClockFrame(ref NPC.frameCounter, 15, 4);
            Color recombinationColor = new Color(drawColor.ToVector4() * 0.5f + Color.White.ToVector4() * 0.5f);

            for (int i = 0; i < PinCoordinate.Count; i++)
            {
                Vector2 strpos = NPC.Center + new Vector2(0, 75);
                Vector2 endpos = PinCoordinate[i];
                Vector2 controlPoint = (strpos + endpos) / 2f;
                Vector2 toEndVr = strpos - endpos;
                float Vrleng = toEndVr.Length();
                float connectionSacle = 1.5f;
                int pointNum = (int)(2 * Vrleng / (WdTexture.Mushroom_connection.Height * connectionSacle));

                HcMath.GenerateCurve2(ref Points, strpos, endpos, pointNum, controlPoint, -0.2f);

                if (Points.Count > pointNum) Points = new List<Vector2>();
                for (int j = 0; j < Points.Count - 2; j++)
                {
                    Vector2 vr1 = Points[j];
                    Vector2 vr2 = Points[j + 1];
                    Vector2 toVr = vr2 - vr1;
                    float rot = toVr.ToRotation();
                    float leng = toVr.Length();

                    Main.EntitySpriteDraw(
                    WdTexture.Rhizome,
                    DrawUtils.WDEpos(vr1),
                    DrawUtils.GetRec(WdTexture.Rhizome),
                    recombinationColor,
                    rot + MathHelper.PiOver2,
                    DrawUtils.GetOrig(WdTexture.Rhizome),
                    connectionSacle,
                    SpriteEffects.None,
                    0
                    );

                    Main.EntitySpriteDraw(
                    WdTexture.Rhizome_Men,
                    DrawUtils.WDEpos(vr1),
                    DrawUtils.GetRec(WdTexture.Rhizome_Men),
                    Color.White,
                    rot + MathHelper.PiOver2,
                    DrawUtils.GetOrig(WdTexture.Rhizome_Men),
                    connectionSacle,
                    SpriteEffects.None,
                    0
                    );
                }

                Main.EntitySpriteDraw(
                    WdTexture.Claw,
                    DrawUtils.WDEpos(PinCoordinate[i]),
                    DrawUtils.GetRec(WdTexture.Claw),
                    recombinationColor,
                    NPC.velocity.ToRotation() + MathHelper.PiOver2,
                    DrawUtils.GetOrig(WdTexture.Claw),
                    4f,
                    SpriteEffects.None,
                    0
                    );

                Main.EntitySpriteDraw(
                    WdTexture.Claw_Men,
                    DrawUtils.WDEpos(PinCoordinate[i]),
                    DrawUtils.GetRec(WdTexture.Claw_Men),
                    Color.White * 10f,
                    NPC.velocity.ToRotation() + MathHelper.PiOver2,
                    DrawUtils.GetOrig(WdTexture.Claw_Men),
                    3f,
                    SpriteEffects.None,
                    0
                    );
            }

            Main.EntitySpriteDraw(
                WdTexture.Sporangium,
                DrawUtils.WDEpos(NPC),
                DrawUtils.GetRec(WdTexture.Sporangium,
                (int)NPC.frameCounter, 5),
                recombinationColor,
                NPC.rotation,
                DrawUtils.GetOrig(WdTexture.Sporangium,
                (int)NPC.frameCounter, 5),
                NPC.scale,
                SpriteEffects.None,
                0
                );

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

            FungusDevourerHead.BlueGlowShader(WdTexture.Sporangium_Men, 10, new Vector4(0, 0, 1, 1));
            Main.EntitySpriteDraw(
                WdTexture.Sporangium_Men,
                DrawUtils.WDEpos(NPC),
                DrawUtils.GetRec(WdTexture.Sporangium_Men, (int)NPC.frameCounter, 5),
                drawColor,
                NPC.rotation,
                DrawUtils.GetOrig(WdTexture.Sporangium_Men, (int)NPC.frameCounter, 5),
                NPC.scale,
                SpriteEffects.None,
                0
                );

            Main.spriteBatch.ResetBlendState();



            return false;
        }

        public override void DrawBehind(int index)
        {
            Main.instance.DrawCacheNPCProjectiles.Add(index);
        }
    }
}
