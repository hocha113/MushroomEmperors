using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Wild.Common.AuxiliaryMeans;
using Wild.Common.HcGlobalProjectile;
using Wild.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Wild.Asset;
using Wild.Common.DrawTools;
using Wild.Asset.Effects;

namespace Wild.Content.Projectiles.FungusProjectile
{
    public class GiantFungusTreeBomb_Short : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 16;
        }

        public override void SetDefaults()
        {
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.scale = 1;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 80;
            CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override string Texture => WdConstant.ProJ_FDers + "GiantFungusTreeBomb_Short";
        public override string GlowTexture => Texture + "_Men";
        public override string Name => Languages.Translation("巨型真菌矮树炸弹", "Giant Fungus Short Tree Bomb");

        public int ProJindex = -1;

        public int Status
        {
            set => Projectile.ai[0] = value;
            get => (int)Projectile.ai[0];
        }

        public int ThisTimeValue
        {
            set => Projectile.ai[1] = value;
            get => (int)Projectile.ai[1];
        }

        public void OnSpanAction()
        {
            if (Main.netMode != NetmodeID.MultiplayerClient) AiBehavior.LoadList(ref ProJwhoAmI.GiantFungusTreeBombShortList, Projectile, ref ProJindex);
        }

        public void OnKillAction()
        {
            if (Main.netMode != NetmodeID.MultiplayerClient) AiBehavior.UnLoadList(ref ProJwhoAmI.GiantFungusTreeBombShortList, Projectile, ref ProJindex);

            for (int j = 0; j < 3; j++)
            {
                for (int i = 0; i < 64; i++)
                {
                    Vector2 vr = (MathHelper.TwoPi / 64 * i).ToRotationVector2() * 13f * j;
                    int dust = Dust.NewDust(Projectile.Center, 1, 1, DustID.MushroomSpray, vr.X, vr.Y);
                    Dust newDust = Main.dust[dust];
                    newDust.scale = 1.5f + j;
                    newDust.noGravity = true;
                }
            }
        }

        public override void Kill(int timeLeft)
        {
            OnKillAction();
        }

        public override void AI()
        {
            if (Status == 0)
            {
                OnSpanAction();
                Status = 1;
            }

            Player target = AiBehavior.FindingTarget(Projectile, -1);
            Vector2 RtargVr = (MathHelper.TwoPi / ProJwhoAmI.GiantFungusTreeBombShortList.GetIntListCount() * ProJindex).ToRotationVector2() * 160f;
            if (AiBehavior.PlayerAlive(target) == false)
            {
                Projectile.timeLeft = 0;
                Projectile.active = false;
            }

            ThisTimeValue++;
            Projectile.timeLeft = 10;

            if (Status == 1)
            {
                Projectile.scale += 0.02f;
                AiBehavior.EntityToRot(Projectile, Projectile.velocity.ToRotation() + MathHelper.ToRadians(90), 0.15f);

                if (Projectile.scale >= 2f)
                {
                    Status = 2;
                    ThisTimeValue = 0;
                }
            }

            if (Status == 2)
            {
                AiBehavior.ChasingBehavior(Projectile, target.Center + RtargVr, 15f, 16f);
                AiBehavior.EntityToRot(Projectile, Projectile.velocity.ToRotation() + MathHelper.ToRadians(90), 0.15f);
                if (ThisTimeValue >= 180 || AiBehavior.GetEntityDgSquared(Projectile, target) < 560 * 560)
                {
                    Status = 3;
                    ThisTimeValue = 0;
                }
            }

            if (Status == 3)
            {
                AiBehavior.ChasingBehavior(Projectile, target.Center + RtargVr, 15f + Projectile.scale * 3f, 16f);
                Projectile.rotation += MathHelper.ToRadians(7f);
                Projectile.scale += 0.005f;
                if (Projectile.scale >= 3f || (target.Center + RtargVr - Projectile.Center).LengthSquared() < 16 * 16)
                {
                    Projectile.Kill();
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.EntitySpriteDraw(
                WdTexture.GiantFungusTreeBomb_Short, 
                DrawUtils.WDEpos(Projectile.Center), 
                DrawUtils.GetRec(WdTexture.GiantFungusTreeBomb_Short), 
                lightColor, 
                Projectile.rotation, 
                DrawUtils.GetOrig(WdTexture.GiantFungusTreeBomb_Short), 
                Projectile.scale, 
                SpriteEffects.None, 
                0
                );

            for (int i = 0; i < Projectile.oldPos.Length - 1; i++)
            {
                Vector2 oldPos = Projectile.oldPos[i] + (Projectile.Center - Projectile.position);
                float oldRot = Projectile.oldRot[i];
                float oldGraduallyValue = i / (float)Projectile.oldPos.Length;
                Main.EntitySpriteDraw(
                    WdTexture.GiantFungusTreeBomb_Short, 
                    DrawUtils.WDEpos(oldPos), 
                    DrawUtils.GetRec(WdTexture.GiantFungusTreeBomb_Short), 
                    lightColor * (0.5f - oldGraduallyValue / 2f) * (1 - oldGraduallyValue), 
                    oldRot, 
                    DrawUtils.GetOrig(WdTexture.GiantFungusTreeBomb_Short), 
                    (1 - oldGraduallyValue) * Projectile.scale, 
                    SpriteEffects.None, 
                    0
                    );
            }

            Main.spriteBatch.ResetBlendState();

            Effect blueGlowShader = WildEffectsRegistry.BlueGlowShader.GetShader().Shader;
            blueGlowShader.Parameters["glowIntensity"].SetValue(10f); // 设置发光强度
            blueGlowShader.CurrentTechnique.Passes["GetBlueGlow"].Apply();

            for (int i = 0; i < Projectile.oldPos.Length - 1; i++)
            {
                Vector2 oldPos = Projectile.oldPos[i] + (Projectile.Center - Projectile.position);
                float oldRot = Projectile.oldRot[i];
                float oldGraduallyValue = i / (float)Projectile.oldPos.Length;
                Main.EntitySpriteDraw(
                    WdTexture.GiantFungusTreeBomb_Short_Men, 
                    DrawUtils.WDEpos(oldPos), 
                    DrawUtils.GetRec(WdTexture.GiantFungusTreeBomb_Short), 
                    Color.White * (0.5f - oldGraduallyValue / 2f) * (1 - oldGraduallyValue), 
                    oldRot, 
                    DrawUtils.GetOrig(WdTexture.GiantFungusTreeBomb_Short), 
                    (1 - oldGraduallyValue) * Projectile.scale, 
                    SpriteEffects.None, 
                    0
                    );
            }

            Main.EntitySpriteDraw(
                WdTexture.GiantFungusTreeBomb_Short_Men, 
                DrawUtils.WDEpos(Projectile.Center), 
                DrawUtils.GetRec(WdTexture.GiantFungusTreeBomb_Short), 
                Color.White, 
                Projectile.rotation, 
                DrawUtils.GetOrig(WdTexture.GiantFungusTreeBomb_Short), 
                Projectile.scale, 
                SpriteEffects.None, 
                0
                );
            Main.spriteBatch.ResetBlendState();

            return false;
        }
    }
}
