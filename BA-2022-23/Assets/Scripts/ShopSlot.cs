using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ShopSlot : MonoBehaviour, IInteractable
{
    private bool playerInTrigger;

    public ShopItem item;

    public SpriteRenderer ren;

    public TextMeshProUGUI descriptionText;

    private bool canInteract;

    [SerializeField] private SpriteRenderer glow;

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
        glow.gameObject.SetActive(true);
    }

    private void BuyItem()
    {
        switch (item.type)
        {
            case ShopItemType.weapon:
                List<GameObject> allBoughtWeapons = GameManager.instance.GetAllBoughtWeapons();
                if(allBoughtWeapons.Count < 3)
                {
                    WeaponItem weaponItem = item as WeaponItem;
                    weaponItem.objectToUnlock.gameObject.GetComponent<Weapon>().UnlockWeapon();
                    weaponItem.bought = true;
                    GameManager.instance.SelectWeapon(weaponItem.objectToUnlock);
                    GameManager.instance.UpdateWeaponText();
                    ClearSlot(this);
                }
                else
                {
                    if (!GameManager.instance.player.currentSelectedWeapon.isStartWeapon)
                    {
                        List<WeaponItem> temp = ShopManager.instance.allWeaponItemsToBuy.Where(r => r.objectToUnlock.GetComponent<Weapon>() == GameManager.instance.player.currentSelectedWeapon).ToList();
                        GameManager.instance.player.currentSelectedWeapon.LockWeapon();
                        foreach(var r in temp)
                        {
                            r.bought = false;
                        }
                        WeaponItem weaponItem = item as WeaponItem;
                        weaponItem.objectToUnlock.gameObject.GetComponent<Weapon>().UnlockWeapon();
                        weaponItem.bought = true;
                        GameManager.instance.SelectWeapon(weaponItem.objectToUnlock);
                        GameManager.instance.UpdateWeaponText();
                        ClearSlot(this);
                    }
                    else
                    {
                        //Indicator for player that weapon cant be bought 
                    }
                }
                break;
            case ShopItemType.artefact:
                HealItemForArtefact artefactItem = item as HealItemForArtefact;
                GameManager.instance.crystal.Heal(artefactItem.healAmount);
                ClearSlot(this);
                break;
            case ShopItemType.player:
                HealItemForPlayer playerItem = item as HealItemForPlayer;
                GameManager.instance.player.Heal(playerItem.healAmount);
                ClearSlot(this);
                break;
        }
    }

    private void ClearSlot(ShopSlot _slot)
    {
        _slot.descriptionText.text = "";
        _slot.ren.sprite = default;
        _slot.item = null;
        canInteract = false;
        glow.gameObject.SetActive(false);
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