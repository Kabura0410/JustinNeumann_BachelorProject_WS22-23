using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public PlayerController player;

    [SerializeField] private List<GameObject> allWeapons;

    public Crystal crystal;

    public List<Wave> allWaves;

    private int currentWave;

    [SerializeField] private List<EnemySpawn> allEnemySpawns;

    [HideInInspector] public List<Enemy> allSpawnedEnemies;

    [SerializeField] private Image crystalHealthImage;
    [SerializeField] private Image playerHealthImage;

    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private TextMeshProUGUI coinText;
    [SerializeField] private TextMeshProUGUI weaponText;

    private int waveCount = 1;
    private int subWaveCount = 1;

    public Camera shopCam;
    public Camera mainCam;

    private bool stopWaveCheck;

    public GameObject shopPortal;

    [SerializeField] private int wavesUntilShop;

    [HideInInspector] public bool inShop;

    [SerializeField] private GameObject winScreen;
    [SerializeField] private GameObject loseScreen;
    [SerializeField] private GameObject pauseScreen;

    public bool paused;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        InitializeWave();
        UpdateWeaponText();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }

        coinText.text = player.CurrentCoins.ToString();
        if (Input.GetMouseButtonDown(1))
        {
            int currentIndex = 0;
            GameObject activeWeapon = null;
            List<GameObject> allBoughtWeapons = allWeapons.Where(r => r.GetComponent<Weapon>().unlocked).ToList();
            foreach (var r in allBoughtWeapons)
            {
                if (r.activeSelf)
                {
                    r.SetActive(false);
                    activeWeapon = r;
                    activeWeapon.GetComponent<Weapon>().reloading = false;
                }
            }
            if (allBoughtWeapons != null)
            {
                currentIndex = allBoughtWeapons.IndexOf(activeWeapon);
                if (currentIndex == allBoughtWeapons.Count - 1)
                {
                    allBoughtWeapons[0].SetActive(true);
                    player.currentSelectedWeapon = allBoughtWeapons[0].GetComponent<Weapon>();
                }
                else
                {
                    allBoughtWeapons[currentIndex + 1].SetActive(true);
                    player.currentSelectedWeapon = allBoughtWeapons[currentIndex + 1].GetComponent<Weapon>();
                }

            }
            UpdateWeaponText();
            if (player.reloadIndicator.gameObject.activeSelf)
            {
                ToggleReloadIndicator();
            }
        }

        if (!stopWaveCheck)
        {
            CheckWaveCondition();
        }

        if (!allWaves[currentWave].waveCompleted)
        {
            if(allWaves[currentWave]._boostTime > 0)
            {
                allWaves[currentWave]._boostTime -= Time.deltaTime;
                if(allWaves[currentWave]._boostTime <= 0)
                {
                    DoWaveBoost();
                }
            }
        }
    }

    private void CheckWaveCondition()
    {
        if(allSpawnedEnemies.Count <= 0)
        {
            if(GetRemainingEnemyAmount() <= 0)
            {
                if((currentWave + 1) % wavesUntilShop == 0)
                {
                    if(currentWave >= allWaves.Count - 1)
                    {
                        WinGame();
                    }
                    else
                    {
                        StopWaves();
                        shopPortal.SetActive(true);
                        SoundManager.instance.PlayPortalSound();
                    }
                }
                else
                {
                    IncreaseWave();
                }
            }
        }
    }

    private void StopWaves()
    {
        stopWaveCheck = true;
    }

    public void ContinueWaves()
    {
        stopWaveCheck = false;
    }

    public void LoseGame()
    {
        loseScreen.SetActive(true);
        paused = true;
        Time.timeScale = 0;
    }

    public void WinGame()
    {
        winScreen.SetActive(true);
        paused = true;
        Time.timeScale = 0;
    }

    public void KillPlayer()
    {
        LoseGame();
    }

    public IEnumerator DeleteParticleDelayed(GameObject _targetObject, float _time)
    {
        yield return new WaitForSecondsRealtime(_time);
        Destroy(_targetObject.gameObject);
    }

    private void InitializeWave()
    {
        //Calculate boost time
        allWaves[currentWave]._boostTime = Random.Range(allWaves[currentWave].minBoostTime, allWaves[currentWave].maxBoostTime);

        //Boost spawner
        foreach (var r in allWaves[currentWave].boostedSpawns)
        {
            r.maxSpawnDelay /= allWaves[currentWave].boostMultiplier;
            r.minSpawnDelay /= allWaves[currentWave].boostMultiplier;
            r.boosted = true;
        }

        foreach(var r in allEnemySpawns)
        {
            r.CalculateSpawnTime();
        }
    }

    private void DoWaveBoost()
    {
        //Calculate boost time
        allWaves[currentWave]._boostTime = Random.Range(allWaves[currentWave].minBoostTime, allWaves[currentWave].maxBoostTime);

        //Calculate the amount of spawner boosted
        int boostedSpawnAmount = allWaves[currentWave].boostedSpawns.Count;

        //Reset boost of the boosted spawns
        List<EnemySpawn> boostedSpawns = allEnemySpawns.Where(r => r.boosted).ToList();
        foreach(var r in boostedSpawns)
        {
            r.maxSpawnDelay *= allWaves[currentWave].boostMultiplier;
            r.minSpawnDelay *= allWaves[currentWave].boostMultiplier;
            r.boosted = false;
        }

        //Apply new boost to spawns
        List<EnemySpawn> possibleBoostedSpawns = allEnemySpawns.Where(r => !r.boosted).ToList();
        if(boostedSpawnAmount > 0)
        {
            for(int i = 0; i < boostedSpawnAmount; i++)
            {
                int r = Random.Range(0, possibleBoostedSpawns.Count);
                possibleBoostedSpawns[r].boosted = true;
                possibleBoostedSpawns[r].maxSpawnDelay /= allWaves[currentWave].boostMultiplier;
                possibleBoostedSpawns[r].minSpawnDelay /= allWaves[currentWave].boostMultiplier;
                possibleBoostedSpawns.RemoveAt(r);
            }
        }
    }

    public void ReduceEnemySpawnAmount()
    {
        allWaves[currentWave].totalEnemySpawnAmount--;
    }

    public int GetRemainingEnemyAmount()
    {
        return allWaves[currentWave].totalEnemySpawnAmount;
    }

    public void IncreaseWave()
    {
        if(currentWave < allWaves.Count - 1)
        {
            foreach(var r in allWaves[currentWave].boostedSpawns)
            {
                r.minSpawnDelay *= allWaves[currentWave].boostMultiplier;
                r.maxSpawnDelay *= allWaves[currentWave].boostMultiplier;
                r.boosted = false;
            }
            allWaves[currentWave].waveCompleted = true;
            currentWave++;
            InitializeWave();
        }
        else
        {
            allWaves[currentWave].waveCompleted = true;
            WinGame();
        }
        subWaveCount++;
        if(subWaveCount == wavesUntilShop + 1)
        {
            subWaveCount = 1;
            waveCount++;
        }
        waveText.text = $"Wave  {waveCount}-{subWaveCount}";
    }

    public void ChangeToShopCam()
    {
        shopCam.gameObject.SetActive(true);
        mainCam.gameObject.SetActive(false);
    }

    public void ChangeToMainCam()
    {
        shopCam.gameObject.SetActive(false);
        mainCam.gameObject.SetActive(true);
    }

    public GameObject GetSpawnObject()
    {
        bool picked = false;
        GameObject go = null;
        foreach(var r in allWaves[currentWave].enemiesOnThisWave)
        {
            float random = Random.Range(0f, 1f) * 100;
            if(random < r.probability && !picked)
            {
                picked = true;
                go = r.enemyObj;
            }
        }
        if(go == null)
        {
            return allWaves[currentWave].enemiesOnThisWave[0].enemyObj;
        }
        else
        {
            return go;
        }
    }

    public void UpdateHealthBars()
    {
        float crystalFillAmount = (float)crystal.health / (float)crystal.maxhealth;
        float playerFillAmount = (float)player.health / (float)player.maxHealth;

        crystalHealthImage.fillAmount = crystalFillAmount;
        playerHealthImage.fillAmount = playerFillAmount;
    }

    public void UpdateWeaponText()
    {
        Weapon weapon = player.GetComponentInChildren<Weapon>();
        if (weapon.useAmmo)
        {
            weaponText.text = $"{weapon.actualAmmo}/{weapon.ammoCapacity} - {weapon.currentAmmo}/{weapon.maxAmmo}\n {weapon.gameObject.name}";
        }
        else
        {
            weaponText.text = $"{weapon.actualAmmo}/{weapon.ammoCapacity}\n {weapon.gameObject.name}";
        }
    }

    public void ToggleReloadIndicator()
    {
        player.reloadIndicator.gameObject.SetActive(!player.reloadIndicator.gameObject.activeSelf);
    }

    public List<GameObject> GetAllBoughtWeapons()
    {
        return allWeapons.Where(r => r.GetComponent<Weapon>().unlocked).ToList();
    }

    public void SelectWeapon(GameObject targetWeapon)
    {
        List<GameObject> allBoughtWeapons = allWeapons.Where(r => r.GetComponent<Weapon>().unlocked).ToList();
        player.currentSelectedWeapon.reloading = false;
        player.reloadIndicator.SetActive(false);
        foreach (var r in allBoughtWeapons)
        {
            if (r.activeSelf)
            {
                r.SetActive(false);
            }
        }
        targetWeapon.SetActive(true);
        player.currentSelectedWeapon = targetWeapon.GetComponent<Weapon>();
    }

    private void TogglePause()
    {
        if (pauseScreen.activeSelf)
        {
            pauseScreen.SetActive(false);
            Time.timeScale = 1;
            paused = false;
        }
        else
        {
            pauseScreen.SetActive(true);
            Time.timeScale = 0;
            paused = true;
        }
    }
}

[System.Serializable]
public class Wave
{
    public int totalEnemySpawnAmount;
    public List<EnemySpawn> boostedSpawns;
    public float maxBoostTime;
    public float minBoostTime;
    [HideInInspector] public float _boostTime;
    public float boostMultiplier;

    public List<EnemyEntry> enemiesOnThisWave;

    public bool waveCompleted;
}

[System.Serializable]
public class EnemyEntry
{
    public GameObject enemyObj;
    public float probability;
}