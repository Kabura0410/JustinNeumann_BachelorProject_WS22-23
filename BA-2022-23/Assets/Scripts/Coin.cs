using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public int value;

    public float despawnTime;


    private void Start()
    {
        StartCoroutine(DespawnCoinsAfterXSeconds(despawnTime));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SoundManager.instance.PlayCoinSound();
            GameManager.instance.player.IncreaseCoins(value);
            Destroy(gameObject);
        }
    }

    private IEnumerator DespawnCoinsAfterXSeconds(float _time)
    {
        yield return new WaitForSeconds(_time);
        Destroy(this.gameObject);
    }

}
