using System;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class Bullet : MonoBehaviour
{
    #region components and references
    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private GameObject trail;
    private BulletModel curBulletModel;
    public Action<GameObject, DamageProfile> OnAttackHit;
    #endregion

    #region variables
    private bool canPlay;
    private bool isReleasing = false;
    private Vector2 movingDir;
    private float elapsedTime;
    #endregion
    private void Awake()=> Initialize();
    private void FixedUpdate() => OnFixedUpdate();

    private void OnEnable()=> OnReset();
    private void Initialize()
    {
        sr =  GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        elapsedTime = 0f;
        canPlay = false;
        movingDir = Vector2.zero;
        curBulletModel = BulletManager.instance.bulletModel;
        UpdateBulletBasedOnModel(curBulletModel);
        //设置攻击击中的效果回调
        OnAttackHit += (hitObject, dmgProfile) =>
        {
            BaseEnemyController enemy = hitObject.GetComponent<BaseEnemyController>();
            if (enemy)
            {
                enemy.OnReceiveDamageProfile(dmgProfile);
            }
        };
    }

    public void Set(Transform parent, Vector2 startPos, Quaternion rotation, BulletModel newModel)
    {
        if(transform.parent != parent)
            transform.SetParent(parent);
        transform.position = startPos;
        transform.rotation = rotation;
        if(curBulletModel != newModel)
            UpdateBulletBasedOnModel(newModel);
    }
    private void OnReset()
    {
        isReleasing = false;
        sr.enabled = false;
        elapsedTime = 0f;
        canPlay = false;
        movingDir = Vector2.zero;
        transform.rotation = quaternion.identity;
    }

    public void Play(Vector2 newMovingDir)
    {
        sr.enabled = true;
        this.movingDir = newMovingDir;
        canPlay = true;
    }
    private void UpdateBulletBasedOnModel(BulletModel newModel)
    {
        sr.sprite = newModel.bulletSprite;
        Destroy(trail);
        trail = Instantiate(newModel.trail, transform.position, Quaternion.identity, transform);
    }
    protected virtual void OnFixedUpdate()
    {
        if(elapsedTime > curBulletModel.existingDuration)
            BulletManager.instance.Release(this);
        elapsedTime += Time.deltaTime;
        float curveValue = curBulletModel.speedCurve.Evaluate(elapsedTime / curBulletModel.existingDuration);
        float currentSpeed = Mathf.Lerp(curBulletModel.minSpeed, curBulletModel.maxSpeed, curveValue);
        rb.velocity = currentSpeed * movingDir;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isReleasing)
            return;
        isReleasing = true;
        
        //特效处理
        Vector2 impactPoint = collision.contacts[0].point;
        VFXManager.instance.GenerateVFX(VFXType.OnHit, impactPoint);
        //触发子弹碰到的action
        GameManager.instance.onBulletHit?.Invoke(impactPoint);
        //给enemy传伤害
        if (collision.gameObject.CompareTag("Enemy"))
        {
            //顿帧处理
            if (curBulletModel.pauseTime > 0f)
            {
                GameManager.instance.StopTime(curBulletModel.pauseTime);
            }
            //update damage profile准备传参
            Vector2 pushDirection = (collision.gameObject.transform.position - transform.position).normalized;
            pushDirection.Normalize();
            curBulletModel.dmgProfile.SetRepelDirection(pushDirection);

            float xDir = movingDir.x > 0 ? 1.0f : -1.0f;
            Vector2 hitDir = new Vector2(xDir, 0f);
            OnAttackHit?.Invoke(collision.gameObject, curBulletModel.dmgProfile);
        }
        //回到对象池
        BulletManager.instance.Release(this);
    }
}