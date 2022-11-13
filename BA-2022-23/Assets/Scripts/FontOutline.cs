using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FontOutline : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI baseText;
    [SerializeField] private TextMeshProUGUI outlineText;

    // Update is called once per frame
    void Update()
    {
        outlineText.text = baseText.text;
    }
}
