using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IntroText : MonoBehaviour
{
    public static IntroText instance;

    private bool start = true;

    private bool stop;

    private bool fadeIn;
    private bool fadeOut;

    private int currentText;

    [SerializeField] private TextMeshProUGUI introText;

    [SerializeField] private List<IntroEntry> allIntroParts;

    [SerializeField] private GameObject mainPanel;

    [SerializeField] private GameObject skipButton;

    [SerializeField] private Animator anim;

    [SerializeField] private Image[] allMenuImages;
    [SerializeField] private float menuFadeSpeed;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        StartIntro();
    }

    void FixedUpdate()
    {
        if (Input.anyKey && !skipButton.activeSelf)
        {
            skipButton.SetActive(true);
        }

        if(start && fadeIn && !stop)
        {
            introText.color = new Color(introText.color.r, introText.color.g, introText.color.b, introText.color.a + allIntroParts[currentText].speed * Time.fixedDeltaTime);
            if (introText.color.a >= 1)
            {
                StartCoroutine(FadeOutDelayed(allIntroParts[currentText].showDuration));
                fadeIn = false;
            }
        }
        else if(start && fadeOut && !stop)
        {
            introText.color = new Color(introText.color.r, introText.color.g, introText.color.b, introText.color.a - allIntroParts[currentText].speed * Time.fixedDeltaTime);
            if(introText.color.a <= 0)
            {
                StartCoroutine(SelectNextTextDelayed(allIntroParts[currentText].delay));
                fadeOut = false;
            }
        }

        if(stop && allMenuImages[0].color.a <= 1)
        {
            foreach(var r in allMenuImages)
            {
                r.color = new Color(r.color.r, r.color.g, r.color.b, r.color.a + Time.fixedDeltaTime * menuFadeSpeed);
            }
        }
    }

    public void StartIntro()
    {
        start = true;
        introText.color = new Color(introText.color.r, introText.color.g, introText.color.b, 0);
        introText.text = allIntroParts[currentText].content;
        fadeIn = true;
    }

    private void SelectNextText()
    {
        fadeIn = true;
        currentText++;
        if(currentText >= allIntroParts.Count)
        {
            introText.text = allIntroParts[allIntroParts.Count - 1].content;
            stop = true;
            mainPanel.SetActive(true);
            skipButton.SetActive(false);
            anim.SetTrigger("skip");
        }
        else
        {
            introText.text = allIntroParts[currentText].content;
        }
    }

    public void SkipIntro()
    {
        mainPanel.SetActive(true);
        stop = true;
        fadeIn = false;
        fadeOut = false;
        start = false;
        introText.text = "";
        skipButton.SetActive(false);
        anim.SetTrigger("skip");
    }

    private IEnumerator SelectNextTextDelayed(float _time)
    {
        yield return new WaitForSecondsRealtime(_time);
        SelectNextText();
    }

    private IEnumerator FadeOutDelayed(float _time)
    {
        yield return new WaitForSecondsRealtime(_time);
        fadeOut = true;
    }
}

[System.Serializable]
public class IntroEntry
{
    public string content;
    public float speed;
    public float showDuration;
    public float delay;
}