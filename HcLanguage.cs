using Terraria.Localization;
using Terraria.ModLoader;

namespace Wild.Common.AuxiliaryMeans
{
    public static class Languages
    {
        /// <summary>
        /// 一个根据语言选项返回字符的方法
        /// </summary>
        public static string Translation(string Cha, string Eng)
        {
            return Language.ActiveCulture.Name == "zh-Hans" ? Cha : Eng;
        }
    }
}
