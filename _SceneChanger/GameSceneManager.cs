using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // 씬 전환을 위한 네임스페이스 추가

public class GameSceneManager : MonoBehaviour
{
	void Awake()
	{
		// ========== 수정: Build & Run 시 해상도 1920×1080으로 강제 설정 ============
        Screen.SetResolution(1920, 1080, true);
	}

	// Use this for initialization
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{

	}
	
	// 시작 버튼 클릭 시 호출 - "Game_Scene"으로 전환
    public void StartButton() {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game_Scene");
    }
	
	public void HomeButton() {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main_Scene");
    }

    // 종료 버튼 클릭 시 호출 - 애플리케이션 종료
	public void Exit()
	{
		Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // 에디터에서 종료용
#endif
	}
}
