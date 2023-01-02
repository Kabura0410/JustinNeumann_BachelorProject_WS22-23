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

    public SpriteRenderer nameRen;

    public TextMeshProUGUI priceText;

    private bool canInteract;

    [SerializeField] private SpriteRenderer glow;

    [SerializeField] private Canvas canvas;

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
        ren.sprite = _item.itemSprite;
        nameRen.sprite = _item.itemNameSprite;
        priceText.text = _item.cost.ToString();
        canInteract = true;
        glow.gameObject.SetActive(true);
    }

    private void BuyItem()
    {
        if(GameManager.instance.player.CurrentCoins >= item.cost)
        {
            GameManager.instance.player.CurrentCoins -= item.cost;
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
                        SoundManager.instance.PlayMoneySound();
                        GameManager.instance.UpdateWeaponUI();
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
                            SoundManager.instance.PlayMoneySound();
                            GameManager.instance.UpdateWeaponUI();
                        }
                        else
                        {
                            SoundManager.instance.PlayWrongSound();
                        }
                    }
                    break;
                case ShopItemType.artefact:
                    HealItemForArtefact artefactItem = item as HealItemForArtefact;
                    GameManager.instance.crystal.Heal(artefactItem.healAmount);
                    ClearSlot(this);
                    SoundManager.instance.PlayMoneySound();
                    break;
                case ShopItemType.player:
                    HealItemForPlayer playerItem = item as HealItemForPlayer;
                    GameManager.instance.player.Heal(playerItem.healAmount);
                    ClearSlot(this);
                    SoundManager.instance.PlayMoneySound();
                    break;
            }
        }
        else
        {
            SoundManager.instance.PlayWrongSound();
        }
    }

    private void ClearSlot(ShopSlot _slot)
    {
        _slot.priceText.text = "";
        _slot.ren.sprite = default;
        _slot.nameRen.sprite = default;
        _slot.item = null;
        canInteract = false;
        glow.gameObject.SetActive(false);
    }

    public void ToggleCanvas()
    {
        if (canvas.isActiveAndEnabled)
        {
            canvas.gameObject.SetActive(false);
        }
        else
        {
            canvas.gameObject.SetActive(true);
        }
    }
}

[System.Serializable]
public class ShopItem
{
    public string description;
    public int cost;
    public Sprite itemSprite;
    public Sprite itemNameSprite;
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