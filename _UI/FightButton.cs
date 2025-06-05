using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FightButton : MonoBehaviour
{
    public Button fightButton;
    public GameObject[] players;
    public GameObject[] enemies;

    void Start()
    {
        fightButton.onClick.AddListener(OnFightStart);
    }

    void OnFightStart()
    {
        fightButton.gameObject.SetActive(false);

        foreach (var player in players)
        {
            // 드래그 먼저 꺼주기 (반드시 Controller 활성화 전에)
            var draggable = player.GetComponent<PlayerDraggable>();
            if (draggable != null && draggable.enabled)
            {
                draggable.enabled = false;
            }

            // 그 다음 전투 컨트롤러 켜기
            player.GetComponent<PlayerController>().enabled = true;
        }
    }
}

