using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Wild.Asset;
using Wild.Common;
using Wild.Common.AuxiliaryMeans;
using Wild.Common.DrawTools;
using Wild.Content.NPCs.FungusDevourers;

namespace Wild.Content.Projectiles
{
    public class MushroomSpores : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 5;
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 16;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.scale = 1;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 180;
            CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override string Texture => WdConstant.ProJ_FDers + "MushroomSpores";

        public int ThisTimeValue
        {
            set => Projectile.ai[1] = value;
            get => (int)Projectile.ai[1];
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.rotation = MathHelper.ToRadians(HcMath.HcRandom.Next(0, 360));
            Projectile.alpha = HcMath.HcRandom.Next(200, 355);
            Projectile.scale = 0.7f + (float)HcMath.HcRandom.NextDouble() * 0.5f;
            Projectile.frameCounter = HcMath.HcRandom.Next(0, 5);
        }

        public override void AI()
        {
            ThisTimeValue++;
            Projectile.rotation = MathHelper.ToRadians(ThisTimeValue * Projectile.velocity.LengthSquared() / 80f);
            Projectile.scale -= 0.01f;
            Projectile.alpha -= 1;
            if (Projectile.scale < 0.1f) Projectile.Kill();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            float Alp = Projectile.alpha / 255f;
            DrawUtils.ClockFrame(ref Projectile.frameCounter, 10, 4);
            FungusDevourerHead.BlueGlowShader(WdTexture.MushroomSpores, 10f, new Vector4(0, 0, 1, 1));           

            

            Main.EntitySpriteDraw(
                WdTexture.MushroomSpores,
                DrawUtils.WDEpos(Projectile),
                DrawUtils.GetRec(WdTexture.MushroomSpores, Projectile.frameCounter, 5),
                Color.White * Alp,
                Projectile.rotation,
                DrawUtils.GetOrig(WdTexture.MushroomSpores, Projectile.frameCounter, 5),
                Projectile.scale,
                SpriteEffects.None,
                0
                );
            return false;
        }
    }
}
