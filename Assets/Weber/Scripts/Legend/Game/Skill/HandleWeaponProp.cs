﻿using Cysharp.Threading.Tasks;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.Melee;
using GameCreator.Runtime.Shooter;
using UnityEngine;
using Weber.Scripts.Legend.Unit;
using Weber.Scripts.Model;

namespace Weber.Scripts.Legend.Skill
{
    public class HandleWeaponProp : BattleProp
    {
        [SerializeField] private MeleeKey _meleeKey;

        private ShooterStance _shooterStance;

        private MeleeStance _meleeStance;

        private MeleeWeapon _meleeWeapon;
        private ShooterWeapon _shooterWeapon;


        protected async override void OnSetTarget()
        {
            await CharacterUnit.Character.Combat.Equip(SkillData.weaponAsset, gameObject, new Args(CharacterUnit.gameObject));
            OnEquip();
        }

        public override void OnDeactive()
        {
        }

        public override void OnActive()
        {
            base.OnActive();
            AutoAttack();
        }

        private void OnEquip()
        {
            var weapon = SkillData.weaponAsset;
            if (weapon.GetType() == typeof(ShooterWeapon))
            {
                _shooterWeapon = weapon as ShooterWeapon;
                _shooterStance = CharacterUnit.Character.Combat.RequestStance<ShooterStance>();
            }
            else if (weapon.GetType() == typeof(MeleeWeapon))
            {
                _meleeWeapon = weapon as MeleeWeapon;
                _meleeStance = CharacterUnit.Character.Combat.RequestStance<MeleeStance>();
            }
        }


        private void AutoAttack()
        {
            if (_shooterWeapon is not null)
            {
                AutoShot();
            }

            if (_meleeWeapon is not null)
            {
                AutoMelee();
            }
        }

        private async void AutoShot()
        {
            _shooterStance.PullTrigger(_shooterWeapon);
            await UniTask.Yield();
            _shooterStance.ReleaseTrigger(_shooterWeapon);
        }

        private void AutoMelee()
        {
            _meleeStance.InputExecute(_meleeKey);
        }
    }
}