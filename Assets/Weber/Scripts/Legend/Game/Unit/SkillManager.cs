using System;
using System.Collections.Generic;
using GameCreator.Runtime.Common;
using UnityEngine;
using UnityEngine.Serialization;
using Weber.Scripts.Common.Utils;
using Weber.Scripts.Domain;
using Weber.Scripts.Legend.Skill;
using Weber.Scripts.Model;
using Math = Unity.Physics.Math;
using Random = UnityEngine.Random;

namespace Weber.Scripts.Legend.Unit
{
    [RequireComponent(typeof(CharacterUnit))]
    public class SkillManager : MonoBehaviour
    {
        [SerializeField] private SkillData _initSkillData;

        //职业技能
        [SerializeField] private AttackSkillData[] _specialAttackSkillDatas;

        //公共技能
        [SerializeField] private SkillData[] _commonSkillDatas;
        [SerializeField] private SkillData[] _baseSkillDatas;

        public SkillData InitSkillData => _initSkillData;

        private List<SkillData> _learnedSkills = new List<SkillData>();

        private List<SkillData> _learnedSpecialSkills = new List<SkillData>();
        private List<SkillData> _learnedCommonSkills = new List<SkillData>();


        private CharacterUnit _characterUnit;

        private void Awake()
        {
            Load();
            _characterUnit = GetComponent<CharacterUnit>();
        }

        public void InitialSkill()
        {
            if (_initSkillData is not null)
            {
                LearnSkill(_initSkillData.ID.ToString());
            }
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            Save();
        }

        private void OnApplicationQuit()
        {
            Save();
        }

        //学习技能并初始化技能
        public void LearnSkill(string skillID, SkillEffectStatValue skillEffectStatValue = null)
        {
            var skillData = GetSkillData(skillID);
            if (!skillData) return;
            if (!_learnedSkills.Contains(skillData))
            {
                if (skillData.GetType() == typeof(AttackSkillData))
                {
                    (skillData as AttackSkillData).MergeHitEffect();
                }

                switch (skillData.classType)
                {
                    case ClassType.Common:
                        _learnedCommonSkills.Add(skillData);
                        break;
                    case ClassType.Special:
                        _learnedSpecialSkills.Add(skillData);
                        break;
                }

                _learnedSkills.Add(skillData);
            }

            skillData.Upgrade(_characterUnit, skillEffectStatValue);
        }

        private SkillData GetSkillData(string skillID)
        {
            if (_initSkillData.ID.ToString() == skillID) return _initSkillData;
            for (int i = 0; i < _specialAttackSkillDatas.Length; i++)
            {
                if (_specialAttackSkillDatas[i].ID.ToString() == skillID)
                {
                    return _specialAttackSkillDatas[i];
                }
            }

            for (int i = 0; i < _commonSkillDatas.Length; i++)
            {
                if (_commonSkillDatas[i].ID.ToString() == skillID)
                {
                    return _commonSkillDatas[i];
                }
            }

            for (int i = 0; i < _baseSkillDatas.Length; i++)
            {
                if (_baseSkillDatas[i].ID.ToString() == skillID)
                {
                    return _baseSkillDatas[i];
                }
            }

            return null;
        }

        private void Save()
        {
        }

        private void Load()
        {
        }

