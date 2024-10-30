using System;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.Stats;
using UnityEngine;
using UnityEngine.UI;

namespace Weber.Scripts.Model
{
    [CreateAssetMenu(fileName = "AllHeroData", menuName = "Weber/Character/AllHeroData", order = 0)]
    public class AllHeroData : ScriptableObject
    {
        public HeroData[] heroDatas;
    }

    [Serializable]
    public class HeroData
    {
        public UniqueID ID = new UniqueID(UniqueID.GenerateID());
        public string heroName;
        public Sprite icon;
        [TextArea] public string description;
        public SkillEffectStatValue[] skillValues;
    }
}