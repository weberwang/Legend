using System;
using UnityEngine;

namespace Weber.Scripts.Legend.Skill
{
    public class InvincibilityProp : BattleProp
    {
        public override void OnActive()
        {
            base.OnActive();
            CharacterUnit.Character.Combat.Invincibility.Set(Convert.ToSingle(CountDown.Duration));
        }
    }
}