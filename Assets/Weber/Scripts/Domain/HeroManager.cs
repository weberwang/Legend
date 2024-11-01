using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Weber.Scripts.Legend.Unit;
using Weber.Scripts.Model;

namespace Weber.Scripts.Domain
{
    public class HeroManager : Singleton<HeroManager>
    {
        private const string SKILL_CONFIG_PATH = "Assets/Weber/Addressable/Config/HeroSkillLevelValue.asset";
        private const string HERO_CONFIG_PATH = "Assets/Weber/Addressable/Config/AllHeroData.asset";
        private const string HERO_LEARN_SKILL_CONFIG_PATH = "Assets/Weber/Addressable/Config/LearnSkillData.asset";
        private const string HERO_SKILL_LUCK_PATH = "Assets/Weber/Addressable/Config/LuckConfig.asset";
        private const string HERO_STAT_LEVEL_PATH = "Assets/Weber/Addressable/Config/StatLevelConfig.asset";

        private const string HERO_PREFAB_PATH = "Assets/Weber/Addressable/Prefabs/Characters/Players/{0}.prefab";
        private const string SAVE_KEY = "HeroSKill";
        [field: NonSerialized] public HeroStatLevelValue HeroStatLevelValueConfig { get; private set; }

        [field: NonSerialized] public Dictionary<string, int> HeroSkillValues { get; private set; }

        [field: NonSerialized] public AllHeroData AllHeroData { get; private set; }
        [field: NonSerialized] public LearnSkillData LearnSkillData { get; private set; }

        [field: NonSerialized] public LuckConfig LuckConfig { get; private set; }

        [field: NonSerialized] public StatLevelConfig StatLevelConfig { get; private set; }

        public async UniTask LoadConfig()
        {
            await UniTask.WhenAll(LoadSkillLevelConfig(), LoadHeroConfig(), LoadSkillConfig(), LoadLuckConfig(), LoadStatLevelConfig());
        }

        private async UniTask LoadSkillLevelConfig()
        {
            var asyncOperationHandle = Addressables.LoadAssetAsync<HeroStatLevelValue>(SKILL_CONFIG_PATH);
            await asyncOperationHandle.Task;
            HeroStatLevelValueConfig = asyncOperationHandle.Result;
            if (ES3.KeyExists(SAVE_KEY))
            {
                HeroSkillValues = ES3.Load<Dictionary<string, int>>(SAVE_KEY);
            }
            else
            {
                HeroSkillValues = new Dictionary<string, int>();
            }
        }

        private async UniTask LoadHeroConfig()
        {
            var asyncOperationHandle = Addressables.LoadAssetAsync<AllHeroData>(HERO_CONFIG_PATH);
            await asyncOperationHandle.Task;
            AllHeroData = asyncOperationHandle.Result;
        }

        private async UniTask LoadSkillConfig()
        {
            var asyncOperationHandle = Addressables.LoadAssetAsync<LearnSkillData>(HERO_LEARN_SKILL_CONFIG_PATH);
            await asyncOperationHandle.Task;
            LearnSkillData = asyncOperationHandle.Result;
        }

        private async UniTask LoadLuckConfig()
        {
            var asyncOperationHandle = Addressables.LoadAssetAsync<LuckConfig>(HERO_SKILL_LUCK_PATH);
            await asyncOperationHandle.Task;
            LuckConfig = asyncOperationHandle.Result;
        }

        private async UniTask LoadStatLevelConfig()
        {
            var asyncOperationHandle = Addressables.LoadAssetAsync<StatLevelConfig>(HERO_STAT_LEVEL_PATH);
            await asyncOperationHandle.Task;
            StatLevelConfig = asyncOperationHandle.Result;
        }

        public SkillEffectStatValue GetSkillLevelValue(string id)
        {
            for (int i = 0; i < HeroStatLevelValueConfig.heroBaseValue.skillValues.Length; i++)
            {
                if (HeroStatLevelValueConfig.heroBaseValue.skillValues[i].stat.ID.String == id)
                {
                    return HeroStatLevelValueConfig.heroBaseValue.skillValues[i];
                }
            }

            return null;
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

        public HeroData GetHeroData(int heroId)
        {
            for (int i = 0; i < AllHeroData.heroDatas.Length; i++)
            {
                if (AllHeroData.heroDatas[i].id == heroId)
                {
                    return AllHeroData.heroDatas[i];
                }
            }

            return null;
        }

        public async UniTask<GameObject> LoadHeroPrefab(string heroName)
        {
            var asyncOperationHandle = Addressables.LoadAssetAsync<GameObject>(string.Format(HERO_PREFAB_PATH, heroName));
            await asyncOperationHandle.Task;
            return asyncOperationHandle.Result;
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