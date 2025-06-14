using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStat : MonoBehaviour
{
    [Header("Player_Stat")]
    public int level = 1;

    public int attackDamage = 100;
    public int skillDamage = 200;
    public float attackSpeed = 1.0f;
    public float attackRange = 6.5f;

    public int maxHP = 1000;
    public int currentHP = 1000;

    public int maxMP = 100;
    public int currentMP = 0;

    public float defense = 0;

    public int currentCoin = 0;

    public void LevelUp()
    {
        level++;

        // 1) 공격력 증가 계산 (버프 적용)
        int baseIncAttack = 20;                                                       // ■ 수정 전: attackDamage += 20
        GadgetEffectHandler geh = GetComponent<GadgetEffectHandler>();
        float atkMul = (geh != null ? geh.GetAttackMultiplier() : 1f);
        int incAtk = Mathf.RoundToInt(baseIncAttack * atkMul);                        // ■ 수정: 버프 곱한 증가치 계산
        attackDamage += incAtk;                                                       // ■ 수정: 증가치 반영

        // 2) 스킬 데미지 증가 계산 (버프 적용)
        int baseIncSkill = 30;                                                         // ■ 수정 전: skillDamage += 30
        float skillMul = (geh != null ? geh.GetSkillMultiplier() : 1f);
        int incSkill = Mathf.RoundToInt(baseIncSkill * skillMul);                     // ■ 수정: 버프 곱한 증가치 계산
        skillDamage += incSkill;                                                       // ■ 수정: 증가치 반영

        // 3) 공격속도 증가 계산 (5레벨 단위, Bow 버프 적용)
        float incSpeed = 0f;
        if (level % 5 == 0 && level > 1)
        {
            float baseIncSpeed = 0.5f;                                                 // ■ 수정 전: attackSpeed += 0.5f
            float speedMul     = (geh != null ? geh.GetSpeedMultiplier() : 1f);
            incSpeed = baseIncSpeed * speedMul;                                        // ■ 수정: 버프 곱한 증가치 계산
            attackSpeed += incSpeed;                                                   // ■ 수정: 증가치 반영
        }

        // 4) 최대 체력 증가 계산 (Armor 버프 적용)
        int baseIncHP = 100;                                                           // ■ 수정 전: maxHP += 100
        float hpMul   = (geh != null ? geh.GetHPMultiplier() : 1f);
        int incHP = Mathf.RoundToInt(baseIncHP * hpMul);                               // ■ 수정: 버프 곱한 증가치 계산
        maxHP += incHP;                                                                // ■ 수정: 증가치 반영
        currentHP = Mathf.Min(currentHP + incHP, maxHP);

        // 5) 방어력 증가 (변경 없음)
        defense += 2f;
        if (defense > 50f) defense = 50f;

        // 레벨업으로 계산된 증가분 Δ(incAtk, incSkill, incSpeed, incHP)를
        // GadgetEffectHandler 기록에도 누적시켜 줌 (제거 시 겹쳐 빠지지 않도록)
        if (geh != null)
            geh.UpdateBaseStats();
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

