using System;
using System.Collections;
using UnityEngine;

public class ExplosionVFXController : VFXController
{
    [SerializeField]private DamageProfile profile;
    [SerializeField] private float pauseTime;

    private float minForce;
    private float maxForce;
    private CircleCollider2D mCollider;
    private float explosionRadius;
    private Action<GameObject, DamageProfile, Vector2> OnExplosionHit;
    protected override void Init()
    {
        base.Init();
        mCollider = GetComponent<CircleCollider2D>();
        explosionRadius = GetComponent<CircleCollider2D>().radius;
        maxForce = profile.RepelForceMagnitude;
        minForce = maxForce / 4;
        OnExplosionHit += (hitObject, dmgProfile, force) =>
        {
            BaseEnemyController enemy = hitObject.GetComponent<BaseEnemyController>();
            if (enemy)
            {
                enemy.OnReceiveDamageProfile(dmgProfile);
                enemy.OnRepel(force);
            }
        };
    }
    protected override void OnReset()
    {
        base.OnReset();
        mCollider.enabled = false;
    }
    public override void Play()
    {
        base.Play();
        mCollider.enabled = true;
        StartCoroutine(DisableCollider());
        GameManager.instance.StopTime(pauseTime);
    }

    IEnumerator DisableCollider()
    {
        yield return new WaitForSeconds(0.1f);
        mCollider.enabled = false;
    }
    private Vector2 CalculateForce(Vector3 otherPos)
    {
        
        Vector2 displacement = otherPos - transform.position;
        float force = Mathf.Lerp(maxForce, minForce, displacement.magnitude / explosionRadius);
        return force * displacement.normalized;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        var force = CalculateForce(other.gameObject.transform.position);
        OnExplosionHit?.Invoke(other.gameObject, profile, force);
        VFXManager.instance.GenerateVFX(VFXType.Smoke, transform.position); //生成烟雾
    }
}