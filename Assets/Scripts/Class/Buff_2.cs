//增加子弹基础伤害
public class Buff_2 : Buff
{
    public Buff_2(BuffConfig config) : base(config) { }
    public override void OnApply()
    {
        base.OnApply();
        IncreaseBaseDamage();
        BulletManager.instance.onBulletModelChanged += IncreaseBaseDamage;
    }

    private void IncreaseBaseDamage()
    {
        BulletManager.instance.bulletModel.dmgProfile.ImpulseDamageAmount += (int)config.value;
    }
}