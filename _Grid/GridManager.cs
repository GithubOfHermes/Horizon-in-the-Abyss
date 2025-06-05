using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public float cellSize = 4f;

    // 격자 범위 설정 (정수 단위)
    public int minX = 0;
    public int maxX = 5;
    public int minZ = 0;
    public int maxZ = 5;

    public Vector3 originPosition = Vector3.zero; // 기준점 (왼쪽 아래)

    public GameObject cellPrefab; // 얇은 Quad 프리팹 (투명 셀)

    private void Start()
    {
        DrawGridVisuals();
    }

    void DrawGridVisuals()
    {
        for (int x = minX; x <= maxX; x++)
        {
            for (int z = minZ; z <= maxZ; z++)
            {
                Vector3 pos = originPosition + new Vector3(x * cellSize, 0.01f, z * cellSize);

                GameObject cell = Instantiate(cellPrefab, transform);
                cell.transform.position = pos;

                // 스케일 조정이 필요 없다면 아래 줄은 생략
                // cell.transform.localScale = new Vector3(...); ← 주석 처리
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.gray;

        for (int x = minX; x <= maxX; x++)
        {
            for (int z = minZ; z <= maxZ; z++)
            {
                Vector3 cellCenter = originPosition + new Vector3(x * cellSize, 0, z * cellSize);
                Gizmos.DrawWireCube(cellCenter, new Vector3(cellSize, 0.01f, cellSize));
            }
        }
    }

    public Vector3 GetNearestGridPosition(Vector3 worldPosition)
    {
        // 월드 위치에서 기준점을 뺀 뒤 cell 단위로 환산
        Vector3 localPos = worldPosition - originPosition;

        int gridX = Mathf.RoundToInt(localPos.x / cellSize);
        int gridZ = Mathf.RoundToInt(localPos.z / cellSize);

        if (gridX < minX || gridX > maxX || gridZ < minZ || gridZ > maxZ)
            return transform.position; // 범위 밖이면 이동 없음

        Vector3 snappedLocal = new Vector3(gridX * cellSize, 0, gridZ * cellSize);
        return originPosition + snappedLocal;
    }

    public bool TryGetNearestGridPosition(Vector3 worldPosition, out Vector3 snappedPosition)
	{
		Vector3 localPos = worldPosition - originPosition;

		int gridX = Mathf.RoundToInt(localPos.x / cellSize);
		int gridZ = Mathf.RoundToInt(localPos.z / cellSize);

		if (gridX < minX || gridX > maxX || gridZ < minZ || gridZ > maxZ)
		{
			snappedPosition = Vector3.zero;
			return false;
		}

		Vector3 snappedLocal = new Vector3(gridX * cellSize, 0, gridZ * cellSize);
		snappedPosition = originPosition + snappedLocal;
		return true;
	}
}

