using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Wild.Asset;
using Wild.Asset.Effects;
using Wild.Common;
using Wild.Common.AuxiliaryMeans;
using Wild.Common.DrawTools;
using Wild.Common.HcGlobalProjectile;
using Wild.Content.NPCs.FungusDevourers;

namespace Wild.Content.Projectiles.FungusProjectile
{
    public class GiantFungusTreeBomb_Leng : ModProjectile
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
            Projectile.scale = 2;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 180;
            CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override string Texture => WdConstant.ProJ_FDers + "GiantFungusTreeBomb_Leng";
        public override string GlowTexture => Texture + "_Men";
        public override string Name => Languages.Translation("巨型真菌高树炸弹", "Giant Fungus Leng Tree Bomb");

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
            if (Main.netMode != NetmodeID.MultiplayerClient)  AiBehavior.LoadList(ref ProJwhoAmI.GiantFungusTreeBombLengList, Projectile, ref ProJindex);
        }

        public void OnKillAction()
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)  AiBehavior.UnLoadList(ref ProJwhoAmI.GiantFungusTreeBombLengList, Projectile, ref ProJindex);

            for (int i = 0; i <= 4; i++)
            {
                Vector2 Vr = (MathHelper.TwoPi / 4 * i).ToRotationVector2() * 7f;
                Projectile.NewProjectile(AiBehavior.GetEntitySource_Parent(Projectile), Projectile.Center, Vr, ModContent.ProjectileType<GiantFungusTreeBomb_Short>(), Projectile.damage, 0f, Main.myPlayer);
            }

            for (int j = 0; j < 4; j++)
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
            Vector2 RtargVr = (MathHelper.TwoPi / ProJwhoAmI.GiantFungusTreeBombLengList.GetIntListCount() * ProJindex).ToRotationVector2() * 160f;
            if (AiBehavior.PlayerAlive(target) == false)
            {
                Projectile.timeLeft = 0;
                Projectile.active = false;
            }

            ThisTimeValue++;

            if (Status == 1)
            {
                Projectile.rotation = MathHelper.ToRadians(ThisTimeValue * Projectile.velocity.Length());
                AiBehavior.ChasingBehavior(Projectile, target.Center + RtargVr, 17f, 16f);
            }

            if (AiBehavior.GetEntityDgSquared(Projectile, target) < 180 * 180)
            {
                Projectile.Kill();
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.EntitySpriteDraw(WdTexture.GiantFungusTreeBomb_Leng, DrawUtils.WDEpos(Projectile.Center), DrawUtils.GetRec(WdTexture.GiantFungusTreeBomb_Leng), new Color( lightColor.ToVector3() * 0.5f + Color.Blue.ToVector3() ), Projectile.rotation, DrawUtils.GetOrig(WdTexture.GiantFungusTreeBomb_Leng), Projectile.scale, SpriteEffects.None, 0);

            for (int i = 0; i < Projectile.oldPos.Length - 1; i++)
            {
                Vector2 oldPos = Projectile.oldPos[i] + (Projectile.Center - Projectile.position);
                float oldRot = Projectile.oldRot[i];
                float oldGraduallyValue = i / (float)Projectile.oldPos.Length;
                Main.EntitySpriteDraw(WdTexture.GiantFungusTreeBomb_Leng, DrawUtils.WDEpos(oldPos), DrawUtils.GetRec(WdTexture.GiantFungusTreeBomb_Leng), lightColor * (0.5f - oldGraduallyValue / 2f) * (1 - oldGraduallyValue), oldRot, DrawUtils.GetOrig(WdTexture.GiantFungusTreeBomb_Leng), Projectile.scale, SpriteEffects.None, 0);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

            FungusDevourerHead.BlueGlowShader(WdTexture.GiantFungusTreeBomb_Leng_Men, 10, new Vector4(0, 0, 0, 1));
            Main.EntitySpriteDraw(WdTexture.GiantFungusTreeBomb_Leng_Men, DrawUtils.WDEpos(Projectile.Center), DrawUtils.GetRec(WdTexture.GiantFungusTreeBomb_Leng), Color.White, Projectile.rotation, DrawUtils.GetOrig(WdTexture.GiantFungusTreeBomb_Leng), Projectile.scale, SpriteEffects.None, 0);

            Main.spriteBatch.ResetBlendState();

            return false;
        }
    }
}
