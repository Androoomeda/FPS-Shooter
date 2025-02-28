using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [SerializeField] private InventorySlot[] InventorySlots;
    [SerializeField] private GameObject InventoryItemPrefab;

    void Awake()
    {
        if(InventoryManager.Instance == null)
            InventoryManager.Instance = this;
        else
            Destroy(gameObject);
    }

    public bool AddItem(Item item)
    {
        for(int i = 0; i < InventorySlots.Length; i++)
        {
            InventorySlot slot = InventorySlots[i];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>(); 
            if(itemInSlot == null)
            {
                SpawnNewItem(item, slot);
                return true;
            }
        }

        return false;
    }

    void SpawnNewItem(Item item, InventorySlot slot)
    {
        GameObject newItem = Instantiate(InventoryItemPrefab, slot.transform);
        InventoryItem inventoryItem = newItem.GetComponent<InventoryItem>();
        inventoryItem.InitializeItem(item);
    }
}
