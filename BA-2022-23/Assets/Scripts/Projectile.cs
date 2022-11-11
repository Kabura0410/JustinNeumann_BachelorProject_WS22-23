using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{

    public float speed;
    public float lifeTime;
    public float distance;
    public int damage;
    public LayerMask whatIsSolid;

    public GameObject destroyEffect;

    public float enemyKnockbackIntensity;

    
    private void Start()
    {
        Invoke("DestroyProjectile", lifeTime);
    }

    private void Update()
    {
        RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, Vector2.right, distance, whatIsSolid);
        if(hitInfo.collider != null)
        {
            if(hitInfo.collider.CompareTag("Enemy"))
            {
                Vector3 direction = hitInfo.transform.position - transform.position;
                if(direction.x > 0)
                {
                    hitInfo.collider.GetComponent<Enemy>().TakeDamage(damage, new Vector3(1,1,0), enemyKnockbackIntensity);
                }
                else
                {
                    hitInfo.collider.GetComponent<Enemy>().TakeDamage(damage, new Vector3(-1, 1, 0), enemyKnockbackIntensity);
                }
            }
            DestroyProjectile();
        }

        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    void DestroyProjectile()
    {
        GameObject go = Instantiate(destroyEffect, transform.position, Quaternion.identity);
        GameManager.instance.StartCoroutine(GameManager.instance.DeleteParticleDelayed(go, 2));
        Destroy(gameObject);
    }
}
