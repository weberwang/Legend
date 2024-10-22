using UnityEngine;
using Weber.Scripts.Domain;

namespace Weber.Scripts.Legend.Unit
{
    public class Hero : CharacterUnit
    {
        protected override void OnCreate()
        {
            Character.IsPlayer = true;
        }

        private void UpdateAttributes()
        {
            foreach (var instanceHeroSkillValue in HeroManager.Instance.HeroSkillValues)
            {
                traits.RuntimeAttributes.Get(instanceHeroSkillValue.Key).Value += instanceHeroSkillValue.Value;
            }
        }
    }
}