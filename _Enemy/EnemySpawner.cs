using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject[] enemyPrefabs; // Enemy 프리팹들 (Close, Assasin, Tanker, Wizard 등)
    public Transform[] spawnPoints;   // 소환 위치들
    public GameObject player;

    public int maxSpawnCount = 10;
    private int currentSpawnCount = 0;

    void Start()
    {
        SpawnEnemiesByPlayerLevel();
    }

    void SpawnEnemiesByPlayerLevel()
    {
        PlayerLevel playerLevel = player.GetComponent<PlayerLevel>();
        PlayerStat playerStat = player.GetComponent<PlayerStat>();

        if (playerLevel == null || playerStat == null) return;

        int playerLv = playerStat.level;

        // 개체 수: 짝수 레벨마다 증가 (최대 10마리)
        int spawnCount = Mathf.Min(1 + playerLv / 2, maxSpawnCount);

        for (int i = 0; i < spawnCount; i++)
        {
            int prefabIndex = Random.Range(0, 4); // Close~Wizard 중 하나
            GameObject enemy = Instantiate(enemyPrefabs[prefabIndex], GetRandomSpawnPosition(), Quaternion.identity);

            EnemyStat stat = enemy.GetComponent<EnemyStat>();
            if (stat != null && stat.enemyType != EnemyStat.Enemy_Type._1F_Elete && stat.enemyType != EnemyStat.Enemy_Type._1F_Boss)
            {
                // EnemyStat 강화
                stat.Enemy_MaxHP += 50 * (playerLv - 1); // Lv 1 → +0, Lv 2 → +50, Lv 3 → +100 ...
                stat.Enemy_currentHP = stat.Enemy_MaxHP;

                if (playerLv % 5 == 0) // 공속 증가: 10% 향상
                {
                    stat.Enemy_attackSpeed *= 0.9f;
                }
            }

            currentSpawnCount++;
        }
    }

    Vector3 GetRandomSpawnPosition()
    {
        if (spawnPoints.Length == 0) return transform.position;

        Transform sp = spawnPoints[Random.Range(0, spawnPoints.Length)];
        return sp.position;
    }
}
