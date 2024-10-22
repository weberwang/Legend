using System;
using System.Collections.Generic;
using UnityEngine;
using Weber.Scripts.Legend.Skill;
using Weber.Scripts.Model;

namespace Weber.Scripts.Legend.Unit
{
    [RequireComponent(typeof(CharacterUnit))]
    public class SkillManager : MonoBehaviour
    {
        [SerializeField] private SkillData m_InitSkillData;

        //可以学习升级的技能
        [SerializeField] private SkillData[] m_SkillDatas;


        private List<SkillData> m_learnedSkills = new List<SkillData>();


        private CharacterUnit m_CharacterUnit;

        private void Awake()
        {
            Load();
            m_CharacterUnit = GetComponent<CharacterUnit>();
            LearnSkill(m_InitSkillData);
        }

        private void Start()
        {
            for (int i = 0; i < m_SkillDatas.Length; i++)
            {
                LearnSkill(m_SkillDatas[i]);
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
        public void LearnSkill(SkillData skillData)
        {
            if (!m_learnedSkills.Contains(skillData))
            {
                if (skillData.GetType() == typeof(AttackSkillData))
                {
                    (skillData as AttackSkillData).MergeHitEffect();
                }

                m_learnedSkills.Add(skillData);
            }

            UpgradeSkill(skillData);
        }

        private void UpgradeSkill(SkillData skillData)
        {
            skillData.Learn(m_CharacterUnit);
        }

        private void Save()
        {
        }

        private void Load()
        {
        }
    }
}