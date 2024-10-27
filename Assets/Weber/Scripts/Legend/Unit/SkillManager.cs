using System;
using System.Collections.Generic;
using GameCreator.Runtime.Common;
using UnityEngine;
using UnityEngine.Serialization;
using Weber.Scripts.Domain;
using Weber.Scripts.Legend.Skill;
using Weber.Scripts.Model;
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
            LearnSkill(_initSkillData.ID.ToString());
            for (int i = 0; i < _commonSkillDatas.Length; i++)
            {
                LearnSkill(_commonSkillDatas[i].ID.ToString());
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
                skillData.Learn(_characterUnit);
            }
            else
            {
                skillData.Upgrade(_characterUnit, skillEffectStatValue);
            }
        }

        private SkillData GetSkillData(string skillID)
        {
            if (_initSkillData.ID.Equals(skillID)) return _initSkillData;
            for (int i = 0; i < _specialAttackSkillDatas.Length; i++)
            {
                if (_specialAttackSkillDatas[i].ID.Equals(skillID))
                {
                    return _specialAttackSkillDatas[i];
                }
            }

            for (int i = 0; i < _commonSkillDatas.Length; i++)
            {
                if (_commonSkillDatas[i].ID.Equals(skillID))
                {
                    return _commonSkillDatas[i];
                }
            }

            for (int i = 0; i < _baseSkillDatas.Length; i++)
            {
                if (_baseSkillDatas[i].ID.Equals(skillID))
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
            var level = _characterUnit.GetAttribute(Constants.TRAITS_LEVEL);
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
                var baseIndex = Random.Range(0, _baseSkillDatas.Length);
                var skills = SelectRandomElements(_learnedSkills.ToArray(), 2);
                var skillList = new List<SkillData>(skills);
                skillList.AddRange(skills);
                skillList.Add(_baseSkillDatas[baseIndex]);
                //打乱skillList顺序
                skillList.Shuffle();
                skillDatas = skillList.ToArray();
            }

            UpgradeSkillData[] upgradeSkillDatas = new UpgradeSkillData[skillDatas.Length];
            var statLevels = HeroManager.Instance.StatLevelConfig.statLevels;
            for (int i = 0; i < skillDatas.Length; i++)
            {
                var skillData = skillDatas[i];
                var skillEffectStatValue = skillData.ChoiceSkillEffectStatValue().Clone();
                var skillRarity = GetSkillRarityByLuck(_characterUnit.GetStat(Constants.TRAITS_LUCK));
                var found = false;
                foreach (var statLevel in statLevels)
                {
                    if (statLevel.stat.ID.Equals(skillEffectStatValue.stat.ID))
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

                upgradeSkillDatas[i] = new UpgradeSkillData(skillData.ID.ToString(), skillRarity, skillEffectStatValue);
            }

            return upgradeSkillDatas;
        }

        public T[] SelectRandomElements<T>(T[] array, int maxSelection, List<T> existingElements = null)
        {
            if (maxSelection > array.Length)
                throw new ArgumentException("maxSelection cannot be greater than array length.");

            // 随机生成一个数量，范围从 1 到 maxSelection
            int selectionCount = Random.Range(1, maxSelection + 1);

            // 使用 List 记录选择的索引，避免重复选择
            List<int> selectedIndices = new List<int>();

            while (selectedIndices.Count < selectionCount)
            {
                int randomIndex = Random.Range(0, array.Length);
                if (!selectedIndices.Contains(randomIndex))
                {
                    if (existingElements != null)
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
            T[] selectedElements = new T[selectionCount];
            for (int i = 0; i < selectionCount; i++)
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