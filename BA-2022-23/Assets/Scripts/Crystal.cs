using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crystal : MonoBehaviour
{
    public int maxhealth;
    public int health;
    public GameObject explosionEffect;

    [SerializeField] private GameObject artifactDamageEffect;

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
        GameObject newParticle = Instantiate(artifactDamageEffect, transform.position, Quaternion.identity);
        GameManager.instance.StartCoroutine(GameManager.instance.DeleteParticleDelayed(newParticle, 4));
        if (health <= 0)
        {
            GameObject go = Instantiate(explosionEffect, transform.position, Quaternion.identity);
            GameManager.instance.StartCoroutine(GameManager.instance.DeleteParticleDelayed(go, 10));
            GameManager.instance.LoseGame();
        }
    }

    public void Heal(int _amount)
    {
        if(health + _amount > maxhealth)
        {
            health = maxhealth;
        }
        else
        {
            health += _amount;
        }
        GameManager.instance.UpdateHealthBars();
    }
}
