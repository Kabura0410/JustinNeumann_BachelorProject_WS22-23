using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public PlayerController player;

    [SerializeField] private List<GameObject> allWeapons;

    public Crystal crystal;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        
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
}
