using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonGroup : MonoBehaviour
{
    public List<CustomButton> allCustomButtons;


    void Start()
    {
        foreach(var r in allCustomButtons)
        {
            r.group = this;
        }    
    }

    public void SelectItem(CustomButton targetButton)
    {
        foreach(var r in allCustomButtons)
        {
            r.DeselectButton();
        }
        targetButton.SelectButton();
    }

}
