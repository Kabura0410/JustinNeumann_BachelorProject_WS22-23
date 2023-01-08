using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void Play ()
    {
        Time.timeScale = 1;
        if(PreSelection.instance != null)
        {
            switch (PreSelection.instance.map)
            {
                case PreSelection.Map.Fall:
                    SceneManager.LoadScene(1);
                    break;
                case PreSelection.Map.Winter:
                    SceneManager.LoadScene(2);
                    break;
            }
        }
        else
        {
            SceneManager.LoadScene(1);
        }
    }

    public void BackToMenu()
    {
        Time.timeScale = 1;
        if(PreSelection.instance != null)
        {
            Destroy(PreSelection.instance.gameObject);
        }
        if(SoundManager.instance != null)
        {
            Destroy(SoundManager.instance.gameObject);
        }
        SceneManager.LoadScene(0);
    }

    public void Quit ()
    {
        Application.Quit();
    }

}
