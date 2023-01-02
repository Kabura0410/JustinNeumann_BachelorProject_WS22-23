using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public AudioMixer audioMixer;

    public static SoundManager instance;

    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    [SerializeField] private AudioSource jumpChickenSound;
    [SerializeField] private AudioSource hitChickenSound;
    [SerializeField] private AudioSource hitEnemySound;
    [SerializeField] private AudioSource coinSound;
    [SerializeField] private AudioSource moneySound;
    [SerializeField] private AudioSource lowMoneySound;
    [SerializeField] private AudioSource wrongSound;
    [SerializeField] private AudioSource portalSound;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        if (PlayerPrefs.HasKey("MusicSliderValue"))
        {
            float temp = PlayerPrefs.GetFloat("MusicSliderValue");
            musicSlider.value = temp;
            if (temp <= musicSlider.minValue)
            {
                audioMixer.SetFloat("Music", -80f);
            }
            else
            {
                audioMixer.SetFloat("Music", Mathf.Log10(musicSlider.value) * 20);
            }
        }

        if (PlayerPrefs.HasKey("SFXSliderValue"))
        {
            float temp = PlayerPrefs.GetFloat("SFXSliderValue");
            sfxSlider.value = temp;
            if (temp <= sfxSlider.minValue)
            {
                audioMixer.SetFloat("SFX", -80f);
            }
            else
            {
                audioMixer.SetFloat("SFX", Mathf.Log10(sfxSlider.value) * 20);
            }
        }

        if (PlayerPrefs.HasKey("MasterSliderValue"))
        {
            float temp = PlayerPrefs.GetFloat("MasterSliderValue");
            masterSlider.value = temp;
            if (temp <= masterSlider.minValue)
            {
                audioMixer.SetFloat("Master", -80f);
            }
            else
            {
                audioMixer.SetFloat("Master", Mathf.Log10(masterSlider.value) * 20);
            }
        }
    }

    void Update()
    {
        
    }

    public void ChangeMusicVolume(float _sliderValue)
    {
        if (_sliderValue <= musicSlider.minValue)
        {
            audioMixer.SetFloat("Music", -80f);
        }
        else
        {
            audioMixer.SetFloat("Music", Mathf.Log10(_sliderValue) * 20);
        }
        PlayerPrefs.SetFloat("MusicSliderValue", musicSlider.value);
    }

    public void ChangeSFXVolume(float _sliderValue)
    {
        if (_sliderValue <= sfxSlider.minValue)
        {
            audioMixer.SetFloat("SFX", -80f);
        }
        else
        {
            audioMixer.SetFloat("SFX", Mathf.Log10(_sliderValue) * 20);
        }
        PlayerPrefs.SetFloat("SFXSliderValue", sfxSlider.value);
    }

    public void ChangeMasterVolume(float _sliderValue)
    {
        if (_sliderValue <= masterSlider.minValue)
        {
            audioMixer.SetFloat("Master", -80f);
        }
        else
        {
            audioMixer.SetFloat("Master", Mathf.Log10(_sliderValue) * 20);
        }
        PlayerPrefs.SetFloat("MasterSliderValue", masterSlider.value);
    }

    public void PlayHitChickenSound()
    {
        hitChickenSound.Play();
    }

    public void PlayHitEnemySound()
    {
        hitEnemySound.Play();
    }

    public void PlayCoinSound()
    {
        coinSound.Play();
    }

    public void PlayJumpChickenSound()
    {
        jumpChickenSound.Play();
    }

    public void PlayMoneySound()
    {
        moneySound.Play();
    }

    public void PlayLowMoneySound()
    {
        lowMoneySound.Play();
    }

    public void PlayWrongSound()
    {
        wrongSound.Play();
    }

    public void PlayPortalSound()
    {
        portalSound.Play();
    }
}
