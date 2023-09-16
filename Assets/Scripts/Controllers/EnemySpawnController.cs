using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Assertions;
using Random = System.Random;

public class EnemySpawnController : MonoBehaviour
{
        private float spawnInterval;
        private Dictionary<EnemyType,int> enemyToSpawnDic;
        private List<EnemyType> keys;
        private int totalCount = 0;
        private Random rand;

        private void Awake()
        {
                enemyToSpawnDic = new Dictionary<EnemyType, int>();
                keys = new List<EnemyType>(enemyToSpawnDic.Keys);
                rand = new Random();
        }

        private void Start()
        {
                GameManager.instance.onLevelStart += BeginSpawn;
        }

        public int PrepareLevel(EnemyWaveStats config, float interval)
        {
                enemyToSpawnDic.Clear();
                totalCount = 0;
                foreach (var stat in config.enemyWaveKeyPairs)
                {
                        totalCount += stat.enemyCount;
                        enemyToSpawnDic.Add(stat.enemyType, stat.enemyCount);
                }
                keys = new List<EnemyType>(enemyToSpawnDic.Keys);
                spawnInterval = interval;
                return totalCount;
        }

        public void BeginSpawn()
        {
                if(totalCount == 0)
                        return;
                StartCoroutine(SpawnEnemyCor());
        }

        private IEnumerator SpawnEnemyCor()
        {
                while (totalCount > 0)
                {
                        var randomKey = keys[rand.Next(keys.Count)];
                        EnemyManager.instance.GenerateEnemy(randomKey, transform.position, 
                                Quaternion.identity);
                        totalCount--;
                        enemyToSpawnDic[randomKey]--;
                        Assert.IsTrue(enemyToSpawnDic[randomKey] >= 0);
                        if (enemyToSpawnDic[randomKey] == 0)
                        {
                                enemyToSpawnDic.Remove(randomKey);
                                keys = new List<EnemyType>(enemyToSpawnDic.Keys);
                        }

                        yield return new WaitForSeconds(spawnInterval);

                }
        }
}