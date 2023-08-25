using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ObjectData;

namespace Wild.Common.WorldGeneration
{
    public static class TileHelper
    {
        /// <summary>
        /// 检测是否为一个背景方块
        /// </summary>
        public static bool TopSlope(this Tile tile)
        {
            byte b = (byte)tile.Slope;
            if (b != 1)
                return b == 2;

            return true;
        }

        /// <summary>
        /// 检测该位置是否存在一个实心的固体方块
        /// </summary>
        public static bool HasSolidTile(this Tile tile)
        {
            return tile.HasTile && Main.tileSolid[tile.TileType] && !Main.tileSolidTop[tile.TileType];
        }

        public static Vector2 FindTopLeft(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            if (tile == null)
                return new Vector2(x, y);
            TileObjectData data = TileObjectData.GetTileData(tile.TileType, 0);
            x -= tile.TileFrameX / 18 % data.Width;
            y -= tile.TileFrameY / 18 % data.Height;
            return new Vector2(x, y);
        }

        /// <summary>
        /// 检测方块的一个矩形区域内是否有实心物块
        /// </summary>
        /// <param name="tileVr">方块坐标</param>
        /// <param name="DetectionL">矩形左</param>
        /// <param name="DetectionR">矩形右</param>
        /// <param name="DetectionD">矩形上</param>
        /// <param name="DetectionS">矩形下</param>
        public static bool TileRectangleDetection(Vector2 tileVr, int DetectionL, int DetectionR, int DetectionD, int DetectionS)
        {
            Vector2 newTileVr;
            for (int x = 0; x < DetectionR - DetectionL; x++)
            {
                for (int y = 0; y < DetectionS - DetectionD; y++)
                {
                    newTileVr = WorldDetection.PTransgressionTile(new Vector2(tileVr.X + x, tileVr.Y + y));
                    if (Main.tile[(int)newTileVr.X, (int)newTileVr.Y].HasSolidTile())
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
