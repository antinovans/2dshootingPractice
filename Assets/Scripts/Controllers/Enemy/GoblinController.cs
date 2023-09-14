public class GoblinController : BaseEnemyController
{
    protected override void OnAwake()
    {
        base.OnAwake();
        checkGroundRayLength = 1.34f;
    }
}