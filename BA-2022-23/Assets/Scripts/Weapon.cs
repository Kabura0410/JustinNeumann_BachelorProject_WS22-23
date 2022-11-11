using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
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


    [SerializeField] private float reloadTime;

    [SerializeField] private int ammoCapacity;
    private int actualAmmo;

    private void Start()
    {
        actualAmmo = ammoCapacity;
    }

    private void Update()
    {
        Vector3 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rotZ + offset);

        if(timeBtwShots <= 0 && actualAmmo > 0)
        {
            if (Input.GetMouseButton(0))
            {
                for(int i = 0; i < bulletAmount; i++)
                {
                    GameObject go = Instantiate(projectile, shotPoint.position, transform.rotation);
                    go.transform.Rotate(new Vector3(0,0,Random.Range(-spreadFactor, spreadFactor)));
                    go.GetComponent<Projectile>().enemyKnockbackIntensity = enemyKnockback;
                }
                actualAmmo--;
                if(actualAmmo <= 0)
                {
                    StartCoroutine(ReloadWeapon());
                }
                timeBtwShots = startTimeBtwShots;
                GameObject newParticle = Instantiate(flashEffect, shotPoint.position, Quaternion.identity);
                GameManager.instance.StartCoroutine(GameManager.instance.DeleteParticleDelayed(newParticle, 2));
                GameManager.instance.player.GetKnockback(-difference.normalized, playerKnockback, GameManager.instance.player.weaponKnockbackDuration);
            }
        }
        else
        {
            timeBtwShots -= Time.deltaTime;
        }
        
    }

    private IEnumerator ReloadWeapon()
    {
        yield return new WaitForSecondsRealtime(reloadTime);
        actualAmmo = ammoCapacity; 
    }
}
