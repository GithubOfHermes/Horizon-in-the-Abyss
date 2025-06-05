using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLevel : MonoBehaviour
{
    public int currentExp = 0;
    public int expGauge = 10;
    public int level = 1;
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
            level++;
            expGauge += 10;
            stat.LevelUp();
        }
    }
}

