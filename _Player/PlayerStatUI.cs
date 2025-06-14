using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatUI : MonoBehaviour
{
    public GameObject player;
    public GameObject statPanel;

    public Text levelText;
    public Text attackText;
    public Text skillDamageText;
    public Text attackSpeedText;
    public Text defenseText;

    private PlayerStat stat;

    void Start()
    {
        stat = player.GetComponent<PlayerStat>();
        statPanel.SetActive(false);   // 시작 시 비활성화
        UpdateUI();
    }

    void Update()
    {
        // UI가 켜져 있으면 수시로 갱신
        if (statPanel.activeSelf)
            UpdateUI();

        // 마우스(터치) 클릭이 들어오면
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            // 플레이어를 클릭했을 때만 토글
            if (Physics.Raycast(ray, out hit) && hit.collider.gameObject == player)
            {
                statPanel.SetActive(!statPanel.activeSelf);
            }
            // else { /* 다른 곳 클릭 시에는 아무 동작도 하지 않음 */ }
        }
    }

    void UpdateUI()
    {
        levelText.text       = "Lv. " + stat.level;
        attackText.text      = "공격력: " + stat.attackDamage;
        skillDamageText.text = "스킬데미지: " + stat.skillDamage;
        attackSpeedText.text = "공격속도: " + stat.attackSpeed.ToString("F1");
        defenseText.text     = "방어력: " + Mathf.RoundToInt(stat.defense) + "%";
    }
}


