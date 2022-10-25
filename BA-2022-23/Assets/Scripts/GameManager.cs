using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;


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
        
    }

    public IEnumerator DeleteParticleDelayed(GameObject _targetObject, float _time)
    {
        yield return new WaitForSecondsRealtime(_time);
        Destroy(_targetObject.gameObject);
    }
}
