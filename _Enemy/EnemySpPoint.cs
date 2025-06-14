using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Unity 5.6.7f1에서는 Vector2Int가 없기 때문에 직접 정의합니다.
[System.Serializable]
public struct Vector2Int
{
    public int x;
    public int y;

    public Vector2Int(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public override bool Equals(object obj)
    {
        if (!(obj is Vector2Int)) return false;
        Vector2Int other = (Vector2Int)obj;
        return x == other.x && y == other.y;
    }

    public override int GetHashCode()
    {
        return x * 397 ^ y;
    }

    public static bool operator ==(Vector2Int a, Vector2Int b)
    {
        return a.x == b.x && a.y == b.y;
    }

    public static bool operator !=(Vector2Int a, Vector2Int b)
    {
        return !(a == b);
    }
}

public class EnemySpPoint : MonoBehaviour
{
    public GridManager gridManager;
    public GameObject[] enemyPrefabs;
    public GameObject player;

    private List<Vector2Int> usedCells = new List<Vector2Int>();

    void Start()
    {
        
    }

    public void SpawnEnemiesUnique()
    {
        PlayerStat playerStat = player.GetComponent<PlayerStat>();
		int spawnCount = Mathf.Min(1 + playerStat.level / 2, 10);

        for (int i = 0; i < spawnCount; i++)
        {
            int index = Random.Range(0, 4); // Close~Wizard
            GameObject prefab = enemyPrefabs[index];

            Vector3 spawnPos = GetUniqueGridPosition(prefab);
            GameObject enemy = Instantiate(prefab, spawnPos, Quaternion.Euler(0, 180, 0));

            // 강화 적용은 EnemySpawner 또는 EnemyStat에서 처리됨
        }
    }

    Vector3 GetUniqueGridPosition(GameObject enemyPrefab)
    {
        EnemyStat stat = enemyPrefab.GetComponent<EnemyStat>();

        int attempts = 100;
        while (attempts-- > 0)
        {
            int x = Random.Range(gridManager.minX, gridManager.maxX + 1);
            int z;

            // Wizard 타입은 z=2~3 고정
            if (stat != null && stat.enemyType == EnemyStat.Enemy_Type.Wizard)
            {
                z = Random.Range(2, 4); // 2 or 3
            }
            else
            {
                z = Random.Range(gridManager.minZ, gridManager.maxZ + 1);
            }

            Vector2Int cell = new Vector2Int(x, z);
            if (usedCells.Contains(cell)) continue;

            usedCells.Add(cell);
            Vector3 pos = gridManager.originPosition + new Vector3(x * gridManager.cellSize, 0f, z * gridManager.cellSize);
            return pos;
        }

        return transform.position; // fallback
    }
}
