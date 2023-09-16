using UnityEngine;

[System.Serializable]
public class EnemyWaveKeyPair
{
    public EnemyType enemyType;
    public int enemyCount;
}

[System.Serializable]
public class EnemyWaveStats
{
    public EnemyWaveKeyPair[] enemyWaveKeyPairs;
}
[CreateAssetMenu(menuName = "LevelConfig")]
public class LevelConfig : ScriptableObject
{
    public EnemyWaveStats[] enemyWaveStatsArray;
    public float[] enemyGenerationInterval;
}

