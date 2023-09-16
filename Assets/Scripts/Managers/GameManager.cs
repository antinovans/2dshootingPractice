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
    //actions
    public Action<Vector2> onBulletFire;
    public Action<Vector2> onBulletHit;
    public Action<Vector2> onBulletHitEnemy;
    public Action<int> onEnemyAttack;
    public Action<EnemyConfig, Vector2> onEnemyDie;
    public Action<Vector2, float> onCameraZoomAt;
    public Action onLevelStart;
    public Action onLevelClear;
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
        onEnemyDie += (e,pos) => StopTime(e.onDiedPauseTime);
    }

    // Start is called before the first frame update


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
    private IEnumerator HitStop()
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
