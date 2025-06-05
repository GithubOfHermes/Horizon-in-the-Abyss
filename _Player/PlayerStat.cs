using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStat : MonoBehaviour
{
    public int level = 1;

    public int attackDamage = 10;
    public int skillDamage = 20;
    public float attackSpeed = 1.0f;
    public float attackRange = 6.5f;

    public int maxHP = 100;
    public int currentHP = 100;

    public int maxMP = 100;
    public int currentMP = 0;

    public float defense = 0;

    public void LevelUp()
    {
        level++;
        attackDamage += 2;
        skillDamage += 2;
        if (level % 5 == 1 && level > 1)
        {
            attackSpeed += 0.5f;
        }

        maxHP += 10;
        currentHP = Mathf.Min(currentHP + 10, maxHP);
    }

    public void GainMP(int amount)
    {
        currentMP += amount;
        if (currentMP >= maxMP)
        {
            currentMP = maxMP;
        }
    }

    public void TakeDamage(int rawDamage)
    {
        float damageMultiplier = 1.0f - (defense * 0.01f);
        int finalDamage = Mathf.CeilToInt(rawDamage * damageMultiplier);

        currentHP -= finalDamage;
        GainMP(5); // 데미지 받을 때 MP 회복
    }
}

