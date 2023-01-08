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

    public GameObject[] allArtefacts;
    public PlayerController[] allPlayer;

    public float playerDelayTime, artifactDelayTime;


    [Header("Weapon UI")]
    [SerializeField] private Image goldenRevolverSprite;
    [SerializeField] private Image weapon2Sprite;
    [SerializeField] private Image weapon3Sprite;

    [SerializeField] private Image lowOnAmmo1;
    [SerializeField] private Image lowOnAmmo2;
    [SerializeField] private Image lowOnAmmo3;

    [SerializeField] private Image frame1;
    [SerializeField] private Image frame2;
    [SerializeField] private Image frame3;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        InitializePreSelection();
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
            UpdateWeaponUI();
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

    public void LoseGame(float _time)
    {
        paused = true;
        Time.timeScale = 0;
        StartCoroutine(LoseScreenDelayed(_time));
    }

    public void WinGame()
    {
        winScreen.SetActive(true);
        paused = true;
        Time.timeScale = 0;
    }

    public void KillPlayer()
    {
        LoseGame(playerDelayTime);
    }

    private IEnumerator LoseScreenDelayed(float _time)
    {
        yield return new WaitForSecondsRealtime(_time);
        loseScreen.SetActive(true);
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

    public void TogglePause()
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

    public void UpdateWeaponUI()
    {
        List<GameObject> allBoughtWeapons = GetAllBoughtWeapons();
        int i = 0;
        if(allBoughtWeapons.Count > 1)
        {
            if(allBoughtWeapons.Count >= 3)
            {
                weapon2Sprite.gameObject.SetActive(true);
                weapon3Sprite.gameObject.SetActive(true);
                frame2.gameObject.SetActive(true);
                frame3.gameObject.SetActive(true);
            }
            else
            {
                weapon2Sprite.gameObject.SetActive(true);
                frame2.gameObject.SetActive(true);
            }
        }
        foreach(var r in allBoughtWeapons)
        {
            Weapon weapon = r.GetComponent<Weapon>();
            switch (i)
            {
                case 0:
                    goldenRevolverSprite.sprite = weapon.uiSprite;
                    if (weapon.CheckForAmmoLeft())
                    {
                        lowOnAmmo1.gameObject.SetActive(false);
                    }
                    else
                    {
                        lowOnAmmo1.gameObject.SetActive(true);
                    }
                    break;
                case 1:
                    weapon2Sprite.sprite = weapon.uiSprite;
                    if (weapon.CheckForAmmoLeft())
                    {
                        lowOnAmmo2.gameObject.SetActive(false);
                    }
                    else
                    {
                        lowOnAmmo2.gameObject.SetActive(true);
                    }
                    break;
                case 2:
                    weapon3Sprite.sprite = weapon.uiSprite;
                    if (weapon.CheckForAmmoLeft())
                    {
                        lowOnAmmo3.gameObject.SetActive(false);
                    }
                    else
                    {
                        lowOnAmmo3.gameObject.SetActive(true);
                    }
                    break;
            }
            i++;
        }
        int j = 0;
        for (int f = 0; f < allBoughtWeapons.Count; f++)
        {
            if(allBoughtWeapons[f] == player.currentSelectedWeapon.gameObject)
            {
                j = f;
            }
        }
        goldenRevolverSprite.color = new Color(goldenRevolverSprite.color.r, goldenRevolverSprite.color.g, goldenRevolverSprite.color.b, .5f);
        weapon2Sprite.color = new Color(weapon2Sprite.color.r, weapon2Sprite.color.g, weapon2Sprite.color.b, .5f);
        weapon3Sprite.color = new Color(weapon3Sprite.color.r, weapon3Sprite.color.g, weapon3Sprite.color.b, .5f);
        frame1.color = new Color(frame1.color.r, frame1.color.g, frame1.color.b, .5f);
        frame2.color = new Color(frame2.color.r, frame2.color.g, frame2.color.b, .5f);
        frame3.color = new Color(frame3.color.r, frame3.color.g, frame3.color.b, .5f);
        switch (j)
        {
            case 0:
                goldenRevolverSprite.color = new Color(goldenRevolverSprite.color.r, goldenRevolverSprite.color.g, goldenRevolverSprite.color.b, 1);
                frame1.color = new Color(frame1.color.r, frame1.color.g, frame1.color.b, 1);
                break;
            case 1:
                weapon2Sprite.color = new Color(weapon2Sprite.color.r, weapon2Sprite.color.g, weapon2Sprite.color.b, 1);
                frame2.color = new Color(frame2.color.r, frame2.color.g, frame2.color.b, 1);
                break;
            case 2:
                weapon3Sprite.color = new Color(weapon3Sprite.color.r, weapon3Sprite.color.g, weapon3Sprite.color.b, 1);
                frame3.color = new Color(frame3.color.r, frame3.color.g, frame3.color.b, 1);
                break;
        }
    }

    private void InitializePreSelection()
    {
        if(PreSelection.instance != null)
        {

            switch (PreSelection.instance.character)
            {
                case PreSelection.Character.Herbert:
                    player = allPlayer.Where(r => r.character == PlayerController.CharacterType.Herbert).First();
                    break;
                case PreSelection.Character.Luis:
                    player = allPlayer.Where(r => r.character == PlayerController.CharacterType.Luis).First();
                    break;
            }
            player.gameObject.SetActive(true);


            switch (PreSelection.instance.artefact)
            {
                case PreSelection.Artefact.Cube:
                    allArtefacts[0].SetActive(true);
                    player.maxHealth = Mathf.RoundToInt(player.maxHealth * 1.5f);
                    player.health = player.maxHealth;
                    break;
                case PreSelection.Artefact.Ball:
                    allArtefacts[1].SetActive(true);
                    player.CurrentCoins += 25;
                    break;
                case PreSelection.Artefact.Ramen:
                    allArtefacts[2].SetActive(true);
                    player.extraJumpsValue++;
                    break;
                case PreSelection.Artefact.Coke:
                    allArtefacts[3].SetActive(true);
                    ShopManager.instance.cokeVending.shouldPay = false;
                    ShopManager.instance.cokeVending.ApplyArtefactBoost();
                    break;
            }
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