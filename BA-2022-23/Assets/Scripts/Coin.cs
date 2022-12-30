using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public int value;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SoundManager.instance.PlayCoinSound();
            GameManager.instance.player.IncreaseCoins(value);
            Destroy(gameObject);
        }
    }
}
