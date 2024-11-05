using Weber.Scripts.Model;

namespace Weber.Scripts.Common.Utils
{
    public class Utils
    {
        public static string GetSkillRarity(LuckConfig.SkillRarity rarity)
        {
            switch (rarity)
            {
                case LuckConfig.SkillRarity.Common:
                    return "普通";
                case LuckConfig.SkillRarity.Rare:
                    return "罕见";
                case LuckConfig.SkillRarity.Epic:
                    return "史诗";
                case LuckConfig.SkillRarity.Legendary:
                    return "传说";
                case LuckConfig.SkillRarity.Mythical:
                    return "神话";
                default:
                    return "未知";
            }
        }
    }
}