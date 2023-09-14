//升级你的子弹，造成更高伤害
public class Buff_4 : Buff
{
    public Buff_4(BuffConfig config) : base(config) { }
    public override void OnApply()
    {
        base.OnApply();
        BulletManager.instance.UpgradeModel();
        VFXManager.instance.UpdateEffect(VFXType.OnHit);
    }
}