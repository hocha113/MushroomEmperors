using System.Collections.Generic;
using Wild.Common.AuxiliaryMeans;

namespace Wild.Common.HcGlobalNPC
{
    public class NPCwhoAmI
    {
        public static int CrimsonStarBoss = -1;
        public static int MechanicalStar = -1;
        public static int FungusDevourer = -1;

        public static List<int> FallingTwinStarsList = new List<int>();
        public static List<int> FungusDevourerList = new List<int>();
        public static List<int> FungusDevourerBodyList = new List<int>();
        public static List<int> MushroomEmperorList = new List<int>();
        public static List<int> MushroomEmperorBodyList = new List<int>();
        public static List<int> LifeTreeList = new List<int>();

        /// <summary>
        /// 清除一次集合的-1元素
        /// </summary>
        public static void SweepLoadList()
        {
            HcMath.SweepLoadLists(ref  FallingTwinStarsList);
        }
        /// <summary>
        /// 重载所有集合
        /// </summary>
        public static void UnLoad()
        {
            CrimsonStarBoss = -1;
            MechanicalStar = -1;
            FungusDevourer = -1;

            FallingTwinStarsList = new List<int>();
            FungusDevourerList = new List<int>();
            FungusDevourerBodyList = new List<int>();
            MushroomEmperorList = new List<int>();
            MushroomEmperorBodyList = new List<int>();
            LifeTreeList = new List<int>();
        }
    }
}
