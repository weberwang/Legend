using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Weber.Scripts.Common.Utils;
using Weber.Scripts.Legend.Game;
using Weber.Scripts.Legend.Game.Items;
using Weber.Scripts.Legend.Unit;
using Weber.Scripts.Model;
using Random = UnityEngine.Random;

namespace Weber.Scripts.Domain
{
    public class EnemyFactory : Singleton<EnemyFactory>
    {
        private List<CharacterUnit> m_Enemies = new List<CharacterUnit>();

        public List<CharacterUnit> Enemies => m_Enemies;

        private const string ENEMY_PATH = "Assets/Weber/Addressable/Prefabs/Characters/Enemies/Enemy.prefab";
        private const string ENEMY_MODEL_PATH = "Assets/Weber/Addressable/Prefabs/Characters/Enemies/Enemy_{0}.prefab";
        private const string ENEMY_SPAWN_PATH = "Assets/Weber/Addressable/Config/EnemySpawnConfig.asset";
        private const string ALL_ENEMY_PATH = "Assets/Weber/Addressable/Config/AllEnemyData.asset";
        private const string XP_BALL_PATH = "Assets/Weber/Addressable/Prefabs/Items/XPBall.prefab";

        private GameObject _enemyPrefab;
        private GameObject _xpBallPrefab;
        private AllEnemyData _allEnemyData;

        private Camera _mainCamera;
        private readonly int _spawnDistance = 2;
        private readonly float _groundLevel = 0;
        private readonly float _minDistanceFromPlayer = 3f;
        private int _groundMask;
        private RaycastHit[] _hits = new RaycastHit[1];

        private EnemySpawnConfig _currentEnemySpawnConfig;

        //保存每次生成敌人的时间
        private Dictionary<int, float> _spawnTime = new Dictionary<int, float>();

        //所有掉落物品
        private List<DropItem> _dropItems = new List<DropItem>();

