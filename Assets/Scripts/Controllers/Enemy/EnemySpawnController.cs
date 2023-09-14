using System;
using System.Threading;
using UnityEngine;

public class EnemySpawnController : MonoBehaviour
{
        public int spawnNumber = 100;
        public float SpawnInterval = 1f;
        private float spawnTimer = 0.0f;
        
        private void Update()
        {
                if(spawnNumber <= 0)
                        return;
                spawnTimer += Time.deltaTime;
                if (spawnTimer > SpawnInterval)
                { 
                    EnemyManager.instance.GenerateEnemy(EnemyType.Goblin, transform.position, 
                            Quaternion.identity); 
                    spawnTimer = 0.0f;
                    spawnNumber--;
                }
        }
}