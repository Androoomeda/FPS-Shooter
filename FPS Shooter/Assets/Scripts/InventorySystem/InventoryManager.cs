using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [SerializeField] private InventorySlot[] InventorySlots;
    [SerializeField] private GameObject InventoryItemPrefab;
    [SerializeField] private CanvasGroup InventoryGroup;
    [SerializeField] private GameObject Player;

    private PlayerInputHandler playerInput;
    private bool isInventoryOpen;

    void Awake()
    {
        if(InventoryManager.Instance == null)
            InventoryManager.Instance = this;
        else
            Destroy(gameObject);

        playerInput = Player.GetComponent<PlayerInputHandler>();
        
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

    private void SpawnNewItem(Item item, InventorySlot slot)
    {
        GameObject newItem = Instantiate(InventoryItemPrefab, slot.transform);
        InventoryItem inventoryItem = newItem.GetComponent<InventoryItem>();
        inventoryItem.InitializeItem(item, Player);
    }
}
