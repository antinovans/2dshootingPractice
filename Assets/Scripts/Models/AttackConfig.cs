using UnityEngine;

[CreateAssetMenu(menuName = "AttackConfig")]
public class AttackConfig: ScriptableObject
{
    public float RecoilAmount;
    public float CameraShakeAmount;
    public float shootingInterval;
    public float maxAngleOffset;
}