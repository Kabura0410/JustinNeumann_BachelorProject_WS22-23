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

    public GameObject flashEffectRight;
    public GameObject flashEffectLeft;

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
    private float currentReloadTime;


    [SerializeField] private AudioSource gunshotSound;

    [SerializeField] private Animator anim;

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

        transform.localScale = difference.x < 0 ? new Vector3(transform.localScale.x, -1, transform.localScale.z) : new Vector3(transform.localScale.x, 1, transform.localScale.z);

        if (actualAmmo <= 0 && !reloading)
        {
            StartCoroutine(ReloadWeapon());
        }

        if (timeBtwShots <= 0 && actualAmmo > 0 && !reloading)
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
                    CameraShake.instance.DoCameraShake();
                    actualAmmo--;
                    gunshotSound.Play();
                    anim.SetTrigger("shot");
                    if(actualAmmo <= 0)
                    {
                        StartCoroutine(ReloadWeapon());
                    }
                    timeBtwShots = startTimeBtwShots;
                    if(difference.x < 0)
                    {
                        GameObject newParticle = Instantiate(flashEffectRight, shotPoint.position, Quaternion.Euler(0f, 0f, rotZ + offset));
                        GameManager.instance.StartCoroutine(GameManager.instance.DeleteParticleDelayed(newParticle, 8));
                    }
                    else
                    {
                        GameObject newParticle = Instantiate(flashEffectLeft, shotPoint.position, Quaternion.Euler(0f, 0f, rotZ + offset));
                        GameManager.instance.StartCoroutine(GameManager.instance.DeleteParticleDelayed(newParticle, 8));
                    }
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

        if (Input.GetKeyDown(KeyCode.R) && actualAmmo != ammoCapacity)
        {
            StartCoroutine(ReloadWeapon());
        }

        if (reloading)
        {
            currentReloadTime += Time.deltaTime;
            GameManager.instance.player.reloadFillImage.fillAmount = currentReloadTime / reloadTime;
        }

    }

    private IEnumerator ReloadWeapon()
    {
        if (!reloading)
        {
            GameManager.instance.ToggleReloadIndicator();
            reloading = true;
            yield return new WaitForSecondsRealtime(reloadTime);
            _chargeTime = 0;
            actualAmmo = ammoCapacity;
            GameManager.instance.UpdateWeaponText();
            GameManager.instance.ToggleReloadIndicator();
            reloading = false;
            currentReloadTime = 0;
        }
        else
        {
            yield return null;
        }
    }
}
