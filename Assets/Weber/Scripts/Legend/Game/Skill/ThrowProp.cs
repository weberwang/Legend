using GameCreator.Runtime.Common;
using UnityEngine;

namespace Weber.Scripts.Legend.Skill
{
    public class ThrowProp : BattleProp
    {
        [SerializeField] private SpellMuzzle[] _muzzles;
        [SerializeField] private GameObject _bullet;
        [SerializeField] private LayerMaskValue _bulletLayer = new LayerMaskValue();
    }
}