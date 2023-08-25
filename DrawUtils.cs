using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;
using Wild.Common.AuxiliaryMeans;

namespace Wild.Common.DrawTools
{
    /// <summary>
    /// 普通绘制工具
    /// </summary>
    static class DrawUtils
    {
        /// <summary>
        /// 获取与纹理大小对应的矩形框
        /// </summary>
        /// <param name="value">纹理对象</param>
        public static Rectangle GetRec(Texture2D value)
        {
            return new Rectangle(0, 0, value.Width, value.Height);
        }
        /// <summary>
        /// 获取与纹理大小对应的矩形框
        /// </summary>
        /// <param name="value">纹理对象</param>
        /// <param name="Dx">X起点</param>
        /// <param name="Dy">Y起点</param>
        /// <param name="Sx">宽度</param>
        /// <param name="Sy">高度</param>
        /// <returns></returns>
        public static Rectangle GetRec(Texture2D value, int Dx, int Dy, int Sx, int Sy)
        {
            return new Rectangle(Dx, Dy, Sx, Sy);
        }
        /// <summary>
        /// 获取与纹理大小对应的矩形框
        /// </summary>
        /// <param name="value">纹理对象</param>
        /// <param name="frameCounter">帧索引</param>
        /// <param name="frameCounterMax">总帧数，该值默认为1</param>
        /// <returns></returns>
        public static Rectangle GetRec(Texture2D value, int frameCounter, int frameCounterMax = 1)
        {
            int singleFrameY = value.Height / frameCounterMax;
            return new Rectangle(0, singleFrameY * frameCounter, value.Width, singleFrameY);
        }
        /// <summary>
        /// 获取与纹理大小对应的缩放中心
        /// </summary>
        /// <param name="value">纹理对象</param>
        /// <returns></returns>
        public static Vector2 GetOrig(Texture2D value)
        {
            return new Vector2(value.Width, value.Height) * 0.5f;
        }
        /// <summary>
        /// 获取与纹理大小对应的缩放中心
        /// </summary>
        /// <param name="value">纹理对象</param>
        /// <param name="ScaleOrig">整体缩放体积偏移</param>
        /// <returns></returns>
        public static Vector2 GetOrig(Texture2D value, float ScaleOrig)
        {
            return new Vector2(value.Width, value.Height) * ScaleOrig;
        }
        /// <summary>
        /// 获取与纹理大小对应的缩放中心
        /// </summary>
        /// <param name="value">纹理对象</param>
        /// <param name="ScaleX">X方向收缩系数</param>
        /// <param name="ScaleY">Y方向收缩系数</param>
        /// <returns></returns>
        public static Vector2 GetOrig(Texture2D value, float ScaleX, float ScaleY)
        {           
            return new Vector2(value.Width * ScaleX, value.Height * ScaleY);
        }
        /// <summary>
        /// 获取与纹理大小对应的缩放中心
        /// </summary>
        /// <param name="value">纹理对象</param>
        /// <param name="frameCounter">帧索引</param>
        /// <param name="frameCounterMax">总帧数，该值默认为1</param>
        /// <returns></returns>
        public static Vector2 GetOrig(Texture2D value, int frameCounter, int frameCounterMax = 1)
        {
            float singleFrameY = value.Height / frameCounterMax;
            return new Vector2(value.Width * 0.5f, singleFrameY / 2 );
        }
        /// <summary>
        /// 对帧数索引进行走表
        /// </summary>
        /// <param name="frameCounter"></param>
        /// <param name="intervalFrame"></param>
        /// <param name="Maxframe"></param>
        public static void ClockFrame(ref int frameCounter, int intervalFrame, int maxFrame)
        {
            if (Main.fpsCount % intervalFrame == 0) frameCounter++;
            if (frameCounter > maxFrame) frameCounter = 0;
        }
        /// <summary>
        /// 对帧数索引进行走表
        /// </summary>
        /// <param name="frameCounter"></param>
        /// <param name="intervalFrame"></param>
        /// <param name="Maxframe"></param>
        public static void ClockFrame(ref double frameCounter, int intervalFrame, int maxFrame)
        {
            if (Main.fpsCount % intervalFrame == 0) frameCounter++;
            if (frameCounter > maxFrame) frameCounter = 0;
        }
        /// <summary>
        /// 将世界位置矫正为适应屏幕的画布位置
        /// </summary>
        /// <param name="entity">传入目标实体</param>
        /// <returns></returns>
        public static Vector2 WDEpos(Entity entity)
        {
            return AiBehavior.GetEntityCenter(entity) - Main.screenPosition;
        }
        /// <summary>
        /// 将世界位置矫正为适应屏幕的画布位置
        /// </summary>
        /// <param name="pos">绘制目标的世界位置</param>
        /// <returns></returns>
        public static Vector2 WDEpos(Vector2 pos)
        {
            return pos - Main.screenPosition;
        }
        /// <summary>
        /// 获取纹理实例，类型为 Texture2D
        /// </summary>
        /// <param name="texture">纹理路径</param>
        /// <returns></returns>
        public static Texture2D GetT2DValue(string texture)
        {
            return ModContent.Request<Texture2D>(texture).Value;
        }

        /// <summary>
        /// 获取纹理实例，类型为 Asset<Texture2D>
        /// </summary>
        /// <param name="texture">纹理路径</param>
        /// <returns></returns>
        public static Asset<Texture2D> GetT2DAsset(string texture)
        {
            return ModContent.Request<Texture2D>(texture);
        }

        /// <summary>
        /// 任意设置 <see cref=" SpriteBatch "/> 的 <see cref=" BlendState "/>。
        /// </summary>
        /// <param name="spriteBatch">绘制模式</param>
        /// <param name="blendState">要使用的混合状态</param>
        public static void SetBlendState(this SpriteBatch spriteBatch, BlendState blendState)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, blendState, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
        }

        /// <summary>
        /// 将 <see cref="SpriteBatch"/> 的 <see cref="BlendState"/> 重置为典型的 <see cref="BlendState.AlphaBlend"/>。
        /// </summary>
        /// <param name="spriteBatch">绘制模式</param>
        public static void ResetBlendState(this SpriteBatch spriteBatch) => spriteBatch.SetBlendState(BlendState.AlphaBlend);
    }
}
