using System;
using GameCreator.Runtime.Stats;
using UnityEngine;
using Weber.Scripts.Model;

namespace Weber.Scripts.Legend.Game.Items
{
    public class XPDropItem : DropItem
    {
        protected override void OnHeroPicked()
        {
            _hero.AddExp(Convert.ToInt32(_value));
        }
    }
}