using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// StageGenerator: 게임 시작 시 방들을 랜덤한 순서로 배치하고 생성합니다.
/// </summary>
public class StageGenerator : MonoBehaviour
{
    [Header("프리팹 & 설정")]
    public GameObject    roomPrefab;      // RoomTrigger가 붙은 방 프리팹
    public Transform     roomsParent;     // 방들을 자식으로 둘 부모 트랜스폼
    public float         roomSpacing = 70f; // 방 간 간격 (월드 좌표)
    public MinimapManager minimapManager; // 미니맵 매니저 참조

    private List<RoomTrigger> rooms = new List<RoomTrigger>();

    void Start()
    {
        GenerateStage();
        minimapManager.SetupMap(rooms, roomSpacing);
    }

    void GenerateStage()
    {
        int totalRooms = 11;  // 수정: 총 방 개수(시작1 + 일반7 + 특수3)

        // 1) 그리드 기반 무작위 트리 위치 생성
        Dictionary<int, Vector2> posGrid = new Dictionary<int, Vector2>();
        List<Vector2> occupied = new List<Vector2>();
        List<int> connected = new List<int>();

        posGrid[0] = Vector2.zero;
        occupied.Add(Vector2.zero);
        connected.Add(0);

        for (int i = 1; i < totalRooms; i++)
        {
            Vector2 newPos;
            int tries = 0;
            do
            {
                int baseIdx = connected[Random.Range(0, connected.Count)];
                Vector2 basePos = posGrid[baseIdx];
                Vector2 dir = new List<Vector2>          // 수정: 네 방향 중 랜덤
                {
                    Vector2.up, Vector2.down,
                    Vector2.left, Vector2.right
                }[Random.Range(0, 4)];
                newPos = basePos + dir;
                tries++;
            }
            while (occupied.Contains(newPos) && tries < 100);

            posGrid[i] = newPos;
            occupied.Add(newPos);
            connected.Add(i);
        }

        // 2) 잎 노드(leaves) 찾기
        List<int> leaves = new List<int>();
        for (int i = 0; i < totalRooms; i++)
        {
            int neighbors = 0;
            foreach (var kv in posGrid)
            {
                if (kv.Key != i &&
                    (posGrid[i] - kv.Value).magnitude == 1)
                    neighbors++;
            }
            if (neighbors == 1 && i != 0)
                leaves.Add(i);  // 수정: 루트(0) 제외
        }

        // 수정: 잎 노드가 3개 미만일 때 후보에서 채워넣기
        if (leaves.Count < 3)
        {
            List<int> candidates = new List<int>();
            for (int i = 1; i < totalRooms; i++)
                if (!leaves.Contains(i))
                    candidates.Add(i);
            for (int i = 0; i < candidates.Count; i++)
            {
                int j = Random.Range(i, candidates.Count);
                int tmp = candidates[i];
                candidates[i] = candidates[j];
                candidates[j] = tmp;
            }
            int idx = 0;
            while (leaves.Count < 3 && idx < candidates.Count)
            {
                leaves.Add(candidates[idx]);
                idx++;
            }
        }

        // 수정: 잎 노드 셔플
        for (int i = 0; i < leaves.Count; i++)
        {
            int j = Random.Range(i, leaves.Count);
            int tmp = leaves[i]; leaves[i] = leaves[j]; leaves[j] = tmp;
        }

        // 3) 순서 리스트 초기화 (Start + 모든 Normal)
        List<RoomType> ordered = new List<RoomType>();
        for (int i = 0; i < totalRooms; i++)
        {
            if (i == 0)
                ordered.Add(RoomType.Start);  // 수정: 시작방 1개
            else
                ordered.Add(RoomType.Normal); // 수정: 기본으로 Normal
        }

        // 4) 잎 노드에서만 Special 방 배치
        ordered[leaves[0]] = RoomType.Shop;   // 수정: 잎[0]에 Shop
        ordered[leaves[1]] = RoomType.Elite;  // 수정: 잎[1]에 Elite
        ordered[leaves[2]] = RoomType.Boss;   // 수정: 잎[2]에 Boss

        // 5) 월드에 방 생성
        for (int i = 0; i < totalRooms; i++)
        {
            Vector2 g = posGrid[i];
            Vector3 pos = new Vector3(
                g.x * roomSpacing,
                0f,
                g.y * roomSpacing);
            GameObject go = Instantiate(
                roomPrefab, pos,
                Quaternion.identity,
                roomsParent);
            RoomTrigger rt = go.GetComponent<RoomTrigger>();
            rt.roomType = ordered[i];           // 수정: ordered 대로 타입 설정
            rooms.Add(rt);
        }
    }
}