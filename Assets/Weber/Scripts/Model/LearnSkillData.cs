using UnityEngine;

namespace Weber.Scripts.Model
{
    [CreateAssetMenu(fileName = "LearnSkillData", menuName = "Weber/Skill/LearnSkillData", order = 0)]
    public class LearnSkillData : ScriptableObject
    {
        public int[] specialSkillLevel;
        public int[] commonSkillLevel;
    }
}