using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crystal : MonoBehaviour
{
    public int maxhealth;
    public int health;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void GetDamage(int _amount)
    {
        health -= _amount;
        GameManager.instance.UpdateHealthBars();
        if(health <= 0)
        {
            GameManager.instance.LoseGame();
        }
    }
}
