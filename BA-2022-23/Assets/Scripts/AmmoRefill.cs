using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoRefill : MonoBehaviour, IInteractable
{
    private bool playerInTrigger;

    private bool canInteract;

    public bool PlayerInTrigger { get => playerInTrigger; set => playerInTrigger = value; }
    public bool CanInteract { get => canInteract; set => canInteract = value; }

    void Start()
    {
        Physics2D.IgnoreLayerCollision(15,15);
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
        if (canInteract && playerInTrigger)
        {
            GameManager.instance.player.currentSelectedWeapon.RefillWeapon();
            GameManager.instance.UpdateWeaponText();
            ShopManager.instance.objectsToDespawn.Remove(this.gameObject);
            Destroy(this.gameObject);
        }
    }

    public void ShowOutline()
    {
        //Not in use
    }
}
