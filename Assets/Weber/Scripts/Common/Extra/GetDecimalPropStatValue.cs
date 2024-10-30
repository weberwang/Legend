using System;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.Stats;
using UnityEngine;
using Weber.Scripts.Legend.Unit;
using Weber.Scripts.Model;

namespace Weber.Scripts.Common.Extra
{
    [Title("Skill Stat Value")]
    [Category("Stats/Skill Stat Value")]
    [Image(typeof(IconStat), ColorTheme.Type.Red)]
    [Description("The Stat value of a character skill's stat")]
    [Serializable]
    public class GetDecimalPropStatValue : PropertyTypeGetDecimal
    {
        [SerializeField] private SkillData _skillData;
        [SerializeField] private Stat _stat;

        public override double Get(Args args)
        {
            var characterUnit = args.Self.Get<CharacterUnit>();
            var battleProp = characterUnit.GetBattleProp(_skillData.ID);
            var stat = battleProp?.GetRuntimeStatData(_stat.ID.String);
            if (stat is null) return 1;
            var value = stat.Value;
            if (value == 0) return 1;
            return (stat.Base / value);
        }

        public static PropertyGetDecimal Create => new PropertyGetDecimal(
            new GetDecimalStatValue()
        );

        public override string String => $"Self {_skillData.skillName}";
    }
}