        public int EnemyKillCount { get; private set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void RuntimeInitialize()
        {
            Instance.WakeUp();
        }

        public async UniTask StartGame(EnemySpawnConfig enemySpawnConfig)
        {
            EnemyKillCount = 0;
            m_Enemies.Clear();
            _currentEnemySpawnConfig = enemySpawnConfig;
            List<int> _loadeds = new List<int>();
            for (int i = 0; i < _currentEnemySpawnConfig.spawnWaves.Count; i++)
            {
                if (_loadeds.Contains(_currentEnemySpawnConfig.spawnWaves[i].enemyID))
                {
                    continue;
                }

                await LoadEnemyModel(_currentEnemySpawnConfig.spawnWaves[i].enemyID);
            }
        }

        private void Update()
        {
            if (Game.Instance.GameState != GameState.Playing) return;
            //根据_currentEnemySpawnConfig配置生成敌人
            for (int i = 0; i < _currentEnemySpawnConfig.spawnWaves.Count; i++)
            {
                var wave = _currentEnemySpawnConfig.spawnWaves[i];
                if (Game.Instance.GameTime >= wave.startTime && Game.Instance.GameTime <= wave.endTime)
                {
                    if (_spawnTime.ContainsKey(wave.enemyID))
                    {
                        _spawnTime[wave.enemyID] += Time.deltaTime;
                        if (_spawnTime[wave.enemyID] >= wave.spawnInterval)
                        {
                            SpawnWaveEnemies(wave);
                            _spawnTime[wave.enemyID] = 0;
                        }
                    }
                    else
                    {
                        SpawnWaveEnemies(wave);
                        _spawnTime.Add(wave.enemyID, 0);
                    }
                }
            }
        }

        protected override void OnCreate()
        {
            LoadEnemy();
            LoadEnemyConfig();
            LoadXPBall();
            _groundMask = LayerMask.GetMask("Ground");
            _mainCamera = Camera.main;
        }

        private async void LoadEnemy()
        {
            var asyncOperationHandle = Addressables.LoadAssetAsync<GameObject>(ENEMY_PATH);
            await asyncOperationHandle.Task;
            _enemyPrefab = asyncOperationHandle.Result;
        }

        private async void LoadEnemyConfig()
        {
            var asyncOperationHandle = Addressables.LoadAssetAsync<AllEnemyData>(ALL_ENEMY_PATH);
            await asyncOperationHandle.Task;
            _allEnemyData = asyncOperationHandle.Result;
        }

        private async UniTask LoadEnemyModel(int enemyID)
        {
            var enemyModelPath = string.Format(ENEMY_MODEL_PATH, enemyID);
            var asyncOperationHandle = Addressables.LoadAssetAsync<GameObject>(enemyModelPath);
            await asyncOperationHandle.Task;
        }

        private async UniTask LoadXPBall()
        {
            var asyncOperationHandle = Addressables.LoadAssetAsync<GameObject>(XP_BALL_PATH);
            await asyncOperationHandle.Task;
            _xpBallPrefab = asyncOperationHandle.Result;
        }

        public void SpawnWaveEnemies(SpawnWave wave)
        {
            for (int i = 0; i < wave.enemyCount; i++)
            {
                CreateEnemy(wave.enemyID);
            }
        }

        public async UniTask<Enemy> CreateEnemy(int enemyID)
        {
            var enemyModelPath = string.Format(ENEMY_MODEL_PATH, enemyID);
            var asyncOperationHandle = Addressables.LoadAssetAsync<GameObject>(enemyModelPath);
            await asyncOperationHandle.Task;
            var model = asyncOperationHandle.Result;
            Vector3 targetPosition;
            while (!SpawnEnemyOffScreen(out targetPosition))
            {
                await UniTask.Yield();
            }

            targetPosition.y = 1;
            var enemyInstance = PoolManager.Instance.Pick(_enemyPrefab, targetPosition, Quaternion.identity, 1);
            var enemy = enemyInstance.Get<Enemy>();
            enemy.Character.ChangeModel(model, new Character.ChangeOptions());
            enemy.ResetSelf(enemyID);
            enemy.OnDeath += OnEnemyDeath;
            m_Enemies.Add(enemy);
            return enemy;
        }

        private void OnEnemyDeath(CharacterUnit characterUnit)
        {
            //todo 生成掉落物品,经验球或者道具

            EnemyKillCount++;
            Enemy enemy = characterUnit as Enemy;
            enemy.OnDeath -= OnEnemyDeath;
            Signals.Emit(new SignalArgs(SignalNames.ENEMY_DEATH, enemy.gameObject));
            DelaySpawnBall(enemy);
            if (enemy.EnemyData.type == EnemyType.LevelBoss)
            {
                //最终Boss被击败，胜利
            }
        }

        private void DelaySpawnBall(Enemy enemy)
        {
            var position = enemy.transform.position;
            var awardXP = enemy.EnemyData.awardXP;
            var _xpBallInstance = PoolManager.Instance.Pick(_xpBallPrefab, 1);
            _xpBallInstance.transform.position = position;
            var xpItem = _xpBallInstance.GetComponent<XPDropItem>();
            xpItem.InitialValue(awardXP);
            Debug.Log("掉落经验 :" + awardXP);
        }

        private bool SpawnEnemyOffScreen(out Vector3 position)
        {
            // 获取屏幕边界的四个角（视口 0-1），并转换为世界坐标
            Vector3 screenBottomLeft = GetGroundPosition(new Vector3(0, 0, _mainCamera.nearClipPlane));
            Vector3 screenBottomRight = GetGroundPosition(new Vector3(1, 0, _mainCamera.nearClipPlane));
            Vector3 screenTopLeft = GetGroundPosition(new Vector3(0, 1, _mainCamera.nearClipPlane));
            Vector3 screenTopRight = GetGroundPosition(new Vector3(1, 1, _mainCamera.nearClipPlane));

            // 扩展边界
            float minX = Mathf.Min(screenBottomLeft.x, screenTopLeft.x) - _spawnDistance;
            float maxX = Mathf.Max(screenBottomRight.x, screenTopRight.x) + _spawnDistance;
            float minZ = Mathf.Min(screenBottomLeft.z, screenBottomRight.z) - _spawnDistance;
            float maxZ = Mathf.Max(screenTopLeft.z, screenTopRight.z) + _spawnDistance;

            // 随机选择一个可用的方向
            int side = Random.Range(0, 4);

            // 生成敌人
            Vector3 spawnPosition = Vector3.zero;
            switch (side)
            {
                case 0: // 左边
                    spawnPosition = new Vector3(minX, _groundLevel, Random.Range(minZ, maxZ));
                    break;
                case 1: // 右边
                    spawnPosition = new Vector3(maxX, _groundLevel, Random.Range(minZ, maxZ));
                    break;
                case 2: // 下边
                    spawnPosition = new Vector3(Random.Range(minX, maxX), _groundLevel, minZ);
                    break;
                case 3: // 上边
                    spawnPosition = new Vector3(Random.Range(minX, maxX), _groundLevel, maxZ);
                    break;
            }

            // 确保与玩家保持一定距离
            if (Vector3.Distance(spawnPosition, ShortcutPlayer.Transform.position) < _minDistanceFromPlayer)
            {
                position = Vector3.zero;
                return false;
            }


            //是否在地面上
            if (Physics.RaycastNonAlloc(spawnPosition + Vector3.up * 20, Vector3.down, _hits, 100, _groundMask) <= 0)
            {
                position = Vector3.zero;
                return false;
            }

            position = spawnPosition;
            return true;
        }

        // 将视口坐标转换为在地面上的世界坐标
        Vector3 GetGroundPosition(Vector3 viewportPosition)
        {
            // 将视口坐标转换为摄像机前方的世界坐标
            Vector3 worldPosition = _mainCamera.ViewportToWorldPoint(viewportPosition);

            // 计算从摄像机到该点的射线
            Ray ray = new Ray(_mainCamera.transform.position, worldPosition - _mainCamera.transform.position);

            // 使用射线投射到地面上 (Y = groundY)
            float distanceToGround = (_mainCamera.transform.position.y - _groundLevel) / ray.direction.y;
            Vector3 groundPosition = ray.origin - ray.direction * distanceToGround;

            return groundPosition;
        }

        public void RemoveEnemy(Enemy enemy)
        {
            m_Enemies.Remove(enemy);
            enemy.gameObject.SetActive(false);
        }

        public void RemoveAll()
        {
            foreach (var enemy in m_Enemies)
            {
                enemy.gameObject.SetActive(false);
            }

            m_Enemies.Clear();
        }

        public void Clear()
        {
            foreach (var enemy in m_Enemies)
            {
                Destroy(enemy.gameObject);
            }

            m_Enemies.Clear();
        }

        public CharacterUnit FindNearestEnemy(Vector3 position)
        {
            if (m_Enemies.Count == 0)
            {
                return null;
            }

            float minDistance = float.MaxValue;
            CharacterUnit nearestEnemy = null;
            foreach (var enemy in m_Enemies)
            {
                float distance = Vector3.Distance(position, enemy.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestEnemy = enemy;
                }
            }

            return nearestEnemy;
        }

        //查找范围内血量最低的敌人
        public CharacterUnit FindLowestHealthEnemy(Vector3 position, float range)
        {
            if (m_Enemies.Count == 0)
            {
                return null;
            }

            float minHealth = float.MaxValue;
            CharacterUnit lowestHealthEnemy = null;
            foreach (var enemy in m_Enemies)
            {
                float distance = Vector3.Distance(position, enemy.transform.position);
                if (distance <= range && enemy.Health < minHealth)
                {
                    minHealth = enemy.Health;
                    lowestHealthEnemy = enemy;
                }
            }

            return lowestHealthEnemy;
        }

        public EnemyData GetEnemyData(int id)
        {
            for (int i = 0; i < _allEnemyData.enemyDatas.Length; i++)
            {
                if (_allEnemyData.enemyDatas[i].id == id)
                {
                    return _allEnemyData.enemyDatas[i];
                }
            }

            return null;
        }

        // private DropItem SpawnDropItem(Enemy enemy)
        // {
        //     var dropInstance = PoolManager.Instance.Pick(_enemyPrefab, enemy.transform.position, Quaternion.identity, 1);
        //     var dropItem = dropInstance.Get<DropItem>();
        //     dropItem.InitialValue(value);
        //     _dropItems.Add(dropItem);
        //     return dropItem;
        // }
    }
}