using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapManager : MonoBehaviour
{
    public static MinimapManager Instance;

    [Header("UI 참조")]
    public GameObject minimapPanel;
    public Button minimapToggleButton;
    public RectTransform iconsParent;     // Inspector에서 Width=1200, Height=700 설정
    public GameObject roomIconPrefab;  // Room_Button 프리팹
    public float uiSpacing = 100f;

    [Header("방 타입별 이미지")]
    public Sprite[] roomImages;      // 0:Start,1:Normal,2:Clear/Shop,3:Elite,4:Boss,5:Boss

    [Header("Persistent RoomTrigger")]
    public RoomTrigger persistentRoom;  // 수정: 씬에 배치된 RoomTrigger 오브젝트를 인스펙터에서 반드시 연결

    private List<RoomTrigger> mapRooms;
    private List<Button> iconButtons = new List<Button>();
    private List<Image> iconBackgrounds = new List<Image>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        minimapToggleButton.onClick.AddListener(() =>
            minimapPanel.SetActive(!minimapPanel.activeSelf)
        );
    }

    /// <summary>
    /// rooms: 생성된 방 리스트
    /// roomSpacing: 방 간격 (월드 단위)
    /// </summary>
    public void SetupMap(List<RoomTrigger> rooms, float roomSpacing)
    {
        mapRooms = rooms;  // 수정: mapRooms에 방 리스트 저장

        // 기존 아이콘 모두 제거
        foreach (Transform c in iconsParent)
            Destroy(c.gameObject);
        iconButtons.Clear();
        iconBackgrounds.Clear();

        // 아이콘 생성 및 클릭 리스너 등록
        for (int i = 0; i < rooms.Count; i++)
        {
            var rt = rooms[i];
            var icon = Instantiate(roomIconPrefab, iconsParent);
            var btn = icon.GetComponent<Button>();
            int idx = i;

            // ========== 수정: 버튼 생성 직후 접근 불가 방의 interactable 설정 ============
            if (rt.roomType == RoomType.Elite && StageManager.Instance.clearCount < 7)
                btn.interactable = false;
            if (rt.roomType == RoomType.Boss && (StageManager.Instance.clearCount < 7 || StageManager.Instance.eliteClearCount < 1))
                btn.interactable = false;

            // 배경 이미지 설정
            Image bg = icon.transform.Find("Room_Button_Background").GetComponent<Image>();
            bg.sprite = roomImages[(int)rt.roomType];
            iconBackgrounds.Add(bg);

            // ========== 수정: 버튼 리스트에 버튼을 한 번만 추가 (중복된 Add 제거) ============
            iconButtons.Add(btn);

            btn.onClick.AddListener(() =>
            {
                if (mapRooms[idx].roomType == RoomType.Clear) return;
                if (mapRooms[idx].roomType == RoomType.Elite && StageManager.Instance.clearCount < 7) return;
                if (mapRooms[idx].roomType == RoomType.Boss
                    && (StageManager.Instance.clearCount < 7 || StageManager.Instance.eliteClearCount < 1)) return;

                StageManager.Instance.EnterRoom(mapRooms[idx]);
                minimapPanel.SetActive(false);
            });
        }

        // 월드 좌표 범위 계산 (X = pos.x, Y = pos.z)
        float minX = float.MaxValue, maxX = float.MinValue;
        float minY = float.MaxValue, maxY = float.MinValue;
        for (int i = 0; i < rooms.Count; i++)
        {
            var p = rooms[i].transform.position;
            if (p.x < minX) minX = p.x;
            if (p.x > maxX) maxX = p.x;
            if (p.z < minY) minY = p.z;
            if (p.z > maxY) maxY = p.z;
        }

        // 아이콘 위치 재배치
        for (int i = 0; i < rooms.Count; i++)
        {
            var rect = iconsParent.GetChild(i)
                                  .GetComponent<RectTransform>();
            float ax = ((rooms[i].transform.position.x - minX)
                        / roomSpacing) * uiSpacing + 75f;      // 수정: 반 아이콘 크기(75) 오프셋
            float ay = ((rooms[i].transform.position.z - minY)
                        / roomSpacing) * uiSpacing + 75f;      // 수정: 반 아이콘 크기(75) 오프셋
            rect.anchoredPosition = new Vector2(ax, ay);
        }
    }

    /// <summary>
    /// 특정 방이 Clear 상태가 되었을 때 호출
    /// </summary>
    public void UpdateIconState(RoomTrigger room, RoomType newType)
    {
        int idx = mapRooms.IndexOf(room);
        if (idx < 0) return;
        iconBackgrounds[idx].sprite = roomImages[(int)newType];
        iconButtons[idx].interactable = (newType != RoomType.Clear);
    }
    
    public void SetupMapAgain()
    {
        for (int i = 0; i < mapRooms.Count; i++)
        {
            var rt = mapRooms[i];
            var btn = iconButtons[i];

            // ========== 수정: 조건에 따라 인터랙션 가능 여부 재설정 ============
            if (rt.roomType == RoomType.Elite)
                btn.interactable = (StageManager.Instance.clearCount >= 7);

            if (rt.roomType == RoomType.Boss)
                btn.interactable = (StageManager.Instance.clearCount >= 7 &&
                                    StageManager.Instance.eliteClearCount >= 1);
        }
    }
}