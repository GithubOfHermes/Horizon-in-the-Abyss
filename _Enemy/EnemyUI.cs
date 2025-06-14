using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUI : MonoBehaviour
{
    public Slider Enemy_HP_Bar;
    private EnemyStat enemyStat;

    IEnumerator Start()
    {
        enemyStat = GetComponent<EnemyStat>();
        yield return null;  // 한 프레임 딜레이
        Enemy_HP_Bar.maxValue = enemyStat.Enemy_MaxHP;
        Enemy_HP_Bar.value    = enemyStat.Enemy_currentHP;
    }

    void OnEnable()
    {
        enemyStat = GetComponent<EnemyStat>();
        Enemy_HP_Bar.maxValue = enemyStat.Enemy_MaxHP;
        Enemy_HP_Bar.value    = enemyStat.Enemy_currentHP;
    }

    void Update()
    {
        Enemy_HP_Bar.value = Mathf.Clamp(enemyStat.Enemy_currentHP, 0, enemyStat.Enemy_MaxHP);
    }
}

