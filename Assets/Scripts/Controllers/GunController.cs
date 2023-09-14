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
    #endregion

    #region variables

    private float timer = 0.0f;
    private bool canShoot = false;

    #endregion
    public GunController(Transform attachedPos)
    {
        attachedTransform = attachedPos;
        attackModel = GameManager.instance.attackModel;
    }
    public void Shoot(bool isFacingRight)
    {
        if (!canShoot)
            return;
        
        Vector2 baseMovingDir = isFacingRight ? Vector2.right : Vector2.left;
        //触发子弹发射时的事件
        GameManager.instance.onBulletFire?.Invoke(baseMovingDir);
        
        // 子弹总的偏移范围，例如总共40度，那么每颗子弹就是40/numOfBulletsPerShot
        float totalAngleRange = attackModel.maxAngleOffset * 2;
        // 每颗子弹之间的角度差
        float anglePerBullet = attackModel.numOfBulletsPerShot == 1 ? 
            0 : totalAngleRange / (attackModel.numOfBulletsPerShot - 1);
        //根据每次设计生成子弹数量生成子弹
        for (int i = 0; i < attackModel.numOfBulletsPerShot; i++)
        {
            // 计算当前子弹的偏移角度
            float currentAngle = attackModel.numOfBulletsPerShot == 1 ? 0 : -attackModel.maxAngleOffset + i * anglePerBullet;

            // 添加随机偏移
            float randomOffset = Random.Range(-attackModel.randomAngleOffset, attackModel.randomAngleOffset);
            currentAngle += randomOffset;

            Quaternion currentRotation = isFacingRight ? 
                Quaternion.Euler(0, 0, currentAngle) : Quaternion.Euler(0, 0, currentAngle + 180f);
            Vector2 currentMovingDir = currentRotation * Vector2.right;
    
            // 通知bulletManager生成bullet
            BulletManager.instance.GenerateAttack(attachedTransform.position, currentRotation, currentMovingDir);
        }
        //play对应的声音
        SoundManager.Instance.PlayClipWithRandomPitch(BulletManager.instance.bulletModel.fireSoundClip);
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
