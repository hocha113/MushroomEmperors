using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;
using Wild.Asset;
using Wild.Common.AuxiliaryMeans;
using Wild.Common.DrawTools;
using Wild.Common.HcGlobalNPC;
using Wild.Content.NPCs.FungusDevourers;

namespace Wild.Content.Skys
{
    public class MushroomEmperorSky : CustomSky
    {
        private bool isActive = false;
        private float intensity = 0f;

        public override void Update(GameTime gameTime)
        {
            const float increment = 0.01f;
            Main.bgStyle = 32;

            if (NPCwhoAmI.MushroomEmperorBodyList.GetIntListCount() > 0)
            {
                intensity += increment;
                if (intensity > 1f)
                {
                    intensity = 1f;
                }
            }
            else
            {
                intensity -= increment;
                if (intensity < 0f)
                {
                    intensity = 0f;
                    Deactivate();
                }
            }
        }

        private int ScreenDownsIndex = 0;
        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            Color lowBlue = new Color(0.107f, 0f, 0.255f) * 1.5f;

            if (maxDepth >= 0 && minDepth < 0)
            {
                ScreenDownsIndex++;

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(0, BlendState.AlphaBlend, SamplerState.AnisotropicWrap, null, null, null, Main.UIScaleMatrix);
                
                spriteBatch.Draw(WdTexture.Extra_193, Vector2.Zero, new Rectangle(ScreenDownsIndex, ScreenDownsIndex, Main.screenWidth, Main.screenHeight), lowBlue * intensity, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.UIScaleMatrix);

                spriteBatch.Draw(WdTexture.MushroomTreeSky2, Vector2.Zero, DrawUtils.GetRec(WdTexture.MushroomTreeSky2), lowBlue * intensity, 0f, new Vector2(0, 0), 2f, SpriteEffects.None, 0);
                spriteBatch.Draw(WdTexture.MushroomTreeSky3, new Vector2(0, 300), DrawUtils.GetRec(WdTexture.MushroomTreeSky3), lowBlue * intensity, 0f, new Vector2(0, 0), 2f, SpriteEffects.None, 0);
            }
        }

        public override float GetCloudAlpha()
        {
            return 1f - intensity;
        }

        public override void Activate(Vector2 position, params object[] args)
        {
            isActive = true;
        }

        public override void Deactivate(params object[] args)
        {
            isActive = false;
        }

        public override void Reset()
        {
            isActive = false;
        }

        public override bool IsActive()
        {
            return isActive;
        }

        public override Color OnTileColor(Color inColor)
        {
            return new Color(Vector4.Lerp(new Vector4(0.1f, 0.2f, 0.7f, 0.5f), new Vector4(0.6f, 1f, 1f, 1f), 1f - intensity));
        }
    }
}
