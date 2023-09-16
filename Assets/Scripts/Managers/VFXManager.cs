using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public enum VFXType
{
    OnHit = 0,
    OhExplosion = 1,
    Smoke = 2
}

[Serializable]
public class VFXGroup
{
    public VFXType type;
    public VFXController[] VFXPrefabs;
    private int idx = 0;
    public VFXController NextPrefab()
    {
        idx++;
        return VFXPrefabs[idx];
    }
}
public class VFXManager : MonoBehaviour
{
    #region public fields
    public static VFXManager instance;
    public Transform vfxParent;
    public List<VFXGroup> VFXPrefabs;
    public VFXController[] defaultEffects;  //reference that points to default effect
    #endregion
    #region private fields

    private Dictionary<VFXType,ObjectPool<VFXController>> vfxPools;
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
        int numOfEffects = Enum.GetValues(typeof(VFXType)).Length;
        defaultEffects = new VFXController[numOfEffects];
        
        vfxPools = new Dictionary<VFXType, ObjectPool<VFXController>>();
        foreach (VFXType vfxType in Enum.GetValues(typeof(VFXType)))
        {
            defaultEffects[(int)vfxType] = VFXPrefabs[(int)vfxType].VFXPrefabs[0];
            vfxPools[vfxType] = new ObjectPool<VFXController>(
                ()=>Instantiate(defaultEffects[(int)vfxType], vfxParent),
                e=>e.gameObject.SetActive(true),
                e=>e.gameObject.SetActive(false),
                e=>Destroy(e.gameObject),
                true, 100, 1000
                );
        }
    }
    public void UpdateEffect(VFXType upgradeType)
    {
        ClearPoolOnVFXType(upgradeType);
        defaultEffects[(int)upgradeType] = VFXPrefabs[(int)upgradeType].NextPrefab();
    }

    private void ClearPoolOnVFXType(VFXType type)
    {
        vfxPools[type].Clear();
    }
    public VFXController GenerateVFX(VFXType type, Vector2 pos)
    {
        VFXController vfx = vfxPools[type].Get();
        vfx.Set(pos);
        vfx.Play();
        return vfx;
    }
    public void Release(VFXController vfxController)
    {
        vfxPools[vfxController.vfxType].Release(vfxController);
    }
}