using UnityEngine;

public class StatusStun : StatusBase
{
    public Vector3 RepelForce;
    public StatusStun(IStatusReceiver parent, float duration,
        Vector3 repelForce) : base(parent, duration)
    {
        this.StatusKind = StatusKind.Stun;
        this.RepelForce = repelForce;
    }

    public override void OnAdd()
    {
        base.OnAdd();
        Parent.OnEnterStun();
        if (RepelForce.magnitude > 0)
        {
            Parent.OnInstantRepel(RepelForce);
        }
    }
    public override void OnUpdate()
    {
        base.OnUpdate();
        if (DurationTimer < Duration)
        {
            DurationTimer += Time.deltaTime;
            return;
        }
        //remove this
        Parent.RemoveStatus(this);
    }
    public override void OnRemove()
    {
        Parent.OnExitStun();
        base.OnRemove();
    }

}