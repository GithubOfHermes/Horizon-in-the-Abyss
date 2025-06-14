using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Coin_UI : MonoBehaviour
{
    public PlayerStat playerStat;
    public Text Coin_Text;

    void Start()
    {
        if (playerStat == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                playerStat = player.GetComponent<PlayerStat>();
        }
    }

    void Update()
    {
        if (playerStat != null && Coin_Text != null)
        {
            Coin_Text.text = playerStat.currentCoin.ToString();
        }
    }
}

