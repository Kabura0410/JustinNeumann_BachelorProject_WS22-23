using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public static ShopManager instance;

    public List<WeaponItem> allWeaponItemsToBuy;
    [SerializeField] private List<HealItemForPlayer> allPlayerHealItemsToBuy;
    [SerializeField] private List<HealItemForArtefact> allArtefactHealItemToBuy;
    [SerializeField] private ShopItem blankItem;

    [SerializeField] private ShopSlot weaponSlot;
    [SerializeField] private ShopSlot playerHealSlot;
    [SerializeField] private ShopSlot artefactHealSlot;

    public List<GameObject> objectsToDespawn;


    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    public void InstantiateShop()
    {
        List<WeaponItem> allAvailableWeapons = allWeaponItemsToBuy.Where(r => !r.bought).ToList();
        int r = Random.Range(0, allAvailableWeapons.Count);
        weaponSlot.SetShopItem(allAvailableWeapons[r]);
     
        int s = Random.Range(0, allPlayerHealItemsToBuy.Count);
        playerHealSlot.SetShopItem(allPlayerHealItemsToBuy[s]);

        int t = Random.Range(0, allArtefactHealItemToBuy.Count);
        artefactHealSlot.SetShopItem(allArtefactHealItemToBuy[t]);
    }

    public void DeleteAllObsoleteObjects()
    {
        for(int i = 0; i < objectsToDespawn.Count; i++)
        {
            GameObject go = objectsToDespawn[i];
            Destroy(go);
        }
        objectsToDespawn.Clear();
    }

    public void RerollShopSlots()
    {
        if(weaponSlot.item != null)
        {
            List<WeaponItem> allAvailableWeapons = allWeaponItemsToBuy.Where(r => !r.bought).ToList();
            int r = Random.Range(0, allAvailableWeapons.Count);
            weaponSlot.SetShopItem(allAvailableWeapons[r]);
        }

        if (weaponSlot.item != null)
        {
            int s = Random.Range(0, allPlayerHealItemsToBuy.Count);
            playerHealSlot.SetShopItem(allPlayerHealItemsToBuy[s]);
        }

        if (weaponSlot.item != null)
        {
            int t = Random.Range(0, allArtefactHealItemToBuy.Count);
            artefactHealSlot.SetShopItem(allArtefactHealItemToBuy[t]);
        }
    }

    public bool CheckShopSlotState()
    {
        bool canReroll = false;
        if(weaponSlot.item != null)
        {
            canReroll = true;
        }
        if (artefactHealSlot.item != null)
        {
            canReroll = true;
        }
        if (playerHealSlot.item != null)
        {
            canReroll = true;
        }

        return canReroll;
    }
}