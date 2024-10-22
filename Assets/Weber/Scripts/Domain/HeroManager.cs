using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine.AddressableAssets;
using Weber.Scripts.Legend.Unit;
using Weber.Scripts.Model;

namespace Weber.Scripts.Domain
{
    public class HeroManager : Singleton<HeroManager>
    {
        private const string SKILL_CONFIG_PATH = "Assets/Weber/Addressable/Config/HeroSkillLevelValue.asset";

        private const string SAVE_KEY = "HeroSKill";
        public HeroSkillLevelValue HeroSkillLevelValue { get; private set; }

        public Dictionary<string, int> HeroSkillValues { get; private set; }

        public async UniTask LoadConfig()
        {
            var asyncOperationHandle = Addressables.LoadAssetAsync<HeroSkillLevelValue>(SKILL_CONFIG_PATH);
            await asyncOperationHandle.Task;
            HeroSkillLevelValue = asyncOperationHandle.Result;
            if (ES3.KeyExists(SAVE_KEY))
            {
                HeroSkillValues = ES3.Load<Dictionary<string, int>>(SAVE_KEY);
            }
            else
            {
                HeroSkillValues = new Dictionary<string, int>();
            }
        }

        public int GetHeroBaseLevel(string skillId)
        {
            if (HeroSkillValues.ContainsKey(skillId))
            {
                return HeroSkillValues[skillId];
            }

            return 0;
        }

        public void UpgradeHeroBaseLevel(string skillId)
        {
            if (HeroSkillValues.ContainsKey(skillId))
            {
                HeroSkillValues[skillId]++;
            }
            else
            {
                HeroSkillValues.Add(skillId, 1);
            }
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                ES3.Save(SAVE_KEY, HeroSkillValues);
            }
        }

#if UNITY_EDITOR
        private void OnApplicationQuit()
        {
            ES3.Save(SAVE_KEY, HeroSkillValues);
        }
#endif
    }
}