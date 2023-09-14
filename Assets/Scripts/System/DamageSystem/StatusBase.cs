public enum StatusKind
{
    InstantDamage = 0,
    Stun = 1
}
public abstract class StatusBase
{
    public StatusKind StatusKind;
    protected float DurationTimer;
    public float Duration;
    public IStatusReceiver Parent;

    public StatusBase(IStatusReceiver parent, float duration)
    {
        this.Parent = parent;
        this.Duration = duration;
        this.DurationTimer = 0f;
    }
    public StatusBase(float duration)
    {
        this.Duration = duration;
        this.DurationTimer = 0f;
    }

    public StatusBase SetParent(IStatusReceiver parent)
    {
        this.Parent = parent;
        return this;
    }

    public virtual void ResetDuration(float newDuration)
    {
        if(Duration - DurationTimer > newDuration)
            return;
        this.DurationTimer = 0f;
        this.Duration = newDuration;
    }
    
    public virtual void OnAdd() { }
    public virtual void OnUpdate() { }

    public virtual void OnAttackColliderExit() { }

    public virtual void OnRemove() { }

}