using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VendingMachine : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject spawnObject;

    [SerializeField] private Transform directionPos;
    [SerializeField] private Transform spawnPos;

    [SerializeField] private float intensity;

    [SerializeField] private int cost;

    private bool playerInTrigger;

    private bool canInteract;

    public bool PlayerInTrigger { get => playerInTrigger; set => playerInTrigger = value; }
    public bool CanInteract { get => canInteract; set => canInteract = value; }

    [SerializeField] private GameObject canvas;

    void Start()
    {
        CanInteract = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerInTrigger = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerInTrigger = false;
        }
    }

    public void Interact()
    {
        if (canInteract && playerInTrigger && GameManager.instance.player.CurrentCoins >= cost)
        {
            SpawnContent();
            GameManager.instance.player.CurrentCoins -= cost;
            SoundManager.instance.PlayLowMoneySound();
        }
        else
        {
            SoundManager.instance.PlayWrongSound();
        }
    }

    public void ShowOutline()
    {
        //Not in use
    }

    private void SpawnContent()
    {
        GameObject go = Instantiate(spawnObject, spawnPos.position, Quaternion.identity);
        go.GetComponent<Rigidbody2D>().AddForce((directionPos.position - transform.position).normalized * intensity);
        ShopManager.instance.objectsToDespawn.Add(go);
    }

    public void ToggleCanvas()
    {
        canvas.SetActive(!canvas.activeSelf);
    }
}
