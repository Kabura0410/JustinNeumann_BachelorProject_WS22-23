using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private Animator anim;

    public static CameraShake instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        anim = GetComponent<Animator>();
    }

    public void DoCameraShake()
    {
        anim.SetTrigger("Shake");
    }
}
