using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeapon : MonoBehaviour
{

    public float offset;

    public GameObject projectile;
    public Transform shotPoint;

    private float timeBtwShots;
    public float startTimeBtwShots;

    public GameObject flashEffect;

    [SerializeField] private float spreadFactor;

    [SerializeField] private int bulletAmount;

    [SerializeField] private float playerKnockback;
    [SerializeField] private float enemyKnockback;

    [SerializeField] private float chargeTime;
    private float _chargeTime;

    public GameObject targetObject;

    [HideInInspector] private SpriteRenderer weaponSprite;

    private void Awake()
    {
        weaponSprite = GetComponentInChildren<SpriteRenderer>();
    }

    private void Update()
    {
        if(targetObject != null)
        {
            Vector3 difference = targetObject.transform.position - transform.position;
            float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, rotZ + offset);

            weaponSprite.flipY = difference.x < 0 ? true : false;

            if (timeBtwShots <= 0)
            {
                if(_chargeTime >= chargeTime)
                {
                    for(int i = 0; i < bulletAmount; i++)
                    {
                        GameObject go = Instantiate(projectile, shotPoint.position, transform.rotation);
                        go.transform.Rotate(new Vector3(0,0,Random.Range(-spreadFactor, spreadFactor)));
                        go.GetComponent<Projectile>().enemyKnockbackIntensity = enemyKnockback;
                    }
                    timeBtwShots = startTimeBtwShots;
                    GameObject newParticle = Instantiate(flashEffect, shotPoint.position, Quaternion.identity);
                    GameManager.instance.StartCoroutine(GameManager.instance.DeleteParticleDelayed(newParticle, 2));
                }
                else
                {
                    _chargeTime += Time.deltaTime; 
                }
            }
            else
            {
                timeBtwShots -= Time.deltaTime;
            }
        }
    }
}
