using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Pool;

public enum EnemyType
{
    Goblin = 0,
    FlyingEye = 1
}
public class EnemyManager : MonoBehaviour
{
    #region public fields
    public static EnemyManager instance;
    public Transform enemysParent;
    #endregion
    
    #region private
    [SerializeField] private List<BaseEnemyController> enemyPrefabs;
    
    private ObjectPool<BaseEnemyController>[] enemyPools;
    
    #endregion
    private void Awake()
    {
        if(instance!= null)
            Destroy(gameObject);
        else
        {
            instance = this;
        }
        Init();
        
    }
    private void Init()
    {
        InitPools();
    }
    private void InitPools()
    {
        int enemyTypeCount = Enum.GetValues(typeof(EnemyType)).Length;
        enemyPools = new ObjectPool<BaseEnemyController>[enemyTypeCount];
        foreach (int enemyType in Enum.GetValues(typeof(EnemyType)))
        {
            enemyPools[enemyType] = new ObjectPool<BaseEnemyController>(
                ()=>Instantiate(enemyPrefabs[enemyType], enemysParent),
                e=>e.gameObject.SetActive(true),
                e=>e.gameObject.SetActive(false),
                e=>Destroy(e.gameObject),
                true, 10, 100
            );
        }
    }
    public BaseEnemyController GenerateEnemy(EnemyType type, Vector2 pos, Quaternion rotation)
    {
        BaseEnemyController enemy = enemyPools[(int)type].Get();
        enemy.OnSet(pos, rotation);
        return enemy;
    }
    public void Release(BaseEnemyController enemy)
    {
        enemyPools[(int)enemy.GetEnemyType()].Release(enemy);
    }
}