using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class IntroText : MonoBehaviour
{
    public static IntroText instance;

    private bool start = false;

    private bool stop;

    private bool fadeIn;
    private bool fadeOut;

    private int currentText;

    [SerializeField] private TextMeshProUGUI introText;

    [SerializeField] private List<IntroEntry> allIntroParts;

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
        if (Input.GetKeyDown(KeyCode.P))
        {
            StartIntro();
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
    }

    public void StartIntro()
    {
        start = true;
        introText.color = new Color(introText.color.r, introText.color.g, introText.color.b, 0);
        introText.text = allIntroParts[currentText].content;
        currentText++;
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
        }
        else
        {
            introText.text = allIntroParts[currentText].content;
        }
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