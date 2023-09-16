//增加子弹基础伤害
public class Buff_2 : Buff
{
    public Buff_2(BuffConfig config) : base(config) { }
    public override void OnApply()
    {
        base.OnApply();
        IncreaseBaseShootingSpeed();
    }

    private void IncreaseBaseShootingSpeed()
    {
        GameManager.instance.attackModel.ShootingInterval *= (1 - config.value);
    }
}