//子弹击中造成爆炸

using UnityEngine;

public class Buff_3 : Buff
{
    public Buff_3(BuffConfig config) : base(config) { }
    public override void OnApply()
    {
        base.OnApply();
        GameManager.instance.onBulletHitEnemy += CastExplosion;
    }

    private void CastExplosion(Vector2 impactPoint)
    {
        if(Random.Range(0f,1f) < config.value)
            VFXManager.instance.GenerateVFX(VFXType.OhExplosion, impactPoint + Random.insideUnitCircle);
        BulletManager.instance.bulletModel.dmgProfile.ImpulseDamageAmount += (int)config.value;
    }
}