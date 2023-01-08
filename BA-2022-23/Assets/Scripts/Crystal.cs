using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crystal : MonoBehaviour
{
    public int maxhealth;
    public int health;
    public GameObject explosionEffect;

    [SerializeField] private GameObject artifactDamageEffect;

    public MeshRenderer[] allArtifacts;

    public Material startMaterial, dmgMaterial;
    private Renderer ren;

    [SerializeField] private float dmgTime;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void GetDamage(int _amount)
    {
        health -= _amount;
        GameManager.instance.UpdateHealthBars();
        GameObject newParticle = Instantiate(artifactDamageEffect, transform.position, Quaternion.identity);
        GameManager.instance.StartCoroutine(GameManager.instance.DeleteParticleDelayed(newParticle, 4));


        switch (PreSelection.instance.artefact)
        {
            case PreSelection.Artefact.Cube:
                if(startMaterial == null)
                {
                    startMaterial = allArtifacts[0].material;
                }
                allArtifacts[0].material = dmgMaterial;
                ren = allArtifacts[0];
                StartCoroutine(DelayMaterialApply());
                break;
            case PreSelection.Artefact.Ball:
                if (startMaterial == null)
                {
                    startMaterial = allArtifacts[1].material;
                }
                allArtifacts[1].material = dmgMaterial;
                ren = allArtifacts[1];
                StartCoroutine(DelayMaterialApply());
                break;
            case PreSelection.Artefact.Ramen:
                if (startMaterial == null)
                {
                    startMaterial = allArtifacts[2].material;
                }
                allArtifacts[2].material = dmgMaterial;
                ren = allArtifacts[2];
                StartCoroutine(DelayMaterialApply());
                break;
            case PreSelection.Artefact.Coke:
                if (startMaterial == null)
                {
                    startMaterial = allArtifacts[3].material;
                }
                allArtifacts[3].material = dmgMaterial;
                ren = allArtifacts[3];
                StartCoroutine(DelayMaterialApply());
                break;
        }

        if (health <= 0)
        {
            GameObject go = Instantiate(explosionEffect, transform.position, Quaternion.identity);
            GameManager.instance.StartCoroutine(GameManager.instance.DeleteParticleDelayed(go, 10));
       
            GameManager.instance.LoseGame();
        }
    }

    public void Heal(int _amount)
    {
        if(health + _amount > maxhealth)
        {
            health = maxhealth;
        }
        else
        {
            health += _amount;
        }
        GameManager.instance.UpdateHealthBars();
    }

    private IEnumerator DelayMaterialApply()
    {
        yield return new WaitForSeconds(dmgTime);
        ren.material = startMaterial;
    }
}
