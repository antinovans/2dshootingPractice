using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public class BuffManager : MonoBehaviour
{
    #region public fields
    public static BuffManager instance;
    public List<BuffConfig> BuffConfigs = new List<BuffConfig>();
    public Dictionary<string,Buff> AllBuffs=new Dictionary<string, Buff>();
    public Dictionary<string,Buff> ActivatedBuffs=new Dictionary<string, Buff>();
    // 转换字典为键值对列表
    List<KeyValuePair<string, Buff>> keyValuePairs;

// 随机选择一个键值对
    System.Random rand = new System.Random();
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
        BuffConfigs.ForEach(bc =>
        {
            Buff buff = CreateBuffInstance(bc.id,bc);
            AllBuffs.TryAdd(bc.id, buff);
        });
        keyValuePairs = AllBuffs.ToList();
    }
    private static Buff CreateBuffInstance(string id, params object[] constructorArgs)
    {
        string className = "Buff_" + id;

        Type buffType = Type.GetType(className);
        if (buffType == null)
        {
            Debug.LogError("Buff type not found for ID: " + id);
            return null;
        }
        Buff buffInstance = Activator.CreateInstance(buffType,constructorArgs) as Buff;
        return buffInstance;
    }

    public Buff GetRandomBuff()
    {
        var randomPair = keyValuePairs[rand.Next(keyValuePairs.Count)];
        return AllBuffs[randomPair.Key];
    }

    public void RecalculateBuffPool()
    {
        keyValuePairs = AllBuffs.ToList();
    }
}