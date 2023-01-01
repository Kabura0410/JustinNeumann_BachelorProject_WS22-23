using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FontOutline : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI baseText;
    [SerializeField] private TextMeshProUGUI outlineText;

    [SerializeField] private bool applyAlpha;

    // Update is called once per frame
    void Update()
    {
        outlineText.text = baseText.text;
        if (applyAlpha)
        {
            outlineText.color = new Color(outlineText.color.r, outlineText.color.g, outlineText.color.b, baseText.color.a);
        }
    }
}
