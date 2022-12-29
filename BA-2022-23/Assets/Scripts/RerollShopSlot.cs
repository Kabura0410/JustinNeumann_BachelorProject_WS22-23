using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RerollShopSlot : MonoBehaviour, IInteractable
{
    [SerializeField] private int cost;

    private bool playerInTrigger;

    private bool canInteract;

    public bool PlayerInTrigger { get => playerInTrigger; set => playerInTrigger = value; }
    public bool CanInteract { get => canInteract; set => canInteract = value; }

    void Start()
    {
        CanInteract = true;
    }

    private void Update()
    {
        if (!ShopManager.instance.CheckShopSlotState())
        {
            canInteract = false;
        }
        else
        {
            canInteract = true;
        }
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
        if(canInteract && playerInTrigger)
        {
            if(GameManager.instance.player.CurrentCoins >= cost)
            {
                ShopManager.instance.RerollShopSlots();
                GameManager.instance.player.CurrentCoins -= cost;
            }
        }
    }

    public void ShowOutline()
    {
        //Not in use
    }
}
