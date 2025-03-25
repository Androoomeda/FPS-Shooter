using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    public Item Item {get; private set;}

    [HideInInspector] public Transform parent;
    [HideInInspector] public int Count = 1;

    private Image image;
    private TextMeshProUGUI countText;
    private GameObject player;

    private void OnEnable()
    {
        image = GetComponent<Image>();
        countText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void InitializeItem(Item newItem, int count, GameObject player)
    {
        Item = newItem;
        image.sprite = newItem.Sprite;
        this.player = player;
        Count = count;
        UpdateCount();
    }

    public void UpdateCount()
    {
        countText.text = Count.ToString();
        countText.gameObject.SetActive(Count > 1);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        image.raycastTarget = false;
        parent = transform.parent;
        transform.SetParent(transform.root);
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        image.raycastTarget = true;
        transform.SetParent(parent);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        switch(Item.Type)
        {
            case ItemType.Weapon:
                player.GetComponent<PlayerWeaponsManager>().SwitchWeapon(Item.WeaponPrefab);
                Destroy(gameObject);
                break;
            case ItemType.Medkit:
                player.GetComponent<Health>().Heal(Item.HealAmount);
                Destroy(gameObject);
                break;
        }
    }

}
