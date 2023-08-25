using Microsoft.Xna.Framework;
using Terraria;

namespace Wild.Common.WorldGeneration
{
    public static class WorldDetection
    {
        /// <summary>
        /// 检测是否为越界方块
        /// </summary>
        public static Tile ParanoidTileRetrieval(int x, int y)
        {
            if (!WorldGen.InWorld(x, y, 0))
            {
                return default(Tile);
            }
            return ((Tilemap)(Main.tile))[x, y];
        }

        /// <summary>
        /// 将可能越界的方块坐标收值为非越界坐标
        /// </summary>
        public static Vector2 PTransgressionTile(Vector2 TileVr, int L = 0, int R = 0, int D = 0, int S = 0)
        {
            if (TileVr.X > Main.maxTilesX - R)
            {
                TileVr.X = Main.maxTilesX - R;
            }
            if (TileVr.X < 0 + L)
            {
                TileVr.X = 0 + L;
            }
            if (TileVr.Y > Main.maxTilesY - S)
            {
                TileVr.Y = Main.maxTilesY - S;
            }
            if (TileVr.Y < 0 + D)
            {
                TileVr.Y = 0 + D;
            }
            return new Vector2(TileVr.X, TileVr.Y);
        }
    }
}
