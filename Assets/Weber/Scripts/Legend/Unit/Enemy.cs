using GameCreator.Runtime.VisualScripting;
using UnityEngine;

namespace Weber.Scripts.Legend.Unit
{
    public class Enemy : CharacterUnit
    {
        protected override void OnCreate()
        {
            Character.IsPlayer = false;
        }
    }
}