using UnityEngine;
using Weber.Scripts.Common.Utils;

namespace Weber.Scripts.Legend.Skill
{
    public class RestoreHealthProp : BattleProp
    {
        public override void OnActive()
        {
            base.OnActive();
            var health = GetRuntimeStatDataValue(TraitsID.TRAITS_HEALTH_REGENERATION);
            CharacterUnit.RestoreHealth(health);
        }
    }
}