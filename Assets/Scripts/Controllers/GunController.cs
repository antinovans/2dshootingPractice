using UnityEngine;

public interface IGun
{
    void Shoot(bool isFacingRight);
    void OnUpdate();
}
public class GunController : IGun
{
    #region references
    private Transform attachedTransform;
    private AttackModel attackModel;
    private BulletModel bulletModel;
    #endregion

    #region variables

    private float timer = 0.0f;
    private bool canShoot = false;

    #endregion
    public GunController(Transform attachedPos)
    {
        attachedTransform = attachedPos;
        attackModel = GameManager.instance.attackModel;
        bulletModel = BulletManager.instance.bulletModel;
    }
    public void Shoot(bool isFacingRight)
    {
        if (!canShoot)
        {
            
            return;
        }
        Quaternion rotation = Quaternion.identity;
        if (!isFacingRight)
        {
            rotation = Quaternion.Euler(0f, 0f, 180f);
        }
        Vector2 movingDir = isFacingRight ? Vector2.right : Vector2.left;
        GameManager.instance.onBulletFire?.Invoke(movingDir);
        //子弹产生随机偏移
        float randomAngle = Random.Range(-attackModel.maxAngleOffset, attackModel.maxAngleOffset);
        Quaternion randomRotation = Quaternion.Euler(0, 0, randomAngle);
        movingDir = randomRotation * movingDir;
        
        BulletManager.instance.GenerateAttack(attachedTransform.position, rotation,
            movingDir);
        //play对应的声音
        SoundManager.Instance.PlayClipWithRandomPitch(bulletModel.fireSoundClip);
        canShoot = false;
    }

    public void OnUpdate()
    {
        if (!canShoot)
        {
            timer += Time.deltaTime;
            if (timer > attackModel.ShootingInterval)
            {
                timer = 0.0f;
                canShoot = true;
            }
        }
    }
}
