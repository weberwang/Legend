using Cysharp.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Weber.Scripts.Model;

namespace Weber.Scripts.Domain
{
    public class LeveManager : Singleton<LeveManager>
    {
        private const string LEVEL_CONFIG = "Assets/Weber/Addressable/Config/EnemySpawnConfig_{0}.asset";
        public int Level { get; private set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void RuntimeInitialize()
        {
            Instance.WakeUp();
        }

        protected override void OnCreate()
        {
        }

        public async UniTask StartGame(int level)
        {
            Level = level;
            var asyncOperationHandle = Addressables.LoadAssetAsync<EnemySpawnConfig>(string.Format(LEVEL_CONFIG, Level));
            await asyncOperationHandle.Task;
            var config = asyncOperationHandle.Result;
            await EnemyFactory.Instance.StartGame(config);
        }
    }
}