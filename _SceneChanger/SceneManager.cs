using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // 씬 전환을 위한 네임스페이스 추가

public class SceneManager : MonoBehaviour
{

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

    // 종료 버튼 클릭 시 호출 - 애플리케이션 종료
    public void Exit() {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // 에디터에서 종료용
#endif
    }
}
