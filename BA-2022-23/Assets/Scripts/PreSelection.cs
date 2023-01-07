using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreSelection : MonoBehaviour
{
    public static PreSelection instance;

    public enum Artefact
    {
        Cube,
        Ball,
        Ramen,
        Coke
    }

    public enum Character
    {
        Herbert,
        Luis
    }

    public enum Map
    {
        Fall,
        Winter
    }

    public Artefact artefact = Artefact.Cube;

    public Character character = Character.Herbert;

    public Map map = Map.Fall;



    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }


    /// <summary>
    /// 0 = Cube; 1 = Ball; 2 = Ramen; 3 = Coke
    /// </summary>
    /// <param name="selectedItem"></param>
    public void SelectArtifact(int selectedItem)
    {
        switch (selectedItem)
        {
            case 0:
                artefact = Artefact.Cube;
                break;
            case 1:
                artefact = Artefact.Ball;
                break;
            case 2:
                artefact = Artefact.Ramen;
                break;
            case 3:
                artefact = Artefact.Coke;
                break;
        }
    }


    /// <summary>
    /// 0 = Herbert; 1 = Luis
    /// </summary>
    /// <param name="selectedCharacter"></param>
    public void SelectCharacter(int selectedCharacter)
    {
        switch (selectedCharacter)
        {
            case 0:
                character = Character.Herbert;
                break;
            case 1:
                character = Character.Luis;
                break;
        }
    }


    /// <summary>
    /// 0 = Fall; 1 = Winter
    /// </summary>
    /// <param name="selectedCharacter"></param>
    public void SelectMap(int selectedCharacter)
    {
        switch (selectedCharacter)
        {
            case 0:
                map = Map.Fall;
                break;
            case 1:
                map = Map.Winter;
                break;
        }
    }
}
