using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStat : MonoBehaviour
{
    public enum Enemy_Type { Close, Assasin, Tanker, Wizard, _1F_Elete, _1F_Boss }
    public Enemy_Type enemyType;

    public int Enemy_MaxHP;
    public int Enemy_currentHP;

    public int Enemy_attack;
    public float Enemy_attackSpeed;
    public float Enemy_attackRange;

    public int Enemy_Coin;
    public int Enemy_Exp;

    void Awake()
    {
        ApplyEnemyStats();   // Start → Awake
    }

    void Start()
    {

    }

    public void ApplyEnemyStats()
    {
        switch (enemyType)
        {
            case Enemy_Type.Close:
                Enemy_MaxHP = 500;
                Enemy_attack = 90;
                Enemy_attackSpeed = 3.0f;
                Enemy_attackRange = 6.5f;
                Enemy_Coin = Random.Range(30, 61);
                Enemy_Exp = 10;
                break;

            case Enemy_Type.Assasin:
                Enemy_MaxHP = 300;
                Enemy_attack = 120;
                Enemy_attackSpeed = 2.0f;
                Enemy_attackRange = 5.0f;
                Enemy_Coin = Random.Range(30, 61);
                Enemy_Exp = 10;
                break;

            case Enemy_Type.Tanker:
                Enemy_MaxHP = 650;
                Enemy_attack = 120;
                Enemy_attackSpeed = 4.0f;
                Enemy_attackRange = 5.0f;
                Enemy_Coin = Random.Range(30, 61);
                Enemy_Exp = 10;
                break;

            case Enemy_Type.Wizard:
                Enemy_MaxHP = 300;
                Enemy_attack = 110;
                Enemy_attackSpeed = 5.0f;
                Enemy_attackRange = 30.0f;
                Enemy_Coin = Random.Range(30, 61);
                Enemy_Exp = 10;
                break;

            case Enemy_Type._1F_Elete:
                Enemy_MaxHP = 7000;
                Enemy_attack = 120;
                Enemy_attackSpeed = 2.0f;
                Enemy_attackRange = 100.0f;
                Enemy_Coin = 300;
                Enemy_Exp = 100;
                break;

            case Enemy_Type._1F_Boss:
                Enemy_MaxHP = 10000;
                Enemy_attack = 170;
                Enemy_attackSpeed = 2.0f;
                Enemy_attackRange = 100.0f;
                Enemy_Coin = 500;
                Enemy_Exp = 200;
                break;
        }

        Enemy_currentHP = Enemy_MaxHP;
    }
}

