public class AttackModel
{
    public float RecoilAmount { get; set; }
    public float CameraShakeAmount { get; set; }
    public float ShootingInterval { get; set; }
    public float maxAngleOffset { get; set; }
    public float randomAngleOffset { get; set; }
    public int numOfBulletsPerShot { get; set; }

    public AttackModel(AttackConfig config)
    {
        RecoilAmount = config.RecoilAmount;
        CameraShakeAmount = config.CameraShakeAmount;
        ShootingInterval = config.shootingInterval;
        maxAngleOffset = config.maxAngleOffset;
        randomAngleOffset = config.randomAngleOffset;
        numOfBulletsPerShot = config.numOfBulletsPerShot;
    } 
}