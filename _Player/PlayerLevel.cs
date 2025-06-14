using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLevel : MonoBehaviour
{
    public int currentExp = 0;
    public int expGauge = 10;
    private PlayerStat stat;

    void Start()
    {
        stat = GetComponent<PlayerStat>();
    }

    public void GainExp(int exp)
    {
        currentExp += exp;

        while (currentExp >= expGauge)
        {
            currentExp -= expGauge;
            expGauge += 10;
            stat.LevelUp();
        }
    }
}

