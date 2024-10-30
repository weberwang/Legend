using System.Collections.Generic;
using UnityEngine;

namespace Weber.Scripts.Model
{
    [CreateAssetMenu(fileName = "EnemySpawnConfig", menuName = "Weber/EnemySpawnConfig", order = 0)]
    public class EnemySpawnConfig : ScriptableObject
    {
        // 每个时间段的生成配置列表
        public List<SpawnWave> spawnWaves;
    }

    [System.Serializable]
    public class SpawnWave
    {
        // 时间点（秒）
        public float startTime;

        public float endTime;

        // 敌人生成的数量
        public int enemyCount;

        // 敌人类型，可以引用敌人Prefab
        public int enemyID;

        // 生成频率或间隔时间
        public float spawnInterval;

        // 敌人的生成区域（可选择多个点随机生成）
        // public List<Transform> spawnLocations;

        // 是否是Boss或精英怪
        public EnemyType enemyType;

        //数值缩放
        public float scalingFactor = 1;
    }
}