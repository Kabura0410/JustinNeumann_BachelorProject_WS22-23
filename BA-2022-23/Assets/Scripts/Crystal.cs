using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crystal : MonoBehaviour
{
    [SerializeField] private int health;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void GetDamage(int _amount)
    {
        health -= _amount;
        if(health <= 0)
        {
            GameManager.instance.LoseGame();
        }
    }
}
