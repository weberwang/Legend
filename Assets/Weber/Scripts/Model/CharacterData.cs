using GameCreator.Runtime.Common;
using UnityEngine;

namespace Weber.Scripts.Model
{
    public class CharacterData
    {
        public int id;
        public string name;
        public Sprite icon;
        [TextArea] public string description;
        public SkillEffectStatValue[] skillValues;
        public float radius = 0.5f;
        public AnimationClip idle;
        public AnimationClip run;
        public AnimationClip hurt;
        public AnimationClip death;
    }
}