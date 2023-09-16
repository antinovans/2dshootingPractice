using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;
    [SerializeField] private LevelConfig[] configs;
    [SerializeField] private EnemySpawnController[] spawners;
    private AudioSource audioSource;
    public int currentLevel = 0;
    private int totalEnemyInThisLevel = 0;
    private void Awake()
    {
        if(instance!= null)
            Destroy(gameObject);
        else
        {
            instance = this;
        }
        // Init();
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        GameManager.instance.onEnemyDie += CheckIsLevelFinished;
        PrepareLevel();
    }

    public void PrepareLevel()
    {
        totalEnemyInThisLevel = 0;
        if (currentLevel == configs.Length)
        {
            return;
        }

        for (int i = 0; i < configs[currentLevel].enemyWaveStatsArray.Length; i++)
        {
            totalEnemyInThisLevel += spawners[i].PrepareLevel(configs[currentLevel].enemyWaveStatsArray[i],
                configs[currentLevel].enemyGenerationInterval[i]);
        }

        foreach (var spawner in spawners)
        {
            spawner.BeginSpawn();
        }
        currentLevel++;
    }

    private void CheckIsLevelFinished(EnemyConfig config, Vector2 diePos)
    {
        totalEnemyInThisLevel--;
        if (totalEnemyInThisLevel == 0)
        {
            //tell the camera to zoom in on the enemy
            GameManager.instance.onCameraZoomAt?.Invoke(diePos, 2f);
            //time pause a little bit
            GameManager.instance.StopTime(2f);
            //play finished sound
            audioSource.Play();
            StartCoroutine(ClearLevel(2f));
        }
    }

    private IEnumerator ClearLevel(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        GameManager.instance.onLevelClear?.Invoke(currentLevel == configs.Length);
    }

}