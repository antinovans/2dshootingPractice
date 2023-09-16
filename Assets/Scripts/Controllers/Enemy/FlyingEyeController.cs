using UnityEngine.AI;

public class FlyingEyeController : BaseEnemyController
{
    private NavMeshAgent agent;
    protected override void OnAwake()
    {
        base.OnAwake();
        agent = GetComponent<NavMeshAgent>();
        agent.speed = config.defaultSpeed;
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        checkGroundRayLength = 1f;
    }
    protected override void UpdatePosition()
    {
        if (CurrentState == EnemyState.Move)
        {
            agent.speed = config.defaultSpeed;
            Navigate();
            if (agent.velocity.x> 0.1f) // 向右移动
            {
                isFacingRight = true;
            }
            else if (agent.velocity.x< -0.1f) // 向左移动
            {
                isFacingRight = false;
            }
        }
        else
        {
            agent.speed = 0;
        }
    }
    protected override void Navigate()
    {
        agent.SetDestination(target.position);
    }
    protected override void OnDied()
    {
        agent.isStopped = true;
        agent.enabled = false;
        base.OnDied();
        rb.gravityScale = 1;
    }
    protected override void OnReset()
    {
        base.OnReset();
        //清空状态池
        agent.enabled = true;
        rb.gravityScale = 0;
    }
}