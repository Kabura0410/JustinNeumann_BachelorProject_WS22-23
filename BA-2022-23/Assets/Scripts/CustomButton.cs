using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomButton : MonoBehaviour
{
    [SerializeField] private Sprite normalSprite, selectedSprite;

    [SerializeField] private Button button;

    [SerializeField] private bool isDefault;

    [HideInInspector] public ButtonGroup group;

    private void Start()
    {
        if (isDefault)
        {
            SelectButton();
        }
    }

    public void SelectItem()
    {
        group.SelectItem(this);
    }


    public void SelectButton()
    {
        button.image.sprite = selectedSprite;
    }

    public void DeselectButton()
    {
        button.image.sprite = normalSprite;
    }

}