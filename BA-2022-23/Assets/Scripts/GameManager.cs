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
    [SerializeField] private TextMeshProUGUI weaponText;

    private int waveCount = 1;
    private int subWaveCount = 1;

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
        if (Input.GetMouseButtonDown(1))
        {
            int currentIndex = 0;
            GameObject activeWeapon = null;
            foreach (var r in allWeapons)
            {
                if (r.activeSelf)
                {
                    r.SetActive(false);
                    activeWeapon = r;
                    activeWeapon.GetComponent<Weapon>().reloading = false;
                }
            }
            if (activeWeapon != null)
            {
                currentIndex = allWeapons.IndexOf(activeWeapon);
                if (currentIndex == allWeapons.Count - 1)
                {
                    allWeapons[0].SetActive(true);
                }
                else
                {
                    allWeapons[currentIndex + 1].SetActive(true);
                }

            }
            UpdateWeaponText();
            if (player.reloadIndicator.gameObject.activeSelf)
            {
                ToggleReloadIndicator();
            }
        }

        CheckWaveCondition();

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
                IncreaseWave();
            }
        }
    }

    public void LoseGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
            //LevelCompleted
        }
        subWaveCount++;
        if(subWaveCount == 6)
        {
            subWaveCount = 1;
            waveCount++;
        }
        waveText.text = $"Wave  {waveCount}-{subWaveCount}";
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
        weaponText.text = $"{weapon.gameObject.name}\n {weapon.actualAmmo} / {weapon.ammoCapacity}";
    }

    public void ToggleReloadIndicator()
    {
        player.reloadIndicator.gameObject.SetActive(!player.reloadIndicator.gameObject.activeSelf);
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
