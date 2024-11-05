using System;
using GameCreator.Runtime.Common;
using UnityEngine;
using Weber.Scripts.Common.Utils;
using Weber.Scripts.Legend.Skill;
using Weber.Scripts.Model;

namespace Weber.Scripts.Legend.Game.UI
{
    public class SkillGroup : MonoBehaviour, ISignalReceiver
    {
        [SerializeField] private SkillCountDown[] _specialSkillCountDowns;
        [SerializeField] private SkillCountDown[] _commonSkillCountDowns;

        private void Awake()
        {
            Signals.Subscribe(this, SignalNames.SKILL_LEARNED);
        }

        private void OnDestroy()
        {
            Signals.Unsubscribe(this, SignalNames.SKILL_LEARNED);
        }

        public void OnReceiveSignal(SignalArgs args)
        {
            if (args.signal == SignalNames.SKILL_LEARNED)
            {
                var battleProp = args.invoker.Get<BattleProp>();
                var skill = battleProp.SkillData;
                if (skill.classType == ClassType.Special)
                {
                    foreach (var skillCountDown in _specialSkillCountDowns)
                    {
                        if (!skillCountDown.Actived)
                        {
                            skillCountDown.Active(battleProp);
                            break;
                        }
                    }
                }
                else
                {
                    foreach (var skillCountDown in _commonSkillCountDowns)
                    {
                        if (!skillCountDown.Actived)
                        {
                            skillCountDown.Active(battleProp);
                            break;
                        }
                    }
                }
            }
        }
    }
}