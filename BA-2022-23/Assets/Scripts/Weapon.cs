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

    public int ammoCapacity;
    [HideInInspector] public int actualAmmo;

    [SerializeField] private float chargeTime;
    private float _chargeTime;

    [HideInInspector] public bool reloading;
    private void Awake()
    {
        actualAmmo = ammoCapacity;
    }

    private void Start()
    {
    }

    private void Update()
    {
        Vector3 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rotZ + offset);

        if (actualAmmo <= 0 && !reloading)
        {
            StartCoroutine(ReloadWeapon());
        }

        if (timeBtwShots <= 0 && actualAmmo > 0)
        {
            if (Input.GetMouseButton(0))
            {
                if(_chargeTime >= chargeTime)
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
                    GameManager.instance.UpdateWeaponText();
                }
                else
                {
                    _chargeTime += Time.deltaTime; 
                }
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            _chargeTime = 0;
        }
        else
        {
            timeBtwShots -= Time.deltaTime;
        }
        
    }

    private IEnumerator ReloadWeapon()
    {
        GameManager.instance.ToggleReloadIndicator();
        reloading = true;
        yield return new WaitForSecondsRealtime(reloadTime);
        _chargeTime = 0;
        actualAmmo = ammoCapacity;
        GameManager.instance.UpdateWeaponText();
        GameManager.instance.ToggleReloadIndicator();
        reloading = false;
    }
}