        public UpgradeSkillData[] ChoiceSkillDatas()
        {
            SkillData[] skillDatas = null;
            var level = Convert.ToInt32(_characterUnit.GetRuntimeStatValue(TraitsID.TRAITS_LEVEL));
            var learnSkillData = HeroManager.Instance.LearnSkillData;

            if (Array.IndexOf(learnSkillData.specialSkillLevel, level) >= 0)
            {
                //选择职业技能
                skillDatas = SelectRandomElements(_specialAttackSkillDatas, 3, _learnedSpecialSkills);
            }
            else if (Array.IndexOf(learnSkillData.commonSkillLevel, level) >= 0)
            {
                //选择公共技能
                skillDatas = SelectRandomElements(_commonSkillDatas, 3, _learnedCommonSkills);
            }
            else
            {
                var maxSkillCount = Mathf.Min(_learnedSkills.Count, 2);
                var skills = SelectRandomElements(_learnedSkills.ToArray(), maxSkillCount);
                var skillList = new List<SkillData>(skills);
                var lastCount = 3 - maxSkillCount;
                var last = SelectRandomElements(_baseSkillDatas, lastCount);
                skillList.AddRange(last);
                skillDatas = skillList.ToArray();
            }

            List<UpgradeSkillData> upgradeSkillDatas = new List<UpgradeSkillData>();
            var statLevels = HeroManager.Instance.StatLevelConfig.statLevels;
            for (int i = 0; i < skillDatas.Length; i++)
            {
                var skillData = skillDatas[i];
                //第一次学习该技能
                if (skillData.classType != ClassType.Base && skillData.Level == 0)
                {
                    upgradeSkillDatas.Add(new UpgradeSkillData(skillData, LuckConfig.SkillRarity.Common, null));
                    continue;
                }

                var skillEffectStatValue = skillData.ChoiceSkillEffectStatValue().Clone();
                var skillRarity = GetSkillRarityByLuck(_characterUnit.GetRuntimeStatValue(TraitsID.TRAITS_LUCK));
                var found = false;
                foreach (var statLevel in statLevels)
                {
                    if (statLevel.stat.ID.String == skillEffectStatValue.stat.ID.String)
                    {
                        foreach (var statLevelRarityValue in statLevel.rarityValues)
                        {
                            if (statLevelRarityValue.rarity == skillRarity)
                            {
                                skillEffectStatValue.value = statLevelRarityValue.value;
                                found = true;
                                break;
                            }
                        }
                    }

                    if (found)
                    {
                        break;
                    }
                }

                upgradeSkillDatas.Add(new UpgradeSkillData(skillData, skillRarity, skillEffectStatValue));
            }

            upgradeSkillDatas.Shuffle();
            return upgradeSkillDatas.ToArray();
        }

        public T[] SelectRandomElements<T>(T[] array, int maxSelection, List<T> existingElements = null)
        {
            if (maxSelection > array.Length)
                throw new ArgumentException("maxSelection cannot be greater than array length.");

            // 随机生成一个数量，范围从 1 到 maxSelection
            // int selectionCount = Random.Range(1, maxSelection + 1);

            // 使用 List 记录选择的索引，避免重复选择
            List<int> selectedIndices = new List<int>();

            while (selectedIndices.Count < maxSelection)
            {
                int randomIndex = Random.Range(0, array.Length);
                if (!selectedIndices.Contains(randomIndex))
                {
                    if (existingElements is not null)
                    {
                        if (!existingElements.Contains(array[randomIndex]))
                        {
                            selectedIndices.Add(randomIndex);
                        }
                    }
                    else
                    {
                        selectedIndices.Add(randomIndex);
                    }
                }
            }

            // 根据索引选择对应的元素并返回
            T[] selectedElements = new T[maxSelection];
            for (int i = 0; i < maxSelection; i++)
            {
                selectedElements[i] = array[selectedIndices[i]];
            }

            return selectedElements;
        }

        public LuckConfig.SkillRarity GetSkillRarityByLuck(float luck)
        {
            foreach (var threshold in HeroManager.Instance.LuckConfig.luckThresholds)
            {
                if (luck >= threshold.minLuck && luck <= threshold.maxLuck)
                {
                    float randomValue = Random.value;
                    float cumulativeChance = 0f;

                    foreach (var rarityChance in threshold.rarityChances)
                    {
                        cumulativeChance += rarityChance.chance;
                        if (randomValue <= cumulativeChance)
                        {
                            return rarityChance.rarity;
                        }
                    }
                }
            }

            // 默认返回普通品质
            return LuckConfig.SkillRarity.Common;
        }
    }
}