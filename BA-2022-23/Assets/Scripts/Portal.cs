using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform portPosition;
    [SerializeField] private Transform portalPosition;

    [SerializeField] private float speed;
    [SerializeField] private float portDelay;

    private bool movePlayer;

    public enum CameraType
    {
        playerCam, shopCam
    }

    [SerializeField] private CameraType cameraType;

    public bool PlayerInTrigger { get => playerInTrigger; set => playerInTrigger = value; }
    public bool CanInteract { get => true; set => CanInteract = true; }

    private bool playerInTrigger;

    private void Update()
    {
        if (movePlayer)
        {
            GameManager.instance.player.transform.position += (portalPosition.position - GameManager.instance.player.transform.position).normalized * Time.fixedDeltaTime * speed;
        }
    }

    public void TeleportPlayer()
    {
        StartCoroutine(TeleportPlayerDelayed());
    }

    private IEnumerator TeleportPlayerDelayed()
    {
        GameManager.instance.player.ToggleMovement();
        GameManager.instance.player.rb.gravityScale = 0;
        GameManager.instance.player.rb.velocity = Vector2.zero;
        movePlayer = true;
        yield return new WaitForSecondsRealtime(portDelay);
        switch (cameraType)
        {
            case CameraType.playerCam:
                GameManager.instance.ChangeToMainCam();
                GameManager.instance.ContinueWaves();
                GameManager.instance.shopPortal.SetActive(false);
                GameManager.instance.IncreaseWave();
                GameManager.instance.inShop = false;
                ShopManager.instance.DeleteAllObsoleteObjects();
                break;
            case CameraType.shopCam:
                GameManager.instance.ChangeToShopCam();
                GameManager.instance.inShop = true;
                ShopManager.instance.InstantiateShop();
                break;
        }
        GameManager.instance.player.transform.position = portPosition.position;
        GameManager.instance.player.ToggleMovement();
        GameManager.instance.player.rb.gravityScale = GameManager.instance.player.StartGravity;
        movePlayer = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerInTrigger = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerInTrigger = false;
        }
    }

    public void Interact()
    {
        if (PlayerInTrigger)
        {
            TeleportPlayer();
        }
    }

    public void ShowOutline()
    {
        //Not used for portal
    }
}
