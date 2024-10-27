using System;
using GameCreator.Runtime.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Weber.Scripts.Model
{
    [Serializable]
    public class HeroData
    {
        public UniqueID ID = new UniqueID(UniqueID.GenerateID());
        public string heroName;
        public Sprite icon;
        [TextArea] public string description;
        public SkillEffectStatValue[] skillValues;
    }

    [CreateAssetMenu(fileName = "AllHeroData", menuName = "Weber/AllHeroData", order = 0)]
    public class AllHeroData : MonoBehaviour
    {
        public HeroData[] heroDatas;
    }
}