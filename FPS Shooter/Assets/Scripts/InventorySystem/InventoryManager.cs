using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public int AmmoAmount {get; private set;}

    [SerializeField] private InventorySlot[] InventorySlots;
    [SerializeField] private GameObject InventoryItemPrefab;
    [SerializeField] private CanvasGroup InventoryGroup;
    [SerializeField] private GameObject Player;

    private PlayerInputHandler playerInput;
    private bool isInventoryOpen;
    private Item newItem;
    private Dictionary<InventorySlot, InventoryItem> ItemsInSlots;

    void Awake()
    {
        if(InventoryManager.Instance == null)
            InventoryManager.Instance = this;
        else
            Destroy(gameObject);

        playerInput = Player.GetComponent<PlayerInputHandler>();

        ItemsInSlots = InventorySlots
            .ToDictionary(slot => slot, key => (InventoryItem)null);

        isInventoryOpen = false;
        ToogleInventoryPanel();
    }

    void Update()
    {
        if(playerInput.GetInventoryButtonDown())
        {
            isInventoryOpen = !isInventoryOpen;
            ToogleInventoryPanel();
        }
    }

    private void ToogleInventoryPanel()
    {
        InventoryGroup.alpha = isInventoryOpen ? 1 : 0;
        Cursor.lockState = isInventoryOpen ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isInventoryOpen;
    }

    public bool AddItem(Item item, int count)
    {
        newItem = item;

        foreach(var slot in ItemsInSlots.Keys)
        {
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>(); 
            if(itemInSlot != null && itemInSlot.Item == newItem && 
            itemInSlot.Count < itemInSlot.Item.MaxCount)
            {
                itemInSlot.Count += count;
                itemInSlot.UpdateCount();

                SaveAmmoAmount(count);

                return true;
            }
        }

        
        foreach(var slot in ItemsInSlots.Keys)
        {
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>(); 
            if(itemInSlot == null)
            {
                SpawnNewItem(count, slot);
                SaveAmmoAmount(count);
                return true;
            }
        }

        return false;
    }

    public void TakeAmmo(int ammoToTake)
    {
        foreach(var item in ItemsInSlots.Values)
        {
            if(item.Item.Type == ItemType.Ammo)
            {
                if(item.Count < ammoToTake)
                {
                    ammoToTake -= item.Count;
                    Destroy(item);
                }

                if(ammoToTake > 0)
                    break;
            }
        }
        AmmoAmount -= ammoToTake;
    }

    private void SpawnNewItem(int count, InventorySlot slot)
    {
        GameObject newInventoryItem = Instantiate(InventoryItemPrefab, slot.transform);
        InventoryItem inventoryItem = newInventoryItem.GetComponent<InventoryItem>();
        ItemsInSlots[slot] = inventoryItem;
        inventoryItem.InitializeItem(newItem, count, Player);
    }

    private void SaveAmmoAmount(int count)
    {
        if(newItem.Type == ItemType.Ammo)
            AmmoAmount += count;
    }
}
