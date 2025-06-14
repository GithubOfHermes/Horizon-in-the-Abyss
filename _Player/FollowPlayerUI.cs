using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FollowPlayerUI : MonoBehaviour
{
    public string playerTag = "Player"; // Player 태그를 따라감
    public Transform target;          // 보이게 할 플레이어(혹은 머리) Transform
    public Vector3 worldOffset;       // world 상에서 위로 얼마나 띄울지
    private RectTransform rt;

    void Awake()
    {
        rt = GetComponent<RectTransform>();
        AssignTarget();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        // ========== 수정: 이벤트 해제 ==========
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        // 씬 리로드(=StartButton 호출) 후 Start() 에서도 보장
        if (target == null)
            AssignTarget();
    }

    // ========== 수정: 씬 로드 직후 플레이어 타겟 재할당 ============
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AssignTarget();
    }

    void AssignTarget()
    {
        GameObject p = GameObject.FindGameObjectWithTag(playerTag);
        if (p != null)
            target = p.transform;
        else
            Debug.LogWarning(
                "[" + name + "] “" + playerTag + "” 태그의 오브젝트를 찾지 못했습니다."
            );
    }

    void LateUpdate()
    {
        if (target == null || Camera.main == null)
            return;

        // 월드 좌표 → 화면 픽셀로 변환
        Vector3 screenPos = Camera.main.WorldToScreenPoint(
            target.position + worldOffset
        );
        rt.position = screenPos;
    }
}

