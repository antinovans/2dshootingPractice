
public class StatusInstantDamage : StatusBase
{
    public int instantDamageAmount;

    public StatusInstantDamage(IStatusReceiver parent, float duration, int damageAmount) : base(parent,0f)
    {
        this.StatusKind = StatusKind.InstantDamage;
        this.instantDamageAmount = damageAmount;
    }
    public override void OnAdd()
    {
        base.OnAdd();
        Parent.OnDamage(instantDamageAmount);
    }
}