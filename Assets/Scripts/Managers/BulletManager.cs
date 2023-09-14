using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Pool;

public class BulletManager : MonoBehaviour
{
    #region public fields
    public static BulletManager instance;
    public BulletModel bulletModel;
    public List<BulletConfig> bulletConfigs = new List<BulletConfig>();
    public Action onBulletModelChanged;
    #endregion

    #region private
    [SerializeField] private Bullet bulletPrefab;
    [SerializeField] private Transform bulletParent;

    private ObjectPool<Bullet> bulletPool;

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
        InitPool();
        Assert.IsTrue(bulletConfigs.Count > 0);
        bulletModel = new BulletModel(bulletConfigs[0]);
    }
    private void InitPool()
    {
        bulletPool = new ObjectPool<Bullet>(
            () => Instantiate(bulletPrefab,bulletParent),
            e=>e.gameObject.SetActive(true),
            e=>e.gameObject.SetActive(false),
            e=>Destroy(e.gameObject),
            true, 100, 1000);
    }

    private int curModel = 0;
    public void UpgradeModel()
    {
        curModel++;
        bulletModel = new BulletModel(bulletConfigs[curModel]);
        onBulletModelChanged?.Invoke();
    }
    public Bullet GenerateAttack(Vector2 pos, Quaternion rotation, Vector2 movingDir)
    {
        Bullet bullet = bulletPool.Get();
        bullet.Set(bulletParent,pos,rotation, bulletModel);
        bullet.Play(movingDir);
        return bullet;
    }

    public void Release(Bullet bullet)
    {
        bulletPool.Release(bullet);
    }
}
