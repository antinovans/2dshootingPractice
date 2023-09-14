using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region public fields

    public GameObject player;
    public Transform target;
    public Transform corpseTransform;
    public bool playerIsFacingRight = true;
    public bool playerIsShooting = false;
    public AttackModel attackModel;
    public BulletModel curBulletModel;
    public Action<Vector2> onBulletFire;
    public Action<EnemyConfig> onEnemyDie;
    public static GameManager instance;
    #endregion
    [SerializeField]private AttackConfig config;
    private Coroutine stopTimeCor = null;
    private float currentHitStopTimeLeft = 0f; // 当前停顿剩余时间
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
        attackModel = new AttackModel(config);
        onEnemyDie += (e => StopTime(e.onDiedPauseTime));
    }

    // Start is called before the first frame update
    void Start()
    {
        curBulletModel = BulletManager.instance.bulletModel;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void StopTime(float duration)
    {
        if (stopTimeCor != null && duration < currentHitStopTimeLeft)
            return;
        if (stopTimeCor == null)
        {
            currentHitStopTimeLeft = duration;
            stopTimeCor = StartCoroutine(HitStop());
            return;
        }
        // 如果请求的停顿时间大于当前剩余的停顿时间，则更新
        if (duration > currentHitStopTimeLeft)
        {
            currentHitStopTimeLeft = duration;
        }
    }
    IEnumerator HitStop()
    {
        Time.timeScale = 0.1f;
        // 等待，直到所有的停顿都完成
        while (currentHitStopTimeLeft > 0f)
        {
            yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);
            currentHitStopTimeLeft -= Time.fixedDeltaTime;
        }
        stopTimeCor = null;
        Time.timeScale = 1f;
    }
}
