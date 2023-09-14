using UnityEngine;

[CreateAssetMenu(menuName = "BulletConfig")]
public class BulletConfig : ScriptableObject
{
    [Header("逻辑")]
    public float maxSpeed;
    public float minSpeed;
    public AnimationCurve speedCurve;
    public float pushBackForce;
    public float pauseTime;
    public float existingDuration;
    public DamageProfile dmgProfile;
    [Header("表现")] 
    public Sprite bulletSprite;
    public GameObject trail;
    [Header("音效")] 
    public SoundClip fireSoundClip;
}