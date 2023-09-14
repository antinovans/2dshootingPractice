using UnityEngine;

[CreateAssetMenu(menuName = "EnemyConfig")]
public class EnemyConfig : ScriptableObject
{
    [Header("逻辑")] 
    public EnemyType type;
    public float defaultSpeed;
    public int hp;
    public int attackDmg;
    [Header("表现")] public float onDiedPauseTime;
    public AudioClip onHitclip;
    public AudioClip onDieclip;
    public AudioClip onAttackClip;
    public GameObject deadEnemyObj;
}