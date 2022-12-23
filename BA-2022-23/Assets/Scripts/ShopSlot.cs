using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShopSlot : MonoBehaviour, IInteractable
{
    private bool playerInTrigger;

    private ShopItem item;

    public SpriteRenderer ren;

    public TextMeshProUGUI descriptionText;

    private bool canInteract;

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

    public bool PlayerInTrigger { get => playerInTrigger; set => playerInTrigger = value; }
    public bool CanInteract { get => canInteract; set => canInteract = value; }

    public void Interact()
    {
        if (PlayerInTrigger && item != null)
        {
            BuyItem();
            ClearSlot(this);
        }
    }

    public void ShowOutline()
    {
    }

    public void SetShopItem(ShopItem _item)
    {
        item = _item;
        ren.sprite = _item.sprite;
        descriptionText.text = _item.description;
        canInteract = true;
    }

    private void BuyItem()
    {
        switch (item.type)
        {
            case ShopItemType.weapon:
                WeaponItem weaponItem = item as WeaponItem;
                weaponItem.objectToUnlock.gameObject.GetComponent<Weapon>().UnlockWeapon();
                break;
            case ShopItemType.artefact:
                HealItemForArtefact artefactItem = item as HealItemForArtefact;
                GameManager.instance.crystal.Heal(artefactItem.healAmount);
                break;
            case ShopItemType.player:
                HealItemForPlayer playerItem = item as HealItemForPlayer;
                GameManager.instance.player.Heal(playerItem.healAmount);
                break;
        }
    }

    private void ClearSlot(ShopSlot _slot)
    {
        _slot.descriptionText.text = "";
        _slot.ren.sprite = default;
        _slot.item = null;
        canInteract = false;
    }

}

[System.Serializable]
public class ShopItem
{
    public string description;
    public int cost;
    public Sprite sprite;
    public ShopItemType type;
}

[System.Serializable]
public class HealItemForPlayer : ShopItem
{
    public int healAmount;
}

[System.Serializable]
public class HealItemForArtefact : ShopItem
{
    public int healAmount;
}

[System.Serializable]
public class WeaponItem : ShopItem
{
    public GameObject objectToUnlock;
    public bool bought;
}

public enum ShopItemType
{
    weapon,
    artefact,
    player
}