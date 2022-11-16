using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{

    public float maxSpeed;
    public float minSpeed;
    private float actualSpeed;

    public float lifeTime;
    public float distance;
    public int damage;
    public LayerMask whatIsSolid;

    public GameObject destroyEffect;

    public float enemyKnockbackIntensity;

    public float playerKnockbackIntensity;

    private Vector3 startPos;

    public int hitAmount;

    private List<Enemy> hitEnemies = new List<Enemy>();

    public enum ProjectileType
    {
        PlayerBullet,
        EnemyBullet
    }

    public ProjectileType projectileType;
    
    private void Start()
    {
        CalculateSpeed();
        startPos = transform.position;
        Invoke("DestroyProjectile", lifeTime);
    }

    private void Update()
    {
        RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, Vector2.right, distance, whatIsSolid);
        if(hitInfo.collider != null)
        {
            switch (projectileType)
            {
                case ProjectileType.EnemyBullet:
                    if (hitInfo.collider.CompareTag("Player"))
                    {
                        PlayerController player = hitInfo.collider.GetComponent<PlayerController>();
                        Vector3 direction = hitInfo.transform.position - startPos;
                        if (direction.x > 0)
                        {
                            player.GetDamage(damage, new Vector3(1, 1, 0), enemyKnockbackIntensity, player.enemyKnockbackDuration);
                        }
                        else
                        {
                            player.GetDamage(damage, new Vector3(-1, 1, 0), enemyKnockbackIntensity, player.enemyKnockbackDuration);
                        }
                        DestroyProjectile();
                    }
                    else if (hitInfo.collider.CompareTag("Crystal"))
                    {
                        GameManager.instance.crystal.GetDamage(damage);
                        DestroyProjectile();
                    }
                    else
                    {
                        DestroyProjectile();
                    }
                    break;
                case ProjectileType.PlayerBullet:
                    #region Enemy logic
                    if (hitInfo.collider.CompareTag("Enemy"))
                    {
                        if (!hitEnemies.Contains(hitInfo.transform.GetComponent<Enemy>()))
                        {
                            hitEnemies.Add(hitInfo.transform.gameObject.GetComponent<Enemy>());
                            Vector3 direction = hitInfo.transform.position - startPos;
                            if (direction.x > 0)
                            {
                                hitInfo.collider.GetComponent<Enemy>().TakeDamage(damage, new Vector3(1, 1, 0), enemyKnockbackIntensity);
                            }
                            else
                            {
                                hitInfo.collider.GetComponent<Enemy>().TakeDamage(damage, new Vector3(-1, 1, 0), enemyKnockbackIntensity);
                            }
                            if (hitAmount <= 0)
                            {
                                DestroyProjectile();
                            }
                            else
                            {
                                hitAmount--;
                            }
                        }
                    }
                    else
                    {
                        DestroyProjectile();
                    }
                    #endregion
                    break;
            }
        }
    }

    private void FixedUpdate()
    {
        transform.Translate(Vector2.right * actualSpeed * Time.fixedDeltaTime);
    }

    void DestroyProjectile()
    {
        GameObject go = Instantiate(destroyEffect, transform.position, Quaternion.identity);
        GameManager.instance.StartCoroutine(GameManager.instance.DeleteParticleDelayed(go, 2));
        Destroy(gameObject);
    }

    private void CalculateSpeed()
    {
        actualSpeed = Random.Range(minSpeed, maxSpeed);
    }
}